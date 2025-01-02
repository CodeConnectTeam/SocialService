using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialService.Controllers;
using SocialService.Models.InstagramModels;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    public async Task GetProfile_ShouldReturnOkResult_WithProfileData()
    {
        // Arrange
        var mockProfile = new InstagramProfile { Id = "123", Name = "test_user" };
        _mockInstagramService.Setup(service => service.GetProfileAsync()).ReturnsAsync(mockProfile);

        // Act
        var result = await _controller.GetProfile();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var profile = Assert.IsType<InstagramProfile>(okResult.Value);
        Assert.Equal("123", profile.Id);
        Assert.Equal("test_user", profile.Name);
    }

    [Fact]
    public async Task CreatePost_ShouldReturnOkResult_WithPostData()
    {
        // Arrange
        var mockDraftPost = new DraftPost { id = "post123" };
        var createPostRequest = new InstagramController.CreatePostRequest
        {
            ImageUrl = "http://example.com/image.jpg",
            Caption = "Test Caption",
            Media_Type = "IMAGE"
        };
        _mockInstagramService.Setup(service => service.CreatePostAsync(createPostRequest.ImageUrl, createPostRequest.Caption, createPostRequest.Media_Type))
                             .ReturnsAsync(mockDraftPost);

        // Act
        var result = await _controller.CreatePost(createPostRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var post = Assert.IsType<DraftPost>(okResult.Value);
        Assert.Equal("post123", post.id);
    }

    [Fact]
    public async Task PublishPost_ShouldReturnOkResult_WithPublishedPostData()
    {
        // Arrange
        var mockPublishedPost = new PublishedPost { id = "publish123" };
        var publishPostRequest = new InstagramController.PublishPostRequest { CreationId = "creation123" };
        _mockInstagramService.Setup(service => service.PublishPostAsync(publishPostRequest.CreationId))
                             .ReturnsAsync(mockPublishedPost);

        // Act
        var result = await _controller.PublishPost(publishPostRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var post = Assert.IsType<PublishedPost>(okResult.Value);
        Assert.Equal("publish123", post.id);
    }

    [Fact]
    public async Task GetMetrics_ShouldReturnOkResult_WithMetricsData()
    {
        // Arrange
        var mockMetrics = new List<InstagramMedia>
        {
            new InstagramMedia { Caption = "Test Caption", LikeCount = 10, CommentsCount = 5 }
        };
        _mockInstagramService.Setup(service => service.GetMetricsAsync()).ReturnsAsync(mockMetrics);

        // Act
        var result = await _controller.GetMetrics();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var metrics = Assert.IsType<List<InstagramMedia>>(okResult.Value);
        Assert.Single(metrics);
        Assert.Equal("Test Caption", metrics[0].Caption);
    }
}
