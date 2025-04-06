using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using HotelManagementAPI.Data;
using HotelManagementAPI.Models.DTOs;
using HotelManagementAPI.Models.Entities;

namespace HotelManagementAPI.Controllers
{
    /// <summary>
    /// Controller xử lý các yêu cầu liên quan đến phòng
    /// </summary>
    [Route("api/rooms")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        
        public RoomsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        
        /// <summary>
        /// Lấy danh sách tất cả phòng
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetRooms()
        {
            var rooms = await _context.Rooms
                .Include(r => r.Images)
                .ToListAsync();
                
            return Ok(rooms.Select(r => MapRoomToDTO(r)));
        }
        
        /// <summary>
        /// Lấy thông tin một phòng theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDTO>> GetRoom(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Images)
                .FirstOrDefaultAsync(r => r.Id == id);
                
            if (room == null)
            {
                return NotFound(new { message = "Không tìm thấy phòng" });
            }
            
            return Ok(MapRoomToDTO(room));
        }
        
        /// <summary>
        /// Tạo phòng mới
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "admin,employee")]
        public async Task<ActionResult<RoomDTO>> CreateRoom(CreateRoomDTO createRoomDto)
        {
            var room = new Room
            {
                RoomNumber = createRoomDto.RoomNumber,
                Type = createRoomDto.Type,
                Beds = createRoomDto.Beds,
                Capacity = createRoomDto.Capacity,
                Price = createRoomDto.Price,
                Status = createRoomDto.Status,
                Description = createRoomDto.Description,
                Amenities = JsonSerializer.Serialize(createRoomDto.Amenities)
            };
            
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, MapRoomToDTO(room));
        }
        
        /// <summary>
        /// Cập nhật thông tin phòng
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,employee")]
        public async Task<ActionResult<RoomDTO>> UpdateRoom(int id, UpdateRoomDTO updateRoomDto)
        {
            var room = await _context.Rooms
                .Include(r => r.Images)
                .FirstOrDefaultAsync(r => r.Id == id);
                
            if (room == null)
            {
                return NotFound(new { message = "Không tìm thấy phòng" });
            }
            
            room.RoomNumber = updateRoomDto.RoomNumber;
            room.Type = updateRoomDto.Type;
            room.Beds = updateRoomDto.Beds;
            room.Capacity = updateRoomDto.Capacity;
            room.Price = updateRoomDto.Price;
            room.Status = updateRoomDto.Status;
            room.Description = updateRoomDto.Description;
            room.Amenities = JsonSerializer.Serialize(updateRoomDto.Amenities);
            
            await _context.SaveChangesAsync();
            
            return Ok(MapRoomToDTO(room));
        }
        
        /// <summary>
        /// Xóa phòng
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            
            if (room == null)
            {
                return NotFound(new { message = "Không tìm thấy phòng" });
            }
            
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            
            return Ok(new { message = "Xóa phòng thành công" });
        }
        
        /// <summary>
        /// Lọc phòng theo các tiêu chí
        /// </summary>
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> FilterRooms([FromQuery] RoomFilterDTO filter)
        {
            var query = _context.Rooms.Include(r => r.Images).AsQueryable();
            
            if (!string.IsNullOrEmpty(filter.Type))
            {
                query = query.Where(r => r.Type == filter.Type);
            }
            
            if (!string.IsNullOrEmpty(filter.Status))
            {
                query = query.Where(r => r.Status == filter.Status);
            }
            
            if (filter.MinPrice.HasValue)
            {
                query = query.Where(r => r.Price >= filter.MinPrice.Value);
            }
            
            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(r => r.Price <= filter.MaxPrice.Value);
            }
            
            if (filter.Capacity.HasValue)
            {
                query = query.Where(r => r.Capacity >= filter.Capacity.Value);
            }
            
            if (filter.Amenities != null && filter.Amenities.Any())
            {
                foreach (var amenity in filter.Amenities)
                {
                    query = query.Where(r => r.Amenities.Contains(amenity));
                }
            }
            
            var rooms = await query.ToListAsync();
            
            return Ok(rooms.Select(r => MapRoomToDTO(r)));
        }
        
        /// <summary>
        /// Kiểm tra tình trạng phòng trống
        /// </summary>
        [HttpGet("availability")]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> CheckAvailability([FromQuery] RoomAvailabilityDTO availabilityDto)
        {
            if (availabilityDto.StartDate >= availabilityDto.EndDate)
            {
                return BadRequest(new { message = "Ngày bắt đầu phải trước ngày kết thúc" });
            }
            
            // Lấy tất cả phòng
            var allRooms = await _context.Rooms
                .Include(r => r.Images)
                .Where(r => r.Status == "available")
                .ToListAsync();
                
            // Lấy các đặt phòng trùng với khoảng thời gian yêu cầu
            var overlappingBookings = await _context.Bookings
                .Where(b => 
                    (b.Status == "confirmed" || b.Status == "checked_in") &&
                    b.CheckInDate < availabilityDto.EndDate &&
                    b.CheckOutDate > availabilityDto.StartDate)
                .ToListAsync();
                
            // Lấy ID của các phòng đã đặt trong khoảng thời gian này
            var bookedRoomIds = overlappingBookings.Select(b => b.RoomId).Distinct().ToList();
            
            // Lọc ra các phòng còn trống
            var availableRooms = allRooms.Where(r => !bookedRoomIds.Contains(r.Id)).ToList();
            
            // Lọc theo số lượng khách nếu có
            if (availabilityDto.Guests.HasValue)
            {
                availableRooms = availableRooms.Where(r => r.Capacity >= availabilityDto.Guests.Value).ToList();
            }
            
            return Ok(availableRooms.Select(r => MapRoomToDTO(r)));
        }
        
        /// <summary>
        /// Lấy danh sách loại phòng
        /// </summary>
        [HttpGet("types")]
        public ActionResult GetRoomTypes()
        {
            var types = _context.Rooms
                .Select(r => r.Type)
                .Distinct()
                .ToList();
                
            return Ok(types);
        }
        
