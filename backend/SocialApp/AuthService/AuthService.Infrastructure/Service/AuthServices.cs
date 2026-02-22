using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Application.DTO;
using AuthService.Application.Interface;
using AuthService.Domain;
using AuthService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Infrastructure.Service
{
	public class AuthServices : IAuthService
	{
		private readonly AuthDbContext _context;
		private readonly IConfiguration _config;

		public AuthServices(AuthDbContext context, IConfiguration config)
		{
			_context = context;
			_config = config;
		}

		public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
		{
			var duplicateEmail = await _context.Users.AnyAsync(u => u.Email == dto.Email);
			if (duplicateEmail)
				throw new Exception("Email already exists.");

			var user = new User
			{
				Username = dto.Username,
				Email = dto.Email,
				PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
			};

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			return new AuthResponseDto
			{
				Token = GenerateJwt(user),
				Username = user.Username,
				UserId = user.Id
			};
		}

		public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email)
				?? throw new Exception("Invalid credentials.");

			if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
				throw new Exception("Invalid credentials.");

			return new AuthResponseDto
			{
				Token = GenerateJwt(user),
				Username = user.Username,
				UserId = user.Id
			};
		}

		private string GenerateJwt(User user)
		{
			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));

			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, user.Username),
				new Claim(ClaimTypes.Email, user.Email)
			};

			var token = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],
				audience: _config["Jwt:Audience"],
				claims: claims,
				expires: DateTime.Now.AddDays(7),
				signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
