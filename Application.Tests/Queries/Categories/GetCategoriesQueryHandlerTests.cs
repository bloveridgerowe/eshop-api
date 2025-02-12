using Application.DataTransferObjects;
using Application.Queries.Categories;
using Domain.Entities;
using Domain.Repositories;
using Moq;
using Xunit;

namespace Application.Tests.Queries.Categories;

public class GetCategoriesQueryHandlerTests
{
    private static readonly Guid ValidId1 = Guid.NewGuid();
    private static readonly Guid ValidId2 = Guid.NewGuid();
    private static readonly String ValidName1 = "Electronics";
    private static readonly String ValidName2 = "Books";

    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly GetCategoriesQueryHandler _queryHandler;
    private readonly List<Category> _categories;

    public GetCategoriesQueryHandlerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _queryHandler = new GetCategoriesQueryHandler(_categoryRepositoryMock.Object);

        _categories =
        [
            new Category(ValidId1, ValidName1),
            new Category(ValidId2, ValidName2)
        ];
    }

    public class HandleMethod : GetCategoriesQueryHandlerTests
    {
        [Fact]
        public async Task WithCategories_ReturnsAllCategories()
        {
            // Arrange
            GetCategoriesQuery query = new GetCategoriesQuery();
            _categoryRepositoryMock
                .Setup(x => x.GetAllCategoriesAsync())
                .ReturnsAsync(_categories);

            // Act
            GetCategoriesQueryResponse response = await _queryHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(2, response.Categories.Count);
            
            CategoryDetails category1 = response.Categories[0];
            Assert.Equal(ValidId1, category1.Id);
            Assert.Equal(ValidName1, category1.Name);
            
            CategoryDetails category2 = response.Categories[1];
            Assert.Equal(ValidId2, category2.Id);
            Assert.Equal(ValidName2, category2.Name);
        }

        [Fact]
        public async Task WithNoCategories_ReturnsEmptyList()
        {
            // Arrange
            GetCategoriesQuery query = new GetCategoriesQuery();
            _categoryRepositoryMock
                .Setup(x => x.GetAllCategoriesAsync())
                .ReturnsAsync([]);

            // Act
            GetCategoriesQueryResponse response = await _queryHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.Empty(response.Categories);
        }
    }
} 