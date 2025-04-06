using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.Models.Entities
{
    /// <summary>
    /// Đối tượng phòng trong khách sạn
    /// </summary>
    public class Room
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string RoomNumber { get; set; } // Số phòng
        
        [Required]
        [MaxLength(50)]
        public string Type { get; set; } // Loại phòng: standard, deluxe, suite, etc.
        
        [Required]
        public int Beds { get; set; } // Số giường
        
        [Required]
        public int Capacity { get; set; } // Sức chứa (số người)
        
        [Required]
        public decimal Price { get; set; } // Giá phòng
        
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "available"; // Trạng thái: available, occupied, maintenance, cleaning
        
        [Required]
        [MaxLength(500)]
        public string Description { get; set; } // Mô tả phòng
        
        // Tiện nghi phòng (lưu dưới dạng chuỗi JSON)
        public string? Amenities { get; set; }
        
        // Quan hệ với các đối tượng khác
        public virtual ICollection<RoomImage>? Images { get; set; }
        public virtual ICollection<Booking>? Bookings { get; set; }
    }
    
    /// <summary>
    /// Đối tượng hình ảnh phòng
    /// </summary>
    public class RoomImage
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int RoomId { get; set; } // ID phòng
        
        [Required]
        [MaxLength(255)]
        public string ImageUrl { get; set; } // Đường dẫn hình ảnh
        
        public bool IsPrimary { get; set; } = false; // Có phải hình ảnh chính không
        
        // Quan hệ với phòng
        public virtual Room? Room { get; set; }
    }
}