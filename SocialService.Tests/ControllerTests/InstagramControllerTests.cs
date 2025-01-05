using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialService.Controllers;
using SocialService.Models.InstagramModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class InstagramControllerTests
{
    private readonly Mock<InstagramService> _mockService;
    private readonly InstagramController _controller;

    public InstagramControllerTests()
    {
        _mockService = new Mock<InstagramService>();
        _controller = new InstagramController(_mockService.Object);
    }

    [Fact]
    public async Task GetProfile_ShouldReturnOk_WhenServiceReturnsProfile()
    {
        // Arrange
        var mockProfile = new InstagramProfile { Id = "123", Name = "test_user" };
        _mockService.Setup(s => s.GetProfileAsync()).ReturnsAsync(mockProfile);

        // Act
        var result = await _controller.GetProfile();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(mockProfile, okResult.Value);
    }

    [Fact]
    public async Task GetProfile_ShouldReturnBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        _mockService.Setup(s => s.GetProfileAsync()).ThrowsAsync(new Exception("Error fetching profile"));

        // Act
        var result = await _controller.GetProfile();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Equal("Error fetching profile", badRequestResult.Value);
    }

    [Fact]
    public async Task CreatePost_ShouldReturnOk_WhenServiceCreatesPost()
    {
        // Arrange
        var mockPost = new DraftPost { id = "post123" };
        var request = new InstagramController.CreatePostRequest
        {
            imageUrl = "http://image.url",
            caption = "Test Caption",
            media_Type = "IMAGE"
        };

        _mockService.Setup(s => s.CreatePostAsync(request.imageUrl, request.caption, request.media_Type))
                    .ReturnsAsync(mockPost);

        // Act
        var result = await _controller.CreatePost(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(mockPost, okResult.Value);
    }

    [Fact]
    public async Task CreatePost_ShouldReturnBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var request = new InstagramController.CreatePostRequest
        {
            imageUrl = "http://image.url",
            caption = "Test Caption",
            media_Type = "IMAGE"
        };

        _mockService.Setup(s => s.CreatePostAsync(request.imageUrl, request.caption, request.media_Type))
                    .ThrowsAsync(new Exception("Error creating post"));

        // Act
        var result = await _controller.CreatePost(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Equal("Error creating post", badRequestResult.Value);
    }

    [Fact]
    public async Task PublishPost_ShouldReturnOk_WhenServicePublishesPost()
    {
        // Arrange
        var mockPublishedPost = new PublishedPost { id = "published123" };
        var request = new InstagramController.PublishPostRequest
        {
            creationId = "creation123",
            postId = 1
        };

        _mockService.Setup(s => s.PublishPostAsync(request.creationId, request.postId))
                    .ReturnsAsync(mockPublishedPost);

        // Act
        var result = await _controller.PublishPost(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(mockPublishedPost, okResult.Value);
    }

    [Fact]
    public async Task PublishPost_ShouldReturnBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var request = new InstagramController.PublishPostRequest
        {
            creationId = "creation123",
            postId = 1
        };

        _mockService.Setup(s => s.PublishPostAsync(request.creationId, request.postId))
                    .ThrowsAsync(new Exception("Error publishing post"));

        // Act
        var result = await _controller.PublishPost(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Equal("Error publishing post", badRequestResult.Value);
    }

    [Fact]
    public async Task GetMetrics_ShouldReturnOk_WhenServiceReturnsMetrics()
    {
        // Arrange
        var mockMetrics = new List<InstagramMedia>
        {
            new InstagramMedia { Id = "media_1", LikeCount = 100, CommentsCount = 10 },
            new InstagramMedia { Id = "media_2", LikeCount = 200, CommentsCount = 20 }
        };

        _mockService.Setup(s => s.GetMetricsAsync()).ReturnsAsync(mockMetrics);

        // Act
        var result = await _controller.GetMetrics();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(mockMetrics, okResult.Value);
    }

    [Fact]
    public async Task GetMetrics_ShouldReturnBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        _mockService.Setup(s => s.GetMetricsAsync()).ThrowsAsync(new Exception("Error fetching metrics"));

        // Act
        var result = await _controller.GetMetrics();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Equal("Error fetching metrics", badRequestResult.Value);
    }
}
