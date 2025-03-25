using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinEdge.Application.DTOs;
using FinEdge.Application.Interface;
using FinEdge.Domain.Entities;
using FinEdge.Infrasturcture.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FinEdge.Infrasturcture.Repository
{
    public class UserRepo : IUser
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UserRepo(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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

        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
            };
            var token = new JwtSecurityToken(issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
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