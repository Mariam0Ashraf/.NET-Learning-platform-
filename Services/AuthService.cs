using Azure.Core;
using LearningPlatform.DTOs;
using LearningPlatform.Models;
using LearningPlatform.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LearningPlatform.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, IEmailService emailService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
        }
        public async Task<string> RegisterUserAsync(RegisterDto request)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            var verificationCode = new Random().Next(100000, 999999).ToString();
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            if (existingUser != null && existingUser.VerifiedAt != null)
            {
                return "User already exists.";
            }

            if (existingUser != null && existingUser.VerifiedAt == null)
            {
                existingUser.Username = request.Username;
                existingUser.PasswordHash = passwordHash;
                existingUser.VerificationToken = verificationCode;
                existingUser.Role = request.Role;

                existingUser.VerificationTokenExpiresAt = DateTime.Now.AddMinutes(2);

                await _userRepository.UpdateUserAsync(existingUser);
            }
            else
            {
                var newUser = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    Role = request.Role,
                    PasswordHash = passwordHash,
                    VerificationToken = verificationCode,

                    VerificationTokenExpiresAt = DateTime.Now.AddMinutes(2),

                    VerifiedAt = null
                };
                await _userRepository.AddUserAsync(newUser);
            
                }

            try
            {
                _emailService.SendEmail(
               request.Email,
               "Verify Your Identity with email verificationcode",
               $"Dear user,<br><br>" + 
               $"Verification code: <b>{verificationCode}</b><br>" + 
               $"Use this to register to Learning Platform.<br>" +
               $"For your security, please don't share this code with anyone else.<br>" +
               $"If you did not make this request, ignore this message.<br><br>" +
               $"Sincerely,<br>" +
               $"LEARNING PLATFORM"
           );
            }
            catch (Exception ex)
            {
                return $"Email Error: {ex.Message}";
            }

            return "Success. Please check your email for the verification code.";
        }

        public async Task<string> VerifyEmailAsync(string email, string code)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null) return "User not found.";
            if (user.VerifiedAt != null) return "User already verified.";

            if (user.VerificationToken != code)
            {
                return "Invalid code.";
            }

            if (user.VerificationTokenExpiresAt < DateTime.Now)
            {
                return "Verification code has expired. Please register again to get a new code.";
            }

            // Success
            user.VerifiedAt = DateTime.Now;
            user.VerificationToken = null; 
            user.VerificationTokenExpiresAt = null; 

            await _userRepository.UpdateUserAsync(user);

            return "Email verified successfully!";
        }

       
        public async Task<AuthResponseDto>LoginAsync(LoginDto request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }
            if (user.VerifiedAt == null)
            {
                throw new Exception("Account not found try to register first.");
            }
            string refreshToken = GenerateRefreshToken();
            string token = CreateToken(user);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userRepository.UpdateUserAsync(user);
            return new AuthResponseDto
            {
                Username = user.Username,
                Role = user.Role,
                Email = user.Email,
                Token = token,
                RefreshToken = refreshToken
            };
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role) 
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSettings:Key").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1), 
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(string tokenRequest)
        {
           
            var user = await _userRepository.GetUserByRefreshTokenAsync(tokenRequest);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return null; 
            }

            string newAccessToken = CreateToken(user);
            string newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userRepository.UpdateUserAsync(user);

            return new AuthResponseDto
            {
                Username=user.Username,
                Email=user.Email,
                Role=user.Role,
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
    }
}
