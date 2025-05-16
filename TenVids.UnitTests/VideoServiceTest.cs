using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TenVids.Utilities;
using TenVids.Services.HelperMethods;
using TenVids.Models.DTOs;
using AutoMapper;
using System.Security.Claims;
using TenVids.Models.Pagination;
using TenVids.Utilities.FileHelpers;
using System.Linq.Expressions;
using TenVids.Models;
using TenVids.ViewModels;
using TenVids.Data.Access.IRepo;
using TenVids.Services;

public class VideosServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IHelper> _helperMock;
  
    private readonly Mock<IPicService> _picServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly VideosService _videosService;

    public VideosServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _helperMock = new Mock<IHelper>();
       
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
    [Fact]
    public async Task CreateEditVideoAsync_ShouldReturnError_WhenVideoExists()
    {
        // Arrange
        var videoVm = new VideoVM
        {
            Title = "Existing Video",
            ImageUpload = new FormFile(Stream.Null, 0, 1024, "thumb", "thumb.jpg"),
            VideoUpload = new FormFile(Stream.Null, 0, 1024 * 1024, "video", "video.mp4")
        };

        var existingVideo = new Videos { Title = "Existing Video" };
        var userChannel = new Channel { Id = 1, AppUserId = "test-user-id" };
     
        _unitOfWorkMock.Setup(uow => uow.VideosRepository.GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<Videos, bool>>>(),
            null, false))
            .ReturnsAsync(existingVideo);

        _unitOfWorkMock.Setup(uow => uow.ChannelRepository.GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<Channel, bool>>>(),
            null, false))
            .ReturnsAsync(userChannel);

        _helperMock.Setup(h => h.IsAcceptableContentType("image", It.IsAny<string>()))
                  .Returns(true);
        _helperMock.Setup(h => h.IsAcceptableContentType("video", It.IsAny<string>()))
                  .Returns(true);

        // Act
        var result = await _videosService.CreateEditVideoAsync(videoVm);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("already exists");
    }
    [Fact]
    public async Task CreateEditVideoAsync_ShouldUpdateVideo_WhenExistingId()
    {
        // Arrange
        var userId = "test-user-id";
        var videoVm = new VideoVM
        {
            Id = 1,
            Title = "Updated Video",
            Description = "Updated Description",
            ImageUpload = new FormFile(Stream.Null, 0, 1024, "thumb.jpg", "thumb.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            },
            VideoUpload = new FormFile(Stream.Null, 0, 1024 * 1024, "video.mp4", "video.mp4")
            {
                Headers = new HeaderDictionary(),
                ContentType = "video/mp4"
            }
        };

        var existingVideo = new Videos { Id = 1 };
        var userChannel = new Channel { Id = 1, AppUserId = userId };
        var thumbnailBytes = new byte[] { 0x01, 0x02 };
        var videoBytes = new byte[] { 0x03, 0x04 };

       
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
        new Claim(ClaimTypes.NameIdentifier, userId)
    }));
        _httpContextAccessorMock.Setup(x => x.HttpContext!.User)
                              .Returns(claimsPrincipal);

    
        _unitOfWorkMock.Setup(uow => uow.ChannelRepository.GetFirstOrDefaultAsync(
            c => c.AppUserId == userId,
            null, false))
            .ReturnsAsync(userChannel);

        _helperMock.Setup(h => h.IsAcceptableContentType("image", videoVm.ImageUpload.ContentType))
                  .Returns(true);
        _helperMock.Setup(h => h.IsAcceptableContentType("video", videoVm.VideoUpload.ContentType))
                  .Returns(true);
        _helperMock.Setup(h => h.ProcessUploadedFiles(videoVm.ImageUpload))
                  .ReturnsAsync(thumbnailBytes);
        _helperMock.Setup(h => h.ProcessUploadedFiles(videoVm.VideoUpload))
                  .ReturnsAsync(videoBytes);
        _helperMock.Setup(h => h.UpdateExistingVideo(
            It.Is<VideoVM>(vm => vm.Id == 1),
            thumbnailBytes,
            videoBytes))
            .ReturnsAsync(ErrorModel<Videos>.Success(existingVideo));

        // Act
        var result = await _videosService.CreateEditVideoAsync(videoVm);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Id.Should().Be(1);

        _helperMock.Verify(h => h.UpdateExistingVideo(
            It.Is<VideoVM>(vm => vm.Id == 1),
            thumbnailBytes,
            videoBytes), Times.Once);
    }
}