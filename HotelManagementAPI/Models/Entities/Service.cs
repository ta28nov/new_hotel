using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.Models.Entities
{
    /// <summary>
    /// Đối tượng dịch vụ khách sạn
    /// </summary>
    public class Service
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // Tên dịch vụ
        
        [Required]
        [MaxLength(50)]
        public string Category { get; set; } // Danh mục: spa, restaurant, laundry, etc.
        
        [Required]
        [MaxLength(500)]
        public string Description { get; set; } // Mô tả dịch vụ
        
        [Required]
        public decimal Price { get; set; } // Giá dịch vụ
        
        public bool IsAvailable { get; set; } = true; // Có sẵn không
        
        [MaxLength(255)]
        public string? ImageUrl { get; set; } // Đường dẫn hình ảnh
        
        // Quan hệ với các đối tượng khác
        public virtual ICollection<BookingService>? BookingServices { get; set; }
    }
    
    /// <summary>
    /// Đối tượng dịch vụ đặt phòng (quan hệ nhiều-nhiều)
    /// </summary>
    public class BookingService
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int BookingId { get; set; } // ID đặt phòng
        
        [Required]
        public int ServiceId { get; set; } // ID dịch vụ
        
        [Required]
        public int Quantity { get; set; } = 1; // Số lượng
        
        [Required]
        public decimal Price { get; set; } // Giá tại thời điểm đặt
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian tạo
        
        // Quan hệ với các đối tượng khác
        public virtual Booking? Booking { get; set; }
        public virtual Service? Service { get; set; }
    }
}