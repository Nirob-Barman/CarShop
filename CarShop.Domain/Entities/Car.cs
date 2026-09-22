
namespace CarShop.Domain.Entities
{
    public class Car : BaseEntity
    {
        public string? Title { get; private set; }
        public string? Description { get; private set; }
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }
        public string? ImageUrl { get; private set; }

        public int BrandId { get; private set; }
        public Brand? Brand { get; private set; }

        public ICollection<Comment>? Comments { get; private set; }

        private Car(
            string title,
            string? description,
            decimal price,
            int quantity,
            int brandId,
            string? imageUrl)
        {
            Title = title;
            Description = description;
            Price = price;
            Quantity = quantity;
            BrandId = brandId;
            ImageUrl = imageUrl;
        }

        public static Car Create(
            string title,
            string? description,
            decimal price,
            int quantity,
            int brandId,
            string? imageUrl = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Car title is required.", nameof(title));

            if (price <= 0)
                throw new ArgumentException("Price must be greater than zero.", nameof(price));

            if (quantity < 0)
                throw new ArgumentException("Quantity cannot be negative.", nameof(quantity));

            if (brandId <= 0)
                throw new ArgumentException("A valid brand is required.", nameof(brandId));

            return new Car(
                title.Trim(),
                description,
                price,
                quantity,
                brandId,
                imageUrl);
        }

        public void Update(
            string title,
            string? description,
            decimal price,
            int quantity,
            int brandId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Car title is required.", nameof(title));

            if (price <= 0)
                throw new ArgumentException("Price must be greater than zero.", nameof(price));

            if (quantity < 0)
                throw new ArgumentException("Quantity cannot be negative.", nameof(quantity));

            if (brandId <= 0)
                throw new ArgumentException("A valid brand is required.", nameof(brandId));

            Title = title.Trim();
            Description = description;
            Price = price;
            Quantity = quantity;
            BrandId = brandId;
        }

        public void ChangeImage(string? imageUrl)
        {
            ImageUrl = imageUrl;
        }

        public bool DecrementStock(int quantity)
        {
            if (quantity <= 0 || Quantity < quantity) return false;
            Quantity -= quantity;
            return true;
        }

        public void RestoreStock(int quantity)
        {
            if (quantity <= 0) return;
            Quantity += quantity;
        }
    }
}
