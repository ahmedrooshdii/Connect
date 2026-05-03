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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly Jwt _jwt;

        public AuthService(UserManager<AppUser> userManager, IOptionsSnapshot<Jwt> jwt, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _roleManager = roleManager;
        }

        public async Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            #region Validation
            if (await _userManager.FindByEmailAsync(request.Email) is not null)
                return Result<RegisterResponse>.Failure(UserErrors.UserAlreadyExists);

            if(await _userManager.FindByNameAsync(request.UserName) is not null)
                return Result<RegisterResponse>.Failure(UserErrors.UserAlreadyExists);
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
                var error = result.Errors.First();

                return Result<RegisterResponse>.Failure(
                    new Error(
                        Code: error.Code,
                        Description: error.Description,
                        Type: ErrorType.Validation)
                );
            }

            await _userManager.AddToRoleAsync(user, Roles.User);

            var jwtSecuritytoken = await CreateJwtToken(user);
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecuritytoken);
            return Result<RegisterResponse>.Success(new RegisterResponse { 
                Token = token, 
                ExpiresOn = jwtSecuritytoken.ValidTo 
            });
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
                return Result<LoginResponse>.Failure(UserErrors.InvalidCredentials);

            var jwtSecuritytoken = await CreateJwtToken(user);
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecuritytoken);

            return Result<LoginResponse>.Success(new LoginResponse { 
                Token = token, 
                ExpiresOn = jwtSecuritytoken.ValidTo 
            });
        }

        public async Task<Result> AddToRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure(UserErrors.NotFoundById(userId));

            if(!await _roleManager.RoleExistsAsync(role))
                return Result.Failure(RoleErrors.NotFoundByName(role));

            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(
                    new Error(
                        Code: error.Code,
                        Description: error.Description,
                        Type: ErrorType.Validation)
                );
            }
            return Result.Success();
        }

        private async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(r => new Claim("roles", r));

            var claims = new[]
            {
                new Claim("uid", user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            }
            .Union(userClaims)
            .Union(roleClaims);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwt.ExpireDays),
                signingCredentials: credentials
            );

            return token;
        }
    }
}
