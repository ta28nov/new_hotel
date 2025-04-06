using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelManagementAPI.Models.DTOs;
using HotelManagementAPI.Services;
using System.Security.Claims;

namespace HotelManagementAPI.Controllers
{
    /// <summary>
    /// Controller xử lý các yêu cầu xác thực
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        
        /// <summary>
        /// Đăng nhập
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login(LoginDTO loginDto)
        {
            try
            {
                var response = await _authService.Login(loginDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        /// <summary>
        /// Đăng ký
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> Register(RegisterDTO registerDto)
        {
            try
            {
                var response = await _authService.Register(registerDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        /// <summary>
        /// Lấy thông tin người dùng hiện tại
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetCurrentUser()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var user = await _authService.GetCurrentUser(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword(ChangePasswordDTO changePasswordDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                await _authService.ChangePassword(userId, changePasswordDto);
                return Ok(new { message = "Đổi mật khẩu thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        /// <summary>
        /// Quên mật khẩu
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordDTO forgotPasswordDto)
        {
            try
            {
                await _authService.ForgotPassword(forgotPasswordDto);
                return Ok(new { message = "Hướng dẫn đặt lại mật khẩu đã được gửi đến email của bạn" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        /// <summary>
        /// Đặt lại mật khẩu
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDTO resetPasswordDto)
        {
            try
            {
                await _authService.ResetPassword(resetPasswordDto);
                return Ok(new { message = "Đặt lại mật khẩu thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        /// <summary>
        /// Đăng xuất
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public ActionResult Logout()
        {
            // JWT là stateless, nên không cần làm gì ở phía server
            // Client nên xóa token khỏi storage
            return Ok(new { message = "Đăng xuất thành công" });
        }
    }
}