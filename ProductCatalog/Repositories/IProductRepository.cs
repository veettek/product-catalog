using ProductCatalog.Models;

namespace ProductCatalog.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();

        Task<Product> AddAsync(ProductCreateDto dto);
    }
}
