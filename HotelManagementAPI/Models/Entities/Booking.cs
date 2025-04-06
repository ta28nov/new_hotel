using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.Models.Entities
{
    /// <summary>
    /// Đối tượng đặt phòng
    /// </summary>
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        
        public int? UserId { get; set; } // ID người dùng (có thể null nếu khách không đăng ký)
        
        [Required]
        public int RoomId { get; set; } // ID phòng
        
        [Required]
        public DateTime CheckInDate { get; set; } // Ngày nhận phòng
        
        [Required]
        public DateTime CheckOutDate { get; set; } // Ngày trả phòng
        
        [Required]
        public int Guests { get; set; } // Số lượng khách
        
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "pending"; // Trạng thái: pending, confirmed, checked_in, checked_out, cancelled
        
        [Required]
        public decimal TotalAmount { get; set; } // Tổng tiền
        
        [MaxLength(20)]
        public string PaymentStatus { get; set; } = "pending"; // Trạng thái thanh toán: pending, paid, partially_paid, refunded
        
        [MaxLength(20)]
        public string? PaymentMethod { get; set; } // Phương thức thanh toán
        
        [MaxLength(500)]
        public string? SpecialRequests { get; set; } // Yêu cầu đặc biệt
        
        // Thông tin khách hàng (cho khách không đăng ký)
        [MaxLength(100)]
        public string? CustomerFirstName { get; set; } // Tên khách hàng
        
        [MaxLength(100)]
        public string? CustomerLastName { get; set; } // Họ khách hàng
        
        [MaxLength(100)]
        public string? CustomerEmail { get; set; } // Email khách hàng
        
        [MaxLength(20)]
        public string? CustomerPhone { get; set; } // Số điện thoại khách hàng
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian tạo
        
        public DateTime? UpdatedAt { get; set; } // Thời gian cập nhật
        
        // Quan hệ với các đối tượng khác
        public virtual User? User { get; set; }
        public virtual Room? Room { get; set; }
        public virtual ICollection<BookingService>? BookingServices { get; set; }
        public virtual Invoice? Invoice { get; set; }
    }
}