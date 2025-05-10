using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TenVids.Services;
using TenVids.Repository.IRepository;
using TenVids.Utilities;
using TenVids.Services.HelperMethods;
using TenVids.Services.IServices;
using TenVids.Models.DTOs;
using AutoMapper;
using System.Security.Claims;
using TenVids.Models.Pagination;
using TenVids.Utilities.FileHelpers;


public class VideosServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IHelper> _helperMock;
    private readonly Mock<IVideoViewService> _videoViewServiceMock;
    private readonly Mock<IPicService> _picServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly VideosService _videosService;

    public VideosServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _helperMock = new Mock<IHelper>();
        _videoViewServiceMock = new Mock<IVideoViewService>();
        _picServiceMock = new Mock<IPicService>();
        _mapperMock = new Mock<IMapper>();

        var fileUploadOptions = Options.Create(new FileUploadConfig
        {
            ImageMaxSizeInMB = 2,
            VideoMaxSizeInMB = 100
        });

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id")
        }, "mock"));

        _httpContextAccessorMock.Setup(x => x.HttpContext!.User).Returns(user);

        _videosService = new VideosService(
            _unitOfWorkMock.Object,
            _httpContextAccessorMock.Object,
            fileUploadOptions,
            _helperMock.Object,
            _videoViewServiceMock.Object,
            _picServiceMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task GetVideosForHomeGridAsync_ShouldReturnPaginatedVideos()
    {
        // Arrange
        var parameters = new HomeParameters
        {
            PageNumber = 1,
            PageSize = 2
        };

        var videoList = new List<VideoForHomeDto>
    {
        new VideoForHomeDto { Title = "Test Video 1" },
        new VideoForHomeDto { Title = "Test Video 2" }
    };

        int totalItemCount = 10;
        int totalPages = (int)Math.Ceiling(totalItemCount / (double)parameters.PageSize);

        var fakePaginatedVideos = new PaginatedList<VideoForHomeDto>(
            videoList,
            totalpages: totalPages,
            count: totalItemCount,
            pageNumber: parameters.PageNumber,
            pageSize: parameters.PageSize
        );

        _helperMock.Setup(h => h.GetVideos(parameters))
            .ReturnsAsync(fakePaginatedVideos);

        // Act
        var result = await _videosService.GetVideosForHomeGridAsync(parameters);

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(10);
        result.PageSize.Should().Be(2);
        result.PageNumber.Should().Be(1);
        result.TotalPages.Should().Be(5);

    }
    [Fact]
    public async Task GetVideosForHomeGridAsync_ShouldReturnEmptyList_WhenNoVideosFound()
    {
        // Arrange
        var parameters = new HomeParameters
        {
            PageNumber = 1,
            PageSize = 2
        };

        var videoList = new List<VideoForHomeDto>();

        int totalItemCount = 0;
        int totalPages = 0;

        var fakePaginatedVideos = new PaginatedList<VideoForHomeDto>(
                       videoList,
                       totalpages: totalPages,
                       count: totalItemCount,
                       pageNumber: parameters.PageNumber,
                       pageSize: parameters.PageSize
                                                                          );

        _helperMock.Setup(h => h.GetVideos(parameters))
            .ReturnsAsync(fakePaginatedVideos);

        // Act
        var result = await _videosService.GetVideosForHomeGridAsync(parameters);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}