namespace HotelManagementAPI.Models.DTOs
{
    /// <summary>
    /// DTO cho đăng nhập
    /// </summary>
    public class LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    
    /// <summary>
    /// DTO cho đăng ký
    /// </summary>
    public class RegisterDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public string? PhoneNumber { get; set; }
    }
    
    /// <summary>
    /// DTO cho phản hồi xác thực
    /// </summary>
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public UserDTO User { get; set; }
    }
    
    /// <summary>
    /// DTO cho thông tin người dùng
    /// </summary>
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string? PhoneNumber { get; set; }
    }
    
    /// <summary>
    /// DTO cho đổi mật khẩu
    /// </summary>
    public class ChangePasswordDTO
    {
        public string CurrentPassword { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
    }
    
    /// <summary>
    /// DTO cho quên mật khẩu
    /// </summary>
    public class ForgotPasswordDTO
    {
        public string Email { get; set; }
    }
    
    /// <summary>
    /// DTO cho đặt lại mật khẩu
    /// </summary>
    public class ResetPasswordDTO
    {
        public string Token { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
    }
}