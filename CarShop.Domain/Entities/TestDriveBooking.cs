namespace CarShop.Domain.Entities
{
    public class TestDriveBooking : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public int CarId { get; set; }
        public DateTime BookingDate { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public Car? Car { get; set; }
    }
}
