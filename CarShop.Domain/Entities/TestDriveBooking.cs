namespace CarShop.Domain.Entities
{
    public class TestDriveBooking : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public int CarId { get; set; }
        public DateTime BookingDate { get; set; }
        public string? Notes { get; set; }
        public TestDriveStatus Status { get; private set; } = TestDriveStatus.Pending;
        public DateTime CreatedAt { get; set; }
        public Car? Car { get; set; }

        public bool Confirm()
        {
            if (Status != TestDriveStatus.Pending) return false;
            Status = TestDriveStatus.Confirmed;
            return true;
        }

        public bool Cancel()
        {
            if (Status == TestDriveStatus.Cancelled) return false;
            Status = TestDriveStatus.Cancelled;
            return true;
        }
    }
}
