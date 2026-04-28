using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Models;

namespace ProductCatalog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private static readonly Product[] seed = new[]
        {
            new Product { Id = Guid.NewGuid(), Kod = "EL-001", Nazwa = "Laptop Lenovo Y580", Cena = 2999.99m },
            new Product { Id = Guid.NewGuid(), Kod = "EL-002", Nazwa = "Realme GT2", Cena = 1950.00m },
            new Product { Id = Guid.NewGuid(), Kod = "EL-003", Nazwa = "Philips 49PUS7502",   Cena = 3495.50m },
        };

        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ILogger<ProductsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Product> GetAll()
        {
            return seed;
        }
    }
}
