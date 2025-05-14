using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.API.PostModel;
using Server.Core.DTOs;
using Server.Core.Entities;
using Server.Core.IServices;
using Server.Service;

namespace Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AuthController(
            IAuthService authService,
            IUserService userService,
            IMapper mapper
            )
        {
            _authService = authService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userService.GetByEmailAsync(model.Email);
            if (user != null && _authService.VerifyPassword(model.Password, user.PasswordHash))
            {
                var token = _authService.GenerateJwtToken(user.Email, user.Roles);
                var userDto = _mapper.Map<UserDto>(user);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.Now.AddDays(7),
                    Path = "/"
                };

                Response.Cookies.Append("jwt", token, cookieOptions);

                return Ok(new { User = userDto });
            }
            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserPostModel model)
        {
            var existingUser = await _userService.GetByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return Conflict("User already exists.");
            }

            var userDto = _mapper.Map<UserDto>(model);
            userDto.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            var newUser=await _userService.AddUserAsync(userDto);

            var token = _authService.GenerateJwtToken(userDto.Email, userDto.Roles);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.Now.AddDays(7),
                Path = "/"
            };

            Response.Cookies.Append("jwt", token, cookieOptions);

            return Ok(new { User = newUser });
        }
      
        [HttpPost("logout")]
        public IActionResult Logout()
        {

            Response.Cookies.Delete("jwt"); 
            return Ok();
        }

        [HttpGet("checkAuth")]
        public async Task<IActionResult> CheckAuth()
        {
            try
            {
                // שליפת ה-Cookie
                if (!Request.Cookies.TryGetValue("jwt", out var token))
                    return Unauthorized(new { message = "No token found" });

                var userEmail = _authService.GetEmailFromToken(token);
                if (string.IsNullOrEmpty(userEmail))
                    return Unauthorized(new { message = "Invalid token" });

                var user = await _userService.GetByEmailAsync(userEmail);
                if (user == null)
                    return Unauthorized(new { message = "User not found" });

                var userDto = _mapper.Map<UserDto>(user);
                return Ok(new { user = userDto });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }


    }
}

