using Moq;
using RestSharp;
using SocialService.Configurations;
using SocialService.Models.InstagramModels;
using Xunit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SocialService.Interfaces;

public class InstagramServiceTests
{
    private readonly Mock<IRestClientWrapper> _mockRestClient;
    private readonly InstagramService _service;
    private readonly User _mockUser;

    public InstagramServiceTests()
    {
        _mockUser = new User
        {
            Id = "123456789",
            AccessToken = new AccessToken { AccessTokenLong = "mock_access_token" }
        };

        var mockOptions = new Mock<IOptions<User>>();
        mockOptions.Setup(o => o.Value).Returns(_mockUser);

        _mockRestClient = new Mock<IRestClientWrapper>();
        _service = new InstagramService(mockOptions.Object, _mockRestClient.Object);
    }

    [Fact]
    public async Task GetProfileAsync_ShouldReturnProfileData_WhenApiResponseIsSuccessful()
    {
        // Arrange
        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = "{ \"id\": \"123\", \"username\": \"test_user\" }"
        };
        _mockRestClient.Setup(client => client.ExecuteAsync(It.IsAny<RestRequest>())).ReturnsAsync(mockResponse);

        // Act
        var result = await _service.GetProfileAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("123", result.Id);
        Assert.Equal("test_user", result.Name);
    }

    [Fact]
    public async Task CreatePostAsync_ShouldReturnDraftPost_WhenApiResponseIsSuccessful()
    {
        // Arrange
        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = "{ \"id\": \"post123\" }"
        };
        _mockRestClient.Setup(client => client.ExecuteAsync(It.IsAny<RestRequest>())).ReturnsAsync(mockResponse);

        // Act
        var result = await _service.CreatePostAsync("http://image.url", "Test Caption", "IMAGE");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("post123", result.id);
    }

    [Fact]
    public async Task PublishPostAsync_ShouldReturnPublishedPost_WhenApiResponseIsSuccessful()
    {
        // Arrange
        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = "{ \"id\": \"publish123\" }"
        };
        _mockRestClient.Setup(client => client.ExecuteAsync(It.IsAny<RestRequest>())).ReturnsAsync(mockResponse);

        // Act
        var result = await _service.PublishPostAsync("creation123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("publish123", result.id);
    }

    [Fact]
    public async Task GetMetricsAsync_ShouldReturnMetricsList_WhenApiResponseIsSuccessful()
    {
        // Arrange
        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = "{ \"data\": [{ \"caption\": \"Test Caption\", \"like_count\": 10, \"comments_count\": 5 }] }"
        };
        _mockRestClient.Setup(client => client.ExecuteAsync(It.IsAny<RestRequest>())).ReturnsAsync(mockResponse);

        // Act
        var result = await _service.GetMetricsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Test Caption", result[0].Caption);
        Assert.Equal(10, result[0].LikeCount);
        Assert.Equal(5, result[0].CommentsCount);
    }

    [Fact]
    public async Task GetProfileAsync_ShouldThrowException_WhenApiResponseIsUnsuccessful()
    {
        // Arrange
        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.BadRequest,
            ErrorMessage = "Bad Request"
        };
        _mockRestClient.Setup(client => client.ExecuteAsync(It.IsAny<RestRequest>())).ReturnsAsync(mockResponse);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _service.GetProfileAsync());
        Assert.Contains("Failed to fetch profile", exception.Message);
    }
}
