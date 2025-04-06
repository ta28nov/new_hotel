using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using HotelManagementAPI.Data;
using HotelManagementAPI.Models.DTOs;
using HotelManagementAPI.Models.Entities;

namespace HotelManagementAPI.Services
{
    /// <summary>
    /// Dịch vụ xác thực người dùng
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        
        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        
        /// <summary>
        /// Đăng nhập người dùng
        /// </summary>
        public async Task<AuthResponseDTO> Login(LoginDTO loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);
                
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new Exception("Email hoặc mật khẩu không đúng");
            }
            
            var token = GenerateJwtToken(user);
            
            return new AuthResponseDTO
            {
                Token = token,
                User = new UserDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    PhoneNumber = user.PhoneNumber
                }
            };
        }
        
        /// <summary>
        /// Đăng ký người dùng mới
        /// </summary>
        public async Task<AuthResponseDTO> Register(RegisterDTO registerDto)
        {
            if (registerDto.Password != registerDto.PasswordConfirmation)
            {
                throw new Exception("Mật khẩu xác nhận không khớp");
            }
            
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == registerDto.Email);
                
            if (existingUser != null)
            {
                throw new Exception("Email đã được sử dụng");
            }
            
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            
            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                PhoneNumber = registerDto.PhoneNumber,
                Role = "customer" // Vai trò mặc định cho người dùng mới
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            var token = GenerateJwtToken(user);
            
            return new AuthResponseDTO
            {
                Token = token,
                User = new UserDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    PhoneNumber = user.PhoneNumber
                }
            };
        }
        
        /// <summary>
        /// Lấy thông tin người dùng hiện tại
        /// </summary>
        public async Task<UserDTO> GetCurrentUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            
            if (user == null)
            {
                throw new Exception("Không tìm thấy người dùng");
            }
            
            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                PhoneNumber = user.PhoneNumber
            };
        }
        
        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        public async Task<bool> ChangePassword(int userId, ChangePasswordDTO changePasswordDto)
        {
            if (changePasswordDto.Password != changePasswordDto.PasswordConfirmation)
            {
                throw new Exception("Mật khẩu xác nhận không khớp");
            }
            
            var user = await _context.Users.FindAsync(userId);
            
            if (user == null)
            {
                throw new Exception("Không tìm thấy người dùng");
            }
            
            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                throw new Exception("Mật khẩu hiện tại không đúng");
            }
            
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.Password);
            user.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            return true;
        }
        
        /// <summary>
        /// Quên mật khẩu
        /// </summary>
        public async Task<bool> ForgotPassword(ForgotPasswordDTO forgotPasswordDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == forgotPasswordDto.Email);
                
            if (user == null)
            {
                // Không tiết lộ rằng người dùng không tồn tại
                return true;
            }
            
            // Trong ứng dụng thực tế, tạo token đặt lại mật khẩu và gửi email
            // Đối với ví dụ này, chúng ta chỉ trả về true
            
            return true;
        }
        
        /// <summary>
        /// Đặt lại mật khẩu
        /// </summary>
        public async Task<bool> ResetPassword(ResetPasswordDTO resetPasswordDto)
        {
            if (resetPasswordDto.Password != resetPasswordDto.PasswordConfirmation)
            {
                throw new Exception("Mật khẩu xác nhận không khớp");
            }
            
            // Trong ứng dụng thực tế, xác thực token và tìm người dùng
            // Đối với ví dụ này, chúng ta chỉ trả về true
            
            return true;
        }
        
        /// <summary>
        /// Tạo JWT token
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JWT:TokenValidityInMinutes"])),
                signingCredentials: credentials
            );
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}