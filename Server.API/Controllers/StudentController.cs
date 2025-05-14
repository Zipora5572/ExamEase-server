using Microsoft.AspNetCore.Mvc;
using Server.Core.DTOs;
using Server.Core.Entities;
using Server.Core.IServices;
using Server.Service;

namespace Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
           _studentService = studentService;
        }
        [HttpGet("exam/{examId}")]
        public async Task<ActionResult<List<StudentDto>>> GetStudentsByExamId(int examId)
        {
            try
            {
                var students = await _studentService.GetStudentsByExamIdAsync(examId);
                return Ok(students);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("teacher/{teacherId}")]

        public async Task<ActionResult<List<StudentDto>>> GetStudentsByTeacherId(int teacherId)
        {
            try
            {
                var students = await _studentService.GetStudentsByTeacherIdAsync(teacherId);
                return Ok(students);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("{examId}/excel")]
        public async Task<IActionResult> UploadStudentsExcel(int examId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                using var stream = file.OpenReadStream();
                await _studentService.UploadFromExcelAsync(examId, stream);
                return Ok();
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, ex.Message);
            }
        }

    }
}
