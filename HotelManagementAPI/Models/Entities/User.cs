using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.Models.Entities
{
    /// <summary>
    /// Đối tượng người dùng trong hệ thống
    /// </summary>
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // Tên người dùng
        
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } // Email người dùng
        
        [Required]
        public string PasswordHash { get; set; } // Mật khẩu đã được mã hóa
        
        [MaxLength(20)]
        public string? PhoneNumber { get; set; } // Số điện thoại
        
        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "customer"; // Vai trò: admin, employee, customer
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian tạo
        
        public DateTime? UpdatedAt { get; set; } // Thời gian cập nhật
        
        // Quan hệ với các đối tượng khác
        public virtual ICollection<Booking>? Bookings { get; set; }
    }
}