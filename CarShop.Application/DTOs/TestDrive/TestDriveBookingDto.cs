namespace CarShop.Application.DTOs.TestDrive
{
    public class TestDriveBookingDto
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }
        public int CarId { get; set; }
        public string? CarTitle { get; set; }
        public DateTime BookingDate { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
    }
}
