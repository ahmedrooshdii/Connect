using Connect.Contracts;
using Connect.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            if (result.IsFailure)
                return Unauthorized(result.Error);

            return Ok(result.Value);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add-to-role")]
        public async Task<IActionResult> AddToRole([FromBody] AddToRoleRequest request)
        {
            var result = await _authService.AddToRoleAsync(request.UserId, request.Role);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok();
        }
    }
}
