using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialService.Controllers;
using SocialService.Models.InstagramModels;
using Xunit;

public class InstagramControllerTests
{
    private readonly Mock<InstagramService> _mockInstagramService;
    private readonly InstagramController _controller;

    public InstagramControllerTests()
    {
        _mockInstagramService = new Mock<InstagramService>();
        _controller = new InstagramController(_mockInstagramService.Object);
    }

    [Fact]
    public async Task GetProfile_ReturnsOkWithProfile()
    {
        // Arrange
        var mockProfile = new InstagramProfile { Id = "12345", Name = "TestUser" };
        _mockInstagramService.Setup(s => s.GetProfileAsync()).ReturnsAsync(mockProfile);

        // Act
        var result = await _controller.GetProfile();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockProfile, okResult.Value);
    }

    [Fact]
    public async Task GetProfile_ReturnsBadRequestOnException()
    {
        // Arrange
        _mockInstagramService.Setup(s => s.GetProfileAsync()).ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetProfile();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Test exception", badRequestResult.Value);
    }

    [Fact]
    public async Task CreatePost_ReturnsOkWithResult()
    {
        // Arrange
        var request = new InstagramController.CreatePostRequest
        {
            ImageUrl = "http://test.com/image.jpg",
            Caption = "Test caption",
            Media_Type = "image"
        };

        var mockResult = new DraftPost { id = "12345" };
        _mockInstagramService.Setup(s => s.CreatePostAsync(
            request.ImageUrl, request.Caption, request.VideoUrl,
            request.Is_Carousel_Item, request.Media_Type, request.Children))
            .ReturnsAsync(mockResult);

        // Act
        var result = await _controller.CreatePost(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResult, okResult.Value);
    }

    [Fact]
    public async Task CreatePost_ReturnsBadRequestOnException()
    {
        // Arrange
        var request = new InstagramController.CreatePostRequest
        {
            ImageUrl = "http://test.com/image.jpg",
            Caption = "Test caption",
        };

        _mockInstagramService.Setup(s => s.CreatePostAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.CreatePost(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Test exception", badRequestResult.Value);
    }

    [Fact]
    public async Task PublishPost_ReturnsOkWithResult()
    {
        // Arrange
        var request = new InstagramController.PublishPostRequest
        {
            CreationId = "12345"
        };

        var mockResult = new PublishedPost { id = "12345" };
        _mockInstagramService.Setup(s => s.PublishPostAsync(request.CreationId))
            .ReturnsAsync(mockResult);

        // Act
        var result = await _controller.PublishPost(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResult, okResult.Value);
    }

    [Fact]
    public async Task PublishPost_ReturnsBadRequestOnException()
    {
        // Arrange
        var request = new InstagramController.PublishPostRequest
        {
            CreationId = "12345"
        };

        _mockInstagramService.Setup(s => s.PublishPostAsync(request.CreationId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.PublishPost(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Test exception", badRequestResult.Value);
    }

    [Fact]
    public async Task GetMetrics_ReturnsOkWithMetrics()
    {
        // Arrange
        var mockMetrics = new List<InstagramMedia> { new InstagramMedia { Id = "12345", MediaType = "image" } };
        _mockInstagramService.Setup(s => s.GetMetricsAsync()).ReturnsAsync(mockMetrics);

        // Act
        var result = await _controller.GetMetrics();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockMetrics, okResult.Value);
    }

    [Fact]
    public async Task GetMetrics_ReturnsBadRequestOnException()
    {
        // Arrange
        _mockInstagramService.Setup(s => s.GetMetricsAsync()).ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetMetrics();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Test exception", badRequestResult.Value);
    }
}
