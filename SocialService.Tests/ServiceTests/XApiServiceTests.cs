using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using RestSharp;
using SocialService.Configurations;
using SocialService.Models.XModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class XApiServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly XApiService _xApiService;
    private readonly Mock<IOptions<XApiSettings>> _settingsMock;

    public XApiServiceTests()
    {
        // Mock HttpMessageHandler for HttpClient
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://api.x.com/")
        };

        // Mock XApiSettings
        _settingsMock = new Mock<IOptions<XApiSettings>>();
        _settingsMock.Setup(s => s.Value).Returns(new XApiSettings
        {
            BearerToken = "test_bearer_token",
            ApiKey = "test_api_key",
            ApiSecretKey = "test_api_secret",
            AccessToken = "test_access_token",
            AccessSecret = "test_access_secret"
        });

        _xApiService = new XApiService(_httpClient, _settingsMock.Object);
    }

    [Fact]
    public async Task GetUserIdByUsernameAsync_ShouldReturnUserId_WhenSuccessful()
    {
        // Arrange
        var username = "testuser";
        var expectedUserId = "12345";
        var userResponse = new UserResponse
        {
            data = new User { id = expectedUserId }
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri.ToString().Contains($"users/by/username/{username}")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(userResponse), Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _xApiService.GetUserIdByUsernameAsync(username);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUserId, result);
    }

    [Fact]
    public async Task GetUserIdByUsernameAsync_ShouldThrowException_WhenResponseFails()
    {
        // Arrange
        var username = "testuser";

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                ReasonPhrase = "Bad Request"
            });

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _xApiService.GetUserIdByUsernameAsync(username));
        Assert.Contains("Kullanıcı ID'si alınamadı", ex.Message);
    }

    [Fact]
    public async Task GetUserTweetsAsync_ShouldReturnTweets_WhenSuccessful()
    {
        // Arrange
        var username = "testuser";
        var userId = "12345";
        var expectedTweets = new List<Tweet>
        {
            new Tweet { Id = "1", Text = "First tweet" },
            new Tweet { Id = "2", Text = "Second tweet" }
        };

        // Mock GetUserIdByUsernameAsync
        _httpMessageHandlerMock
            .Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            // First call: Get user ID
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new UserResponse
                {
                    data = new User { id = userId }
                }), Encoding.UTF8, "application/json")
            })
            // Second call: Get user tweets
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new GetTweetsResponse
                {
                    Tweets = expectedTweets
                }), Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _xApiService.GetUserTweetsAsync(username);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTweets.Count, result.Count);
        Assert.Equal("First tweet", result[0].Text);
    }

    [Fact]
    public async Task GetUserTweetsAsync_ShouldThrowException_WhenResponseFails()
    {
        // Arrange
        var username = "testuser";

        _httpMessageHandlerMock
            .Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            // First call: Get user ID succeeds
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new UserResponse
                {
                    data = new User { id = "12345" }
                }), Encoding.UTF8, "application/json")
            })
            // Second call: Get tweets fails
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                ReasonPhrase = "Bad Request"
            });

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _xApiService.GetUserTweetsAsync(username));
        Assert.Contains("Kullanıcının tweetleri alınamadı", ex.Message);
    }

    [Fact]
    public async Task PostTweetAsync_ShouldReturnTweet_WhenSuccessful()
    {
        // Arrange
        var tweetText = "Hello, world!";
        var expectedTweet = new Tweet { Id = "12345", Text = tweetText };

        var restClientMock = new Mock<RestClient>();
        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonSerializer.Serialize(expectedTweet)
        };

        restClientMock
            .Setup(client => client.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        var service = new XApiService(new HttpClient(), restClientMock.Object, CreateSettings());

        // Act
        var result = await service.PostTweetAsync(tweetText);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTweet.Id, result.Id);
        Assert.Equal(expectedTweet.Text, result.Text);
    }

    [Fact]
    public async Task DeleteTweetAsync_ShouldReturnTrue_WhenSuccessful()
    {
        // Arrange
        var tweetId = "12345";

        var restClientMock = new Mock<RestClient>();
        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK
        };

        restClientMock
            .Setup(client => client.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        var service = new XApiService(new HttpClient(), restClientMock.Object, CreateSettings());

        // Act
        var result = await service.DeleteTweetAsync(tweetId);

        // Assert
        Assert.True(result);
    }


    private Mock<XApiService> CreatePartialMockService()
    {
        var serviceMock = new Mock<XApiService>(_httpClient, _settingsMock.Object) { CallBase = true };
        return serviceMock;
    }
}
