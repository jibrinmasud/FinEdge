using FinEdge.Application.DTOs;
using FinEdge.Application.Interface;
using FinEdge.Domain.Entities;
using FinEdge.Infrasturcture.Data;
using Microsoft.EntityFrameworkCore;

namespace FinEdge.Infrasturcture.Repository
{
    public class UserRepo : IUser
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LoginResponse> LoginUserAsync(LoginDto loginDto)
        {
            //check if user exist
            var getUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (getUser == null) return new LoginResponse(false, "User Not Found");

            // check if password is correct
            bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, getUser.Password);
            if (verifyPassword)
                return new LoginResponse(true, "Login successfully", GenerateToken(getUser));
            else
                return new LoginResponse(false, "Invalid Login Details");
        }

        //private async Task<AppUser> FindUserByEmail(string emial) => await _context.Users.FirstOrDefaultAsync(u =>u.Email == email)
        public async Task<RegisterationResponse> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerUserDto.Email);
            if (existingUser != null) return new RegisterationResponse(false, "User Already Exist");
            var user = new User
            {
                UserName = registerUserDto.UserName,
                Email = registerUserDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password)
            };

            await _context.SaveChangesAsync();
            return new RegisterationResponse(true, "User Registered Successfully");
        }
    }
}