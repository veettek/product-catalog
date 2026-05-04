using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Models;
using ProductCatalog.Repositories;

namespace ProductCatalog.Controllers
{
    /// <summary>
    /// Kontroler REST API do zarządzania katalogiem produktów.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController(IProductRepository repository, ILogger<ProductsController> logger) : ControllerBase
    {
        private readonly IProductRepository _repository = repository;
        private readonly ILogger<ProductsController> _logger = logger;

        /// <summary>
        /// Pobiera listę wszystkich produktów w katalogu.
        /// </summary>
        /// <returns>Lista produktów.</returns>
        /// <response code="200">Zwraca listę produktów (może być pusta).</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var products = await _repository.GetAllAsync();
            return Ok(products);
        }

        /// <summary>
        /// Dodaje nowy produkt do katalogu.
        /// </summary>
        /// <param name="dto">Dane nowego produktu. Wszystkie pola są wymagane; cena musi być nieujemna.</param>
        /// <returns>Nowo utworzony produkt.</returns>
        /// <response code="201">Produkt został pomyślnie dodany.</response>
        /// <response code="400">Dane wejściowe są nieprawidłowe (brakujące pole lub ujemna cena).</response>
        [HttpPost]
        [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
