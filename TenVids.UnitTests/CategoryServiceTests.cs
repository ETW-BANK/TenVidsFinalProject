
using Moq;
using FluentAssertions;
using TenVids.Services;
using TenVids.Repository.IRepository;
using TenVids.Models;
using TenVids.ViewModels;
using TenVids.Data.Access.Data;
using Microsoft.EntityFrameworkCore;
using TenVids.Utilities.FileHelpers;
using System.Linq.Expressions;

public class CategoryServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPicService> _picServiceMock;
    private readonly Mock<ICategoryRepository> _categoryRepoMock;
    private readonly Mock<IVideosRepository> _videosRepoMock;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _picServiceMock = new Mock<IPicService>();
        _categoryRepoMock = new Mock<ICategoryRepository>();
        _videosRepoMock = new Mock<IVideosRepository>();

        var dbContextOptions = new DbContextOptionsBuilder<TenVidsApplicationContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        var dbContext = new TenVidsApplicationContext(dbContextOptions);

        _unitOfWorkMock.Setup(u => u.CategoryRepository).Returns(_categoryRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.VideosRepository).Returns(_videosRepoMock.Object);

        _service = new CategoryService(_unitOfWorkMock.Object, dbContext, _picServiceMock.Object);
    }

    [Fact]
    public async Task CreateCategoryAsync_ShouldReturnSuccess_WhenNewCategoryIsValid()
    {
        // Arrange
        var model = new CategoryVM { Name = "Test Category" };
        _categoryRepoMock.Setup(x =>x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Category, bool>>>(), null,true))

            .ReturnsAsync((Category)null);

        // Act
        var result = await _service.CreateCategoryAsync(model);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Name.Should().Be("Test Category");
        result.Message.Should().Be("Category Created Succesfully");
        _unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateCategoryAsync_ShouldReturnError_WhenCategoryExists()
    {
        // Arrange
        var model = new CategoryVM { Name = "Existing" };
        _categoryRepoMock.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Category, bool>>>(), null,false))
            .ReturnsAsync(new Category { Id = 1, Name = "Existing" });

        // Act
        var result = await _service.CreateCategoryAsync(model);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(409);
        result.Message.Should().Be("Category with this name already exists.");
        _unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Never);
    }
}
