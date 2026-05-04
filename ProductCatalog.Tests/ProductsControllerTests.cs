using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProductCatalog.Controllers;
using ProductCatalog.Models;
using ProductCatalog.Repositories;
using Xunit;

namespace ProductCatalog.Tests;

/// <summary>
/// Testy jednostkowe dla <see cref="ProductsController"/>.
/// </summary>
public class ProductsControllerTests
{
    private readonly Mock<IProductRepository> _repoMock;
    private readonly Mock<ILogger<ProductsController>> _loggerMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _repoMock   = new Mock<IProductRepository>();
        _loggerMock = new Mock<ILogger<ProductsController>>();
        _controller = new ProductsController(_repoMock.Object, _loggerMock.Object);
    }

    // -------------------------------------------------------------------------
    // GetAll
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetAll_ShouldReturn200_WithProductList()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Kod = "EL-001", Nazwa = "Laptop",  Cena = 3999m },
            new() { Id = Guid.NewGuid(), Kod = "EL-002", Nazwa = "Telefon", Cena = 1999m }
        };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(StatusCodes.Status200OK);
        ok.Value.Should().BeEquivalentTo(products);
    }

    [Fact]
    public async Task GetAll_ShouldReturn200_WithEmptyList_WhenNoProducts()
    {
        // Arrange
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(Enumerable.Empty<Product>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeAssignableTo<IEnumerable<Product>>()
            .Which.Should().BeEmpty();
    }

    // -------------------------------------------------------------------------
    // Add – happy path
    // -------------------------------------------------------------------------

    [Fact]
    public async Task Add_ShouldReturn201_WithCreatedProduct_WhenDtoIsValid()
    {
        // Arrange
        var dto     = new ProductCreateDto("EL-010", "iPad Pro", 4299m);
        var created = new Product { Id = Guid.NewGuid(), Kod = dto.Kod, Nazwa = dto.Nazwa, Cena = dto.Cena };
        _repoMock.Setup(r => r.AddAsync(dto)).ReturnsAsync(created);

        // Act
        var result = await _controller.Add(dto);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        createdResult.Value.Should().BeEquivalentTo(created);
    }

    [Fact]
    public async Task Add_ShouldCallRepository_ExactlyOnce_WhenDtoIsValid()
    {
        // Arrange
        var dto = new ProductCreateDto("EL-010", "iPad Pro", 4299m);
        _repoMock.Setup(r => r.AddAsync(dto))
                 .ReturnsAsync(new Product { Id = Guid.NewGuid(), Kod = dto.Kod, Nazwa = dto.Nazwa, Cena = dto.Cena });

        // Act
        await _controller.Add(dto);

        // Assert
        _repoMock.Verify(r => r.AddAsync(dto), Times.Once);
    }

    // -------------------------------------------------------------------------
    // Add – walidacja pola Kod
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null!)]
    public async Task Add_ShouldReturn400_WhenKodIsNullOrWhitespace(string? kod)
    {
        // Arrange
        var dto = new ProductCreateDto(kod!, "Nazwa", 100m);

        // Act
        var result = await _controller.Add(dto);

        // Assert
        var bad = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        bad.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<ProductCreateDto>()), Times.Never);
    }

    // -------------------------------------------------------------------------
    // Add – walidacja pola Nazwa
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null!)]
    public async Task Add_ShouldReturn400_WhenNazwaIsNullOrWhitespace(string? nazwa)
    {
        // Arrange
        var dto = new ProductCreateDto("EL-001", nazwa!, 100m);

        // Act
        var result = await _controller.Add(dto);

        // Assert
        var bad = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        bad.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<ProductCreateDto>()), Times.Never);
    }

    // -------------------------------------------------------------------------
    // Add – walidacja pola Cena
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-100)]
    [InlineData(-1)]
    public async Task Add_ShouldReturn400_WhenCenaIsNegative(decimal cena)
    {
        // Arrange
        var dto = new ProductCreateDto("EL-001", "Produkt", cena);

        // Act
        var result = await _controller.Add(dto);

        // Assert
        var bad = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        bad.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<ProductCreateDto>()), Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.01)]
    [InlineData(9999.99)]
    public async Task Add_ShouldAccept_WhenCenaIsZeroOrPositive(decimal cena)
    {
        // Arrange
        var dto = new ProductCreateDto("EL-001", "Produkt", cena);
        _repoMock.Setup(r => r.AddAsync(dto))
                 .ReturnsAsync(new Product { Id = Guid.NewGuid(), Kod = dto.Kod, Nazwa = dto.Nazwa, Cena = dto.Cena });

        // Act
        var result = await _controller.Add(dto);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
    }
}
