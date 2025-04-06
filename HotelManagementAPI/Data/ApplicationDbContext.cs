using Microsoft.EntityFrameworkCore;
using HotelManagementAPI.Models.Entities;

namespace HotelManagementAPI.Data
{
    /// <summary>
    /// Lớp DbContext để tương tác với cơ sở dữ liệu
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomImage> RoomImages { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<BookingService> BookingServices { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        
        /// <summary>
        /// Cấu hình mối quan hệ giữa các entity
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Cấu hình mối quan hệ
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.SetNull);
                
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<RoomImage>()
                .HasOne(ri => ri.Room)
                .WithMany(r => r.Images)
                .HasForeignKey(ri => ri.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<BookingService>()
                .HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingServices)
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<BookingService>()
                .HasOne(bs => bs.Service)
                .WithMany(s => s.BookingServices)
                .HasForeignKey(bs => bs.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Booking)
                .WithOne(b => b.Invoice)
                .HasForeignKey<Invoice>(i => i.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Tạo tài khoản admin mặc định
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Admin@123");
            
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "Admin User",
                    Email = "admin@hotel.com",
                    PasswordHash = hashedPassword,
                    Role = "admin",
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}