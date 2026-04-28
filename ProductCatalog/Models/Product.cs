namespace ProductCatalog.Models
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Kod { get; set; } = string.Empty;

        public string Nazwa { get; set; } = string.Empty;

        public decimal Cena { get; set; }
    }
}
