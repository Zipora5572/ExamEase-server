using Microsoft.AspNetCore.Mvc;
using Server.Core.Entities;
using Server.Core.IServices;

namespace Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmailAsync([FromBody] EmailRequest request)
        {
            Console.WriteLine($"To: {request.To}, Subject: {request.Subject}, Body: {request.Body}");

            await _emailService.SendEmailAsync(request);
            return Ok();
        }
    }
}
