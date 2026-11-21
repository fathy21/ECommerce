using ECommerce.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto , CancellationToken cancellationToken = default);
        Task<AuthResponseDto> LoginAsync(LoginDto dto,CancellationToken cancellationToken = default);
    }
}
