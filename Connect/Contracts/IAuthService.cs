using Connect.Common;
using Connect.DTOs;

namespace Connect.Contracts
{
    public interface IAuthService
    {
        Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request);
        Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
    }
}
