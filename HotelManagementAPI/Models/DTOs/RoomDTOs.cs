namespace HotelManagementAPI.Models.DTOs
{
    /// <summary>
    /// DTO cho thông tin phòng
    /// </summary>
    public class RoomDTO
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public string Type { get; set; }
        public int Beds { get; set; }
        public int Capacity { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public List<string> Amenities { get; set; }
        public List<RoomImageDTO> Images { get; set; }
    }
    
    /// <summary>
    /// DTO cho tạo phòng mới
    /// </summary>
    public class CreateRoomDTO
    {
        public string RoomNumber { get; set; }
        public string Type { get; set; }
        public int Beds { get; set; }
        public int Capacity { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public List<string> Amenities { get; set; }
    }
    
    /// <summary>
    /// DTO cho cập nhật phòng
    /// </summary>
    public class UpdateRoomDTO
    {
        public string RoomNumber { get; set; }
        public string Type { get; set; }
        public int Beds { get; set; }
        public int Capacity { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public List<string> Amenities { get; set; }
    }
    
    /// <summary>
    /// DTO cho hình ảnh phòng
    /// </summary>
    public class RoomImageDTO
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public bool IsPrimary { get; set; }
    }
    
    /// <summary>
    /// DTO cho lọc phòng
    /// </summary>
    public class RoomFilterDTO
    {
        public string? Type { get; set; }
        public string? Status { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Capacity { get; set; }
        public List<string>? Amenities { get; set; }
    }
    
    /// <summary>
    /// DTO cho kiểm tra tình trạng phòng
    /// </summary>
    public class RoomAvailabilityDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? Guests { get; set; }
    }
}