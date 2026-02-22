using AuthService.Application.DTO;
using AuthService.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controller
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
		public async Task<IActionResult> Register(RegisterDto dto)
		{
			try 
			{ 
				return Ok(await _authService.RegisterAsync(dto)); 
			}
			catch (Exception ex) 
			{ 
				return BadRequest(new { message = ex.Message }); 
			}
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginDto dto)
		{
			try 
			{ 
				return Ok(await _authService.LoginAsync(dto)); 
			}
			catch (Exception ex) 
			{ 
				return Unauthorized(new { message = ex.Message }); 
			}
		}
	}
}
