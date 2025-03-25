using FinEdge.Application.DTOs;

namespace FinEdge.Application.Interface
{
    public interface IUser
    {
        Task<RegisterationResponse> RegisterUserAsync(RegisterUserDto registerUserDto);

        Task<LoginResponse> LoginUserAsync(LoginDto loginDto);
    }
}