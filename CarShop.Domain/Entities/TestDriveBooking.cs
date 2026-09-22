using CarShop.Domain.Enums;

namespace CarShop.Domain.Entities
{
    public class TestDriveBooking : BaseEntity
    {
        public string UserId { get; private set; } = string.Empty;
        public int CarId { get; private set; }
        public DateTime BookingDate { get; private set; }
        public string? Notes { get; private set; }
        public TestDriveStatus Status { get; private set; } = TestDriveStatus.Pending;
        public DateTime CreatedAt { get; private set; }
        public Car? Car { get; private set; }


        private TestDriveBooking(
            string userId,
            int carId,
            DateTime bookingDate,
            string? notes)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required.", nameof(userId));

            if (carId <= 0)
                throw new ArgumentOutOfRangeException(nameof(carId), "Car ID must be greater than zero.");

            if (bookingDate <= DateTime.UtcNow)
                throw new ArgumentException("Booking date must be in the future.", nameof(bookingDate));

            UserId = userId;
            CarId = carId;
            BookingDate = bookingDate;
            Notes = string.IsNullOrWhiteSpace(notes)
                ? null
                : notes.Trim();

            Status = TestDriveStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public static TestDriveBooking Create(
            string userId,
            int carId,
            DateTime bookingDate,
            string? notes = null)
        {
            return new TestDriveBooking(
                userId,
                carId,
                bookingDate,
                notes);
        }


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