/// <summary>
/// Lấy danh sách tiện nghi phòng
/// </summary>
[HttpGet("amenities")]
public ActionResult GetRoomAmenities()
{
    var amenities = _context.Rooms
        .Where(r => r.Amenities != null)
        .AsEnumerable() // chuyển sang LINQ to Objects
        .SelectMany(r => JsonSerializer.Deserialize<List<string>>(r.Amenities))
        .Distinct()
        .ToList();

    return Ok(amenities);
}

        /// <summary>
        /// Tải lên hình ảnh phòng
        /// </summary>
        [HttpPost("{roomId}/images")]
        [Authorize(Roles = "admin,employee")]
        public async Task<ActionResult<RoomImageDTO>> UploadRoomImage(int roomId, IFormFile image)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            
            if (room == null)
            {
                return NotFound(new { message = "Không tìm thấy phòng" });
            }
            
            if (image == null || image.Length == 0)
            {
                return BadRequest(new { message = "Không có file hình ảnh" });
            }
            
            // Kiểm tra xem file có phải là hình ảnh không
            if (!image.ContentType.StartsWith("image/"))
            {
                return BadRequest(new { message = "File không phải là hình ảnh" });
            }
            
            // Tạo thư mục uploads nếu chưa tồn tại
            var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads", "rooms");
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }
            
            // Tạo tên file duy nhất
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            var filePath = Path.Combine(uploadsDir, fileName);
            
            // Lưu file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            
            // Tạo bản ghi hình ảnh phòng mới
            var roomImage = new RoomImage
            {
                RoomId = roomId,
                ImageUrl = $"/uploads/rooms/{fileName}",
                IsPrimary = !await _context.RoomImages.AnyAsync(ri => ri.RoomId == roomId)
            };
            
            _context.RoomImages.Add(roomImage);
            await _context.SaveChangesAsync();
            
            return Ok(new RoomImageDTO
            {
                Id = roomImage.Id,
                ImageUrl = roomImage.ImageUrl,
                IsPrimary = roomImage.IsPrimary
            });
        }
        
        /// <summary>
        /// Xóa hình ảnh phòng
        /// </summary>
        [HttpDelete("{roomId}/images/{imageId}")]
        [Authorize(Roles = "admin,employee")]
        public async Task<ActionResult> DeleteRoomImage(int roomId, int imageId)
        {
            var roomImage = await _context.RoomImages
                .FirstOrDefaultAsync(ri => ri.Id == imageId && ri.RoomId == roomId);
                
            if (roomImage == null)
            {
                return NotFound(new { message = "Không tìm thấy hình ảnh phòng" });
            }
            
            // Xóa file
            var filePath = Path.Combine(_environment.WebRootPath, roomImage.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            
            _context.RoomImages.Remove(roomImage);
            
            // Nếu đây là hình ảnh chính, đặt hình ảnh khác làm hình ảnh chính
            if (roomImage.IsPrimary)
            {
                var nextImage = await _context.RoomImages
                    .FirstOrDefaultAsync(ri => ri.RoomId == roomId && ri.Id != imageId);
                    
                if (nextImage != null)
                {
                    nextImage.IsPrimary = true;
                }
            }
            
            await _context.SaveChangesAsync();
            
            return Ok(new { message = "Xóa hình ảnh phòng thành công" });
        }
        
        /// <summary>
        /// Chuyển đổi từ entity Room sang DTO
        /// </summary>
        private RoomDTO MapRoomToDTO(Room room)
        {
            var amenities = room.Amenities != null
                ? JsonSerializer.Deserialize<List<string>>(room.Amenities)
                : new List<string>();
                
            return new RoomDTO
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                Type = room.Type,
                Beds = room.Beds,
                Capacity = room.Capacity,
                Price = room.Price,
                Status = room.Status,
                Description = room.Description,
                Amenities = amenities,
                Images = room.Images?.Select(ri => new RoomImageDTO
                {
                    Id = ri.Id,
                    ImageUrl = ri.ImageUrl,
                    IsPrimary = ri.IsPrimary
                }).ToList() ?? new List<RoomImageDTO>()
            };
        }
    }
}