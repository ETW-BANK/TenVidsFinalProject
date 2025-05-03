using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using TenVids.Models;
using TenVids.Repository.IRepository;
using TenVids.Services;
using TenVids.ViewModels;


namespace TenVids.UnitTests
{
    public class CategoryServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _categoryService = new CategoryService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ShouldReturnAllCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            };

            _unitOfWorkMock.Setup(u => u.CategoryRepository.GetAllAsync(null, null, null))
                .ReturnsAsync(categories);

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Category1");
        }

        [Fact]
        public async Task DeleteCategoryAsync_ShouldDeleteCategory()
        {
            // Arrange
            var categoryToDelete = new Category { Id = 1, Name = "Category1" };
            _unitOfWorkMock.Setup(u => u.CategoryRepository.GetFirstOrDefaultAsync(
         It.IsAny<Expression<Func<Category, bool>>>(), "", true))
        .ReturnsAsync(categoryToDelete);

            // Act
            var result= _categoryService.DeleteCategoryAsync(categoryToDelete);

            // Assert

            result.Should().NotBeNull();
            result.IsCompleted.Should().BeTrue();
            result.IsFaulted.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteCategoryAsync_ShouldThrowException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryToDelete = new Category { Id = 1 };
            _unitOfWorkMock.Setup(u => u.CategoryRepository.GetFirstOrDefaultAsync(
             It.IsAny<Expression<Func<Category, bool>>>(),
             It.Is<string>(s => s == null),
             It.Is<bool>(b => b == false)))
             .ReturnsAsync((Category)null);


            // Act
            Func<Task> act = async () => await _categoryService.DeleteCategoryAsync(categoryToDelete);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Category not found.");
        }

        [Fact]
        public async Task CreateCategoryAsync_ShouldReturnSuccess_WhenCategoryIsCreated()
        {
            // Arrange
            var model = new CategoryVM { Name = "NewCategory" };
            _unitOfWorkMock.Setup(u => u.CategoryRepository.GetFirstOrDefaultAsync(
             It.IsAny<Expression<Func<Category, bool>>>(),
             It.Is<string>(s => s == null),
             It.Is<bool>(b => b == false)))
             .ReturnsAsync((Category)null);


            // Act
            var result = await _categoryService.CreateCategoryAsync(model);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Category Created Succesfully");
        }

        [Fact]
        public async Task CreateCategoryAsync_ShouldReturnFailure_WhenCategoryAlreadyExists()
        {
            // Arrange
            var model = new CategoryVM { Name = "ExistingCategory" };
            var existingCategory = new Category { Id = 1, Name = "ExistingCategory" };
            _unitOfWorkMock.Setup(u => u.CategoryRepository.GetFirstOrDefaultAsync(
             It.IsAny<Expression<Func<Category, bool>>>(),
             It.Is<string>(s => s == null),
             It.Is<bool>(b => b == false)))
             .ReturnsAsync(existingCategory);


            // Act
            var result = await _categoryService.CreateCategoryAsync(model);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Category with this name already exists.");
        }

        [Fact]
        public async Task UpdateCategoryAsync_ShouldReturnSuccess_WhenCategoryIsUpdated()
        {
            // Arrange
            var model = new CategoryVM { Id = 1, Name = "UpdatedCategory" };
            var existingCategory = new Category { Id = 1, Name = "OldCategory" };
            _unitOfWorkMock.Setup(u => u.CategoryRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Category, bool>>>(), null, true))
                .ReturnsAsync(existingCategory);
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(true);

            // Act
            var result = await _categoryService.UpdateCategoryAsync(model);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Category updated successfully");
        }

        [Fact]
        public async Task UpdateCategoryAsync_ShouldReturnFailure_WhenCategoryDoesNotExist()
        {
            // Arrange
            var model = new CategoryVM { Id = 1, Name = "UpdatedCategory" };
            _unitOfWorkMock.Setup(u => u.CategoryRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Category, bool>>>(), null, true))
                .ReturnsAsync((Category)null);

            // Act
            var result = await _categoryService.UpdateCategoryAsync(model);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Category doesn't exist.");
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ShouldReturnCategory_WhenCategoryExists()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Category1" };
            _unitOfWorkMock.Setup(u => u.CategoryRepository.GetFirstOrDefaultAsync(
                It.Is<Expression<Func<Category, bool>>>(x => x.Compile()(category)),
                It.Is<string>(s => s == null),
                It.Is<bool>(b => b == false)))
                .ReturnsAsync(category);

            // Act
            var result = await _categoryService.GetCategoryByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Category1");
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ShouldThrowException_WhenCategoryDoesNotExist()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.CategoryRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.Is<string>(s => s == null),
                It.Is<bool>(b => b == false)))
                .ReturnsAsync((Category?)null);

            // Act
            Func<Task> act = async () => await _categoryService.GetCategoryByIdAsync(1);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Category not found.");
        }


    }
}
