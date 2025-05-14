using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Server.API.PostModel;
using Server.Core.DTOs;
using Server.Core.IServices;
using Server.Service;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Server.Core.Entities;

namespace Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentExamController : ControllerBase
    {
        private readonly IStudentExamService _studentExamService;
        private readonly IStudentService _studentService;
        private readonly IStorageService _storageService;
        private readonly IExamService _examService;
        private readonly IFolderService _folderService;
        private readonly IMapper _mapper;

        public StudentExamController(
            IStudentExamService studentExamService,
            IStudentService studentService,
            IMapper mapper,
            IStorageService storageService,
            IExamService examService, 
            IFolderService folderService)
        {
            _studentExamService = studentExamService;
            _studentService = studentService;
            _mapper = mapper;
            _storageService = storageService;
            _examService = examService;
            _folderService = folderService;

        }

        // GET: api/<StudentExamController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentExamDto>>> Get()
        {
            List<StudentExamDto> studentExams = await _studentExamService.GetAllStudentExamsAsync();
            return Ok(studentExams);
        }

        // GET api/<StudentExamController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentExamDto>> Get(int id)
        {
            StudentExamDto studentExam = await _studentExamService.GetByIdAsync(id);
            if (studentExam == null)
            {
                return NotFound();
            }
            return Ok(studentExam);
        }

        // GET api/<StudentExamController>/exam/{examId}
        [HttpGet("exam/{examId}")]
        public async Task<ActionResult<IEnumerable<StudentExamDto>>> GetByExamId(int examId)
        {
            var studentExams = await _studentExamService.GetStudentExamsByExamIdAsync(examId);
            return Ok(studentExams);
        }

        [HttpPost("uploadStudentExam")]
        public async Task<ActionResult> UploadStudentExam([FromForm] StudentExamPostModel studentExamPostModel)
        {
            if (studentExamPostModel.Files == null || studentExamPostModel.Files.Count == 0)
            {
                return BadRequest("No files uploaded.");
            }

            var exam = await _examService.GetByIdAsync(studentExamPostModel.ExamId);
            if (exam == null)
            {
                return NotFound("Exam not found.");
            }

            var teacherExamFolder = exam.Name;
            string Name = Path.GetDirectoryName(studentExamPostModel.Files[0].FileName);
            var studentExams = new List<StudentExamDto>();
            var folderDto = new FolderDto
            {
                Name = Name,
                UserId = exam.UserId,
                OfTeacherExams = false,
                NamePrefix = Name
            };
            folderDto = await _folderService.AddFolderAsync(folderDto);
            int id = folderDto.Id;
            var uploadTasks = studentExamPostModel.Files.Select(file =>
            {
                string fileName = Path.GetFileName(file.FileName);
                var objectName = $"{teacherExamFolder}/{fileName}";
                var filePath = Path.Combine(Path.GetTempPath(), fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return _storageService.UploadFileAsync(filePath, objectName);
            }).ToList();

            await Task.WhenAll(uploadTasks);
            var httpClient = new HttpClient();
            foreach (var file in studentExamPostModel.Files)
            {
                var objectName = $"{teacherExamFolder}/{Path.GetFileName(file.FileName)}";
                var studentExamDto = _mapper.Map<StudentExamDto>(studentExamPostModel);

                var students = await _studentService.GetStudentsByExamIdAsync(studentExamPostModel.ExamId);
                var studentNames = students.Select(s => $"{s.FirstName} {s.LastName}").ToList();
                
                using var stream = file.OpenReadStream();
                using var form = new MultipartFormDataContent();

                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                form.Add(fileContent, "file", file.FileName);
                form.Add(new StringContent(studentExamPostModel.Lang ?? "he", Encoding.UTF8, "text/plain"), "lang");

                var namesJson = JsonConvert.SerializeObject(studentNames);
                form.Add(new StringContent(namesJson, Encoding.UTF8, "text/plain"), "student_names_list");

                var flaskResponse = await httpClient.PostAsync("http://localhost:5000/extract-name", form);
                if (!flaskResponse.IsSuccessStatusCode)
                    return StatusCode((int)flaskResponse.StatusCode, flaskResponse);


                var flaskContent = await flaskResponse.Content.ReadAsStringAsync();
                var nameResult = JsonConvert.DeserializeObject<dynamic>(flaskContent);
                string firstName = nameResult.firstName;
                string lastName = nameResult.lastName;
                var student = await _studentService.GetByNameAsync(firstName, lastName);
                var studentExam= await _studentExamService.GetByStudentIdAsync(student.Id);

                studentExamDto.ExamPath = $"https://storage.cloud.google.com/exams-bucket/{objectName}";
                studentExamDto.StudentId =student.Id;
                studentExamDto.NamePrefix = objectName;
                studentExamDto.FolderId = id;

                var savedStudentExam = await _studentExamService.UpdateStudentExamAsync(studentExam.Id,studentExamDto);

                //var savedStudentExam = await _studentExamService.AddStudentExamAsync(studentExamDto);
                studentExams.Add(savedStudentExam);


                var filePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(file.FileName));
                System.IO.File.Delete(filePath);
            }

            return Ok(new { message = "Files uploaded successfully.", studentExams });
        }



        // POST api/<StudentExamController>
        [HttpPost]
        public async Task<ActionResult<StudentExamDto>> Post([FromBody] StudentExamPostModel studentExamPostModel)
        {
            if (studentExamPostModel == null)
            {
                return BadRequest("StudentExam cannot be null");
            }
            var studentExamDto = _mapper.Map<StudentExamDto>(studentExamPostModel);
            studentExamDto = await _studentExamService.AddStudentExamAsync(studentExamDto);
            return CreatedAtAction(nameof(Get), new { id = studentExamDto.Id }, studentExamDto);
        }

        // PUT api/<StudentExamController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<StudentExamDto>> Put(int id, [FromBody] StudentExamPostModel studentExamPostModel)
        {

            if (studentExamPostModel == null)
            {
                return BadRequest("StudentExam cannot be null");
            }

            var existingStudentExamDto = await _studentExamService.GetByIdAsync(id);
            if (existingStudentExamDto == null)
            {
                return NotFound();
            }

            var updatedStudentExamDto = _mapper.Map<StudentExamDto>(studentExamPostModel);


            updatedStudentExamDto.Id = id; // Ensure the ID is set for the update
            updatedStudentExamDto.CheckedAt = DateTime.Now;
            updatedStudentExamDto.IsChecked = true;

            updatedStudentExamDto = await _studentExamService.UpdateStudentExamAsync(id, updatedStudentExamDto);
            return Ok(updatedStudentExamDto);
        }

        // DELETE api/<StudentExamController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var studentExam = await _studentExamService.GetByIdAsync(id);
            if (studentExam == null)
            {
                return NotFound();
            }
            await _studentExamService.DeleteStudentExamAsync(studentExam);
            return NoContent();
        }

        [HttpPost("{id}/upload-corrected")]
        public async Task<IActionResult> UploadCorrectedImage(int id, IFormFile correctedImage)
        {
            if (correctedImage == null || correctedImage.Length == 0)
                return BadRequest("No file uploaded.");

            using var stream = correctedImage.OpenReadStream();

            string contentType = correctedImage.ContentType;

            await _studentExamService.ReplaceCorrectedImageAsync(id, stream);
            return Ok("Corrected image uploaded successfully.");
        }

        // POST api/<StudentExamController>
        [HttpPost("autoCheck")]
        public async Task<ActionResult<AutoCheckResponse>> AutoCheck([FromBody] AutoCheckRequest request)
        {
            try
            {
                using var httpClient = new HttpClient();

                // Download the files from the storage service
                var studentStream = await _storageService.DownloadFileAsync(request.StudentExamUrl);
                var teacherStream = await _storageService.DownloadFileAsync(request.TeacherExamUrl);

                if (studentStream == null || teacherStream == null)
                    return BadRequest("One of the exam files could not be found.");

                using var form = new MultipartFormDataContent();

                var studentContent = new StreamContent(studentStream);
                studentContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                form.Add(studentContent, "student_exam", "student_exam.png");

                var teacherContent = new StreamContent(teacherStream);
                teacherContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                form.Add(teacherContent, "teacher_exam", "teacher_exam.png");

                form.Add(new StringContent(request.Lang), "lang");


                var response = await httpClient.PostAsync("http://localhost:5000/grade", form);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Failed to process grading in Flask service.");

                var responseString = await response.Content.ReadAsStringAsync();

                dynamic result = JsonConvert.DeserializeObject(responseString);
                string? grade = result.grade;
                string evaluation = result.evaluation;

                return Ok(new AutoCheckResponse
                {
                    Grade = grade,
                    Evaluation = evaluation
                });
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "An internal error occurred during auto check.");
            }
        }

       

    }
}
