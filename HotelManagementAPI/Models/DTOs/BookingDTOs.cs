namespace HotelManagementAPI.Models.DTOs
{
    /// <summary>
    /// DTO cho thông tin đặt phòng
    /// </summary>
    public class BookingDTO
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Guests { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public string? SpecialRequests { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    
    /// <summary>
    /// DTO cho tạo đặt phòng mới
    /// </summary>
    public class CreateBookingDTO
    {
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Guests { get; set; }
        public string? PaymentMethod { get; set; }
        public string? SpecialRequests { get; set; }
        public CustomerDetailsDTO CustomerDetails { get; set; }
    }
    
    /// <summary>
    /// DTO cho thông tin khách hàng
    /// </summary>
    public class CustomerDetailsDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
    }
    
    /// <summary>
    /// DTO cho cập nhật đặt phòng
    /// </summary>
    public class UpdateBookingDTO
    {
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int? Guests { get; set; }
        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public string? SpecialRequests { get; set; }
    }
    
    /// <summary>
    /// DTO cho lọc đặt phòng
    /// </summary>
    public class BookingFilterDTO
    {
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? PaymentStatus { get; set; }
    }
    
    /// <summary>
    /// DTO cho dịch vụ đặt phòng
    /// </summary>
    public class BookingServiceDTO
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceCategory { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
    
    /// <summary>
    /// DTO cho thêm dịch vụ vào đặt phòng
    /// </summary>
    public class AddBookingServiceDTO
    {
        public int ServiceId { get; set; }
        public int Quantity { get; set; }
    }
}