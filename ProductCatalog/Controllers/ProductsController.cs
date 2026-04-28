using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Models;
using ProductCatalog.Repositories;

namespace ProductCatalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IProductRepository repository, ILogger<ProductsController> logger) : ControllerBase
    {
        private readonly IProductRepository _repository = repository;
        private readonly ILogger<ProductsController> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _repository.GetAllAsync();
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProductCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Kod))
                return BadRequest(new { error = "Pole 'Kod' jest wymagane." });

            if (string.IsNullOrWhiteSpace(dto.Nazwa))
                return BadRequest(new { error = "Pole 'Nazwa' jest wymagane." });

            if (dto.Cena < 0)
                return BadRequest(new { error = "Cena nie może być ujemna." });

            var product = await _repository.AddAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { id = product.Id }, product);
        }
    }
}
