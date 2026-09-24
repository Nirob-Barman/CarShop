using CarShop.Domain.Entities;
using FluentAssertions;

namespace CarShop.Domain.UnitTests.Entities
{
    public class BrandTests
    {
        [Fact]
        public void Constructor_ShouldCreateBrand_WhenNameIsValid()
        {
            // Arrange
            var name = "Toyota";

            // Act
            var brand = new Brand(name);

            // Assert
            brand.Name.Should().Be("Toyota");
        }

        [Fact]
        public void Constructor_ShouldTrimName_WhenNameContainsWhitespace()
        {
            // Arrange
            var name = "  Toyota  ";

            // Act
            var brand = new Brand(name);

            // Assert
            brand.Name.Should().Be("Toyota");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void Constructor_ShouldThrowArgumentException_WhenNameIsNullOrWhitespace(
            string name)
        {
            // Act
            var act = () => new Brand(name);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Brand name is required. (Parameter 'name')");
        }

        [Fact]
        public void Rename_ShouldChangeBrandName_WhenNewNameIsValid()
        {
            // Arrange
            var brand = new Brand("Toyota");

            // Act
            brand.Rename("Honda");

            // Assert
            brand.Name.Should().Be("Honda");
        }

        [Fact]
        public void Rename_ShouldTrimNewName_WhenNewNameContainsWhitespace()
        {
            // Arrange
            var brand = new Brand("Toyota");

            // Act
            brand.Rename("  Honda  ");

            // Assert
            brand.Name.Should().Be("Honda");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void Rename_ShouldThrowArgumentException_WhenNewNameIsNullOrWhitespace(
            string name)
        {
            // Arrange
            var brand = new Brand("Toyota");

            // Act
            var act = () => brand.Rename(name);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Brand name is required. (Parameter 'name')");
        }

        [Fact]
        public void Rename_ShouldKeepOriginalName_WhenNewNameIsInvalid()
        {
            // Arrange
            var brand = new Brand("Toyota");

            // Act
            var act = () => brand.Rename("   ");

            // Assert
            act.Should().Throw<ArgumentException>();
            brand.Name.Should().Be("Toyota");
        }
    }
}
