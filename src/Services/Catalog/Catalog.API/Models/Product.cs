namespace Catalog.API.Models
{
    public class Product
    {
        public Guid Id { get; protected set; }
        public string Name { get; protected set; } = default!;
        public List<string> Category { get; protected set; } = [];
        public string Description { get; protected set; } = default!;
        public string ImageFile { get; protected set; } = default!;
        public decimal Price { get; protected set; }

        public Product() { }

        public void UpdateProductDetails(string name, List<string> category, string description, string imageFile)
        {
            Name = name;
            Category = category;
            Description = description;
            ImageFile = imageFile;
        }

        public void UpdateProductPrice(decimal price)
        {
            if (price <= 0)
                throw new Exception("Price must be greater than 0");

            Price = price;
        }

        public abstract class ProductFactory
        {
            public static Product Create(string name, List<string> category, string description, string imageFile, decimal price)
            {
                return new Product
                {
                    Name = name,
                    Category = category,
                    Description = description,
                    ImageFile = imageFile,
                    Price = price
                };
            }
        }
    }
}
