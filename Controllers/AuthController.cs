using LearningPlatform.DTOs;
using LearningPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            var result = await _authService.RegisterUserAsync(request);

            if (!result.StartsWith("Success"))
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                if (result == null) return BadRequest("Invalid Email or Password");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {


            var result = await _authService.RefreshTokenAsync(refreshToken);

            if (result == null)
            {
                return BadRequest("Invalid Refresh Token");
            }

            return Ok(result);
        }


        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(VerifyDto request)
        {
            var result = await _authService.VerifyEmailAsync(request.Email, request.Code);

            if (result == "Invalid code." || result == "User not found.")
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}

