using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.Models.Entities
{
    /// <summary>
    /// Đối tượng hóa đơn
    /// </summary>
    public class Invoice
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int BookingId { get; set; } // ID đặt phòng
        
        [Required]
        [MaxLength(20)]
        public string InvoiceNumber { get; set; } // Số hóa đơn
        
        [Required]
        public DateTime IssueDate { get; set; } = DateTime.UtcNow; // Ngày phát hành
        
        [Required]
        public decimal SubTotal { get; set; } // Tổng tiền trước thuế
        
        [Required]
        public decimal Tax { get; set; } // Thuế
        
        [Required]
        public decimal Total { get; set; } // Tổng tiền sau thuế
        
        [MaxLength(20)]
        public string Status { get; set; } = "issued"; // Trạng thái: issued, paid, cancelled
        
        [MaxLength(500)]
        public string? Notes { get; set; } // Ghi chú
        
        // Quan hệ với đặt phòng
        public virtual Booking? Booking { get; set; }
    }
}