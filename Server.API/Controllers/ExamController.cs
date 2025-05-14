using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Server.API.PostModel;
using Server.Core.DTOs;
using Server.Core.Entities;
using Server.Core.IServices;
using Server.Service;
using System.Collections.Generic;

namespace Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly IExamService _examService;
        private readonly IFolderService _folderService;
        private readonly IMapper _mapper;

        public ExamController(
            IStorageService storageService,
            IExamService examService,
            IFolderService folderService,
          
            IMapper mapper)
        {
            _storageService = storageService;
            _examService = examService;
            _folderService = folderService;
        
            _mapper = mapper;
        }

        // GET: api/<ExamController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Exam>>> Get()
        {
            List<Exam> exams = await _examService.GetAllExamsAsync();
            if (exams == null)
            {
                return NotFound();
            }
            return Ok(exams);
        }
        [HttpGet("signed-url")]
        public IActionResult GetSignedUrl([FromQuery] string objectName)
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                return BadRequest("objectName is required.");
            }

            var url = _examService.GetSignedUrl(objectName, TimeSpan.FromMinutes(15));
            return Ok(url);
        }
        // GET api/<ExamController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamDto>> Get(int id)
        {
            ExamDto examDto = await _examService.GetByIdAsync(id);
            if (examDto == null)
            {
                return NotFound();
            }
            return Ok(examDto);
        }

        // POST api/<ExamController>
        [HttpPost("upload")]
        public async Task<ActionResult<ExamDto>> Post([FromForm] ExamPostModel examPostModel)
        {
            if (examPostModel.File == null || examPostModel.File.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
          
            var examDto = _mapper.Map<ExamDto>(examPostModel);
            try
            {
                var result = await _examService.UploadExamAsync(examDto, examPostModel.File, examPostModel.FolderId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading file: {ex.Message}");
            }
        }
        // PUT api/<ExamController>/5

        [HttpPut("{id}")]
        public async Task<ActionResult<Exam>> Put(int id, [FromBody] ExamPostModel examPostModel)
        {
            if (examPostModel == null)
            {
                return BadRequest("Exam cannot be null");
            }
            ExamDto examDto = _mapper.Map<ExamDto>(examPostModel);

            examDto = await _examService.UpdateExamAsync(id, examDto);
            if (examDto == null)
            {
                return NotFound();
            }
            return Ok(examDto);
        }
        // PATCH api/<ExamController>/update-name/5
        [HttpPatch("rename/{id}")]
        public async Task<ActionResult<ExamDto>> UpdateName(int id, [FromBody] string newName)
        {
            try
            {
                var updatedExam = await _examService.RenameExamAsync(id, newName);
                if (updatedExam == null)
                {
                    return NotFound();
                }
                return Ok(updatedExam);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating exam name: {ex.Message}");
            }
        }
        // DELETE api/<ExamController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<int>> Delete(int id)
        {

            ExamDto exam = await _examService.GetByIdAsync(id);
            try
            {

                await _examService.DeleteExamAsync(exam);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting file: {ex.Message}");
            }

            return Ok(id);
        }

        // GET api/<ExamController>/download/{filename}
        [HttpGet("download")]
        public async Task<IActionResult> Download(string fileNamePrefix)
        {

            if (string.IsNullOrEmpty(fileNamePrefix))
            {
                return BadRequest("File name cannot be null or empty.");
            }

            try
            {

                var fileStream = await _storageService.DownloadFileAsync(fileNamePrefix);
                if (fileStream == null)
                {
                    return NotFound("File not found.");
                }

                var contentType = "image/png";

                var fileName = Path.GetFileName(fileNamePrefix);
                Response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");
                return File(fileStream, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error downloading file: {ex.Message}");
            }
        }
        [HttpPatch("{id}/toggle-star")]
        public async Task<IActionResult> ToggleStar(int id)
        {
            var updatedExam = await _examService.ToggleStarAsync(id);
            if (updatedExam == null) return NotFound();
            return Ok(updatedExam);
        }


    }
}