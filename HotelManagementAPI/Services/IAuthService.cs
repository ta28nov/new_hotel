using HotelManagementAPI.Models.DTOs;

namespace HotelManagementAPI.Services
{
    /// <summary>
    /// Interface cho dịch vụ xác thực
    /// </summary>
    public interface IAuthService
    {
        Task<AuthResponseDTO> Login(LoginDTO loginDto);
        Task<AuthResponseDTO> Register(RegisterDTO registerDto);
        Task<UserDTO> GetCurrentUser(int userId);
        Task<bool> ChangePassword(int userId, ChangePasswordDTO changePasswordDto);
        Task<bool> ForgotPassword(ForgotPasswordDTO forgotPasswordDto);
        Task<bool> ResetPassword(ResetPasswordDTO resetPasswordDto);
    }
}