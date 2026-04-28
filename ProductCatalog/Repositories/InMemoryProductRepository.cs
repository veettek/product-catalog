using ProductCatalog.Models;
using System.Collections.Concurrent;

namespace ProductCatalog.Repositories
{
    public class InMemoryProductRepository : IProductRepository
    {
        private readonly ConcurrentDictionary<Guid, Product> _products = new();

        public InMemoryProductRepository()
        {
            var seed = new[]
            {
                new Product { Id = Guid.NewGuid(), Kod = "EL-001", Nazwa = "Laptop Lenovo Y580", Cena = 2999.99m },
                new Product { Id = Guid.NewGuid(), Kod = "EL-002", Nazwa = "Realme GT2", Cena = 1950.00m },
                new Product { Id = Guid.NewGuid(), Kod = "EL-003", Nazwa = "Philips 49PUS7502",   Cena = 3495.50m },
            };

            foreach (var p in seed)
                _products[p.Id] = p;
        }

        public Task<IEnumerable<Product>> GetAllAsync()
            => Task.FromResult(_products.Values.AsEnumerable());

        public Task<Product> AddAsync(ProductCreateDto dto)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Kod = dto.Kod.Trim(),
                Nazwa = dto.Nazwa.Trim(),
                Cena = dto.Cena
            };

            _products[product.Id] = product;
            return Task.FromResult(product);
        }
    }
}
