using Connect.Common;
using Connect.Constants;
using Connect.Contracts;
using Connect.DTOs;
using Connect.Errors;
using Connect.Models;
using Connect.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Connect.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly Jwt _jwt;

        public AuthService(UserManager<AppUser> userManager, IOptionsMonitor<Jwt> jwt)
        {
            _userManager = userManager;
            _jwt = jwt.CurrentValue;
        }

        public async Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            #region Validation
            if (await _userManager.FindByEmailAsync(request.Email) is not null)
                return Result<RegisterResponse>.Failure(UserErrors.UserAlreadyExists);

            if(await _userManager.FindByEmailAsync(request.Email) is not null)
                return Result<RegisterResponse>.Failure(UserErrors.EmailAlreadyExists);
            #endregion

            var user = new AppUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return Result<RegisterResponse>.Failure(
                    new Error(
                        Code: "RegistrationFailed",
                        Description: "User registration failed. Please try again.",
                        Type: ErrorType.Failure)
                );
            }

            await _userManager.AddToRoleAsync(user, Roles.User);

            var jwtSecuritytoken = await CreateJwtToken(user);
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecuritytoken);
            return Result<RegisterResponse>.Success(new RegisterResponse { 
                IsAuthenticated = true,
                Token = token, 
                ExpiresOn = jwtSecuritytoken.ValidTo 
            });
        }

        private async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserName", user.UserName)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.ExpireDays),
                signingCredentials: credentials
            );

            return token;
        }
    }
}
