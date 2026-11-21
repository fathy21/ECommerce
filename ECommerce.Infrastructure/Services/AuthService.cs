using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Interfaces;
using ECommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager ,IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto,CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }
            var isPasswordValid = await _userManager.CheckPasswordAsync( user, dto.Password);
            if (!isPasswordValid)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }
            var token = await GenerateJwtTokenAsync(user);
            var duration = int.Parse(_configuration["Jwt:DurationInMinutes"]!);
            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful.",
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Token = token,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(duration)
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto,CancellationToken cancellationToken = default)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser is not null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email is already registered.",
                    Errors = ["Email is already registered."]
                };
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Registration failed.",
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Customer");

            if (!roleResult.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User created, but role assignment failed.",
                    Errors = roleResult.Errors.Select(e => e.Description)
                };
            }

            return new AuthResponseDto
            {
                Success = true,
                Message = "User registered successfully.",
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email
            };
        }
        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            { 
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };
            var roles = await _userManager.GetRolesAsync(user);
            Console.WriteLine($"User: {user.Email}");
            Console.WriteLine($"Roles: {string.Join(", ", roles)}");
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var duration = int.Parse(jwtSettings["DurationInMinutes"]!);
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(duration),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
