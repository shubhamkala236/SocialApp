using System;
using System.Collections.Generic;
using System.Text;
using AuthService.Application.DTO;

namespace AuthService.Application.Interface
{
	public interface IAuthService
	{
		Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
		Task<AuthResponseDto> LoginAsync(LoginDto dto);
	}
}
