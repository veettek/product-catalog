using FluentAssertions;
using ProductCatalog.Models;
using ProductCatalog.Repositories;
using Xunit;

namespace ProductCatalog.Tests;

/// <summary>
/// Testy jednostkowe dla <see cref="InMemoryProductRepository"/>.
/// </summary>
public class InMemoryProductRepositoryTests
{
    // -------------------------------------------------------------------------
    // GetAllAsync
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetAllAsync_ShouldReturnSeededProducts_OnInit()
    {
        // Arrange
        var repo = new InMemoryProductRepository();

        // Act
        var products = await repo.GetAllAsync();

        // Assert
        products.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProducts_AfterAdd()
    {
        // Arrange
        var repo = new InMemoryProductRepository();
        var dto = new ProductCreateDto("TEST-01", "Produkt testowy", 99.99m);

        // Act
        await repo.AddAsync(dto);
        var products = await repo.GetAllAsync();

        // Assert
        products.Should().HaveCount(4);
    }

    // -------------------------------------------------------------------------
    // AddAsync
    // -------------------------------------------------------------------------

    [Fact]
    public async Task AddAsync_ShouldReturnProductWithGeneratedId()
    {
        // Arrange
        var repo = new InMemoryProductRepository();
        var dto = new ProductCreateDto("EL-100", "Nowy produkt", 199.00m);

        // Act
        var product = await repo.AddAsync(dto);

        // Assert
        product.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task AddAsync_ShouldMapDtoFieldsCorrectly()
    {
        // Arrange
        var repo = new InMemoryProductRepository();
        var dto = new ProductCreateDto("EL-100", "Nowy produkt", 199.00m);

        // Act
        var product = await repo.AddAsync(dto);

        // Assert
        product.Kod.Should().Be("EL-100");
        product.Nazwa.Should().Be("Nowy produkt");
        product.Cena.Should().Be(199.00m);
    }

    [Fact]
    public async Task AddAsync_ShouldTrimWhitespace_FromKodAndNazwa()
    {
        // Arrange
        var repo = new InMemoryProductRepository();
        var dto = new ProductCreateDto("  EL-200  ", "  Produkt z spacjami  ", 50m);

        // Act
        var product = await repo.AddAsync(dto);

        // Assert
        product.Kod.Should().Be("EL-200");
        product.Nazwa.Should().Be("Produkt z spacjami");
    }

    [Fact]
    public async Task AddAsync_ShouldGenerateUniqueIds_ForEachProduct()
    {
        // Arrange
        var repo = new InMemoryProductRepository();
        var dto1 = new ProductCreateDto("EL-101", "Produkt A", 10m);
        var dto2 = new ProductCreateDto("EL-102", "Produkt B", 20m);

        // Act
        var p1 = await repo.AddAsync(dto1);
        var p2 = await repo.AddAsync(dto2);

        // Assert
        p1.Id.Should().NotBe(p2.Id);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistProduct_VisibleInGetAll()
    {
        // Arrange
        var repo = new InMemoryProductRepository();
        var dto = new ProductCreateDto("EL-999", "Unikalny produkt", 1m);

        // Act
        var added = await repo.AddAsync(dto);
        var all = await repo.GetAllAsync();

        // Assert
        all.Should().Contain(p => p.Id == added.Id);
    }

    [Fact]
    public async Task AddAsync_ShouldBeThreadSafe_WhenCalledConcurrently()
    {
        // Arrange
        var repo = new InMemoryProductRepository();
        var tasks = Enumerable.Range(1, 100)
            .Select(i => repo.AddAsync(new ProductCreateDto($"KOD-{i}", $"Produkt {i}", i)));

        // Act
        await Task.WhenAll(tasks);
        var all = await repo.GetAllAsync();

        // Assert – 3 seed + 100 dodanych
        all.Should().HaveCount(103);
    }
}
