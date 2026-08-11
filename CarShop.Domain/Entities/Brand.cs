namespace CarShop.Domain.Entities
{
    public class Brand : BaseEntity
    {
        public string? Name { get; private set; }

        public ICollection<Car>? Cars { get; set; }

        public Brand(string name)
        {
            Name = NormalizeName(name);
        }

        public void Rename(string name)
        {
            Name = NormalizeName(name);
        }

        private static string NormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Brand name is required.", nameof(name));
            return name.Trim();
        }
    }
}
