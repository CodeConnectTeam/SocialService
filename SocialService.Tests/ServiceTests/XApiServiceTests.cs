using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using SocialService.Configurations;
using SocialService.Models;
using Xunit;

public class XApiServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly XApiService _xApiService;

    public XApiServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://api.x.com/")
        };
        var settings = Options.Create(new XApiSettings
        {
            BearerToken = "test_token"
        });
        _xApiService = new XApiService(_httpClient, settings);
    }

    [Fact]
    public async Task GetUserIdByUsernameAsync_ShouldReturnUserId_WhenApiReturnsSuccess()
    {
        // Arrange
        var username = "testuser";
        var expectedUserId = "12345";
        var responseContent = JsonSerializer.Serialize(new UserResponse
        {
            User = new User { Id = expectedUserId }
        });

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString().Contains(username)),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        // Act
        var result = await _xApiService.GetUserIdByUsernameAsync(username);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUserId, result);
    }

    [Fact]
    public async Task GetUserTweetsAsync_ShouldReturnTweets_WhenApiReturnsSuccess()
    {
        // Arrange
        var username = "testuser";
        var expectedTweets = new List<Tweet>
        {
            new Tweet { Id = "1", Text = "Hello World" },
            new Tweet { Id = "2", Text = "Another tweet" }
        };
        var userResponse = new UserResponse { User = new User { Id = "12345" } };
        var tweetsResponse = new GetTweetsResponse { Tweets = expectedTweets };

        // Mock GetUserIdByUsernameAsync
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString().Contains("username")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(userResponse))
            });

        // Mock GetUserTweetsAsync
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString().Contains("tweets")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(tweetsResponse))
            });

        // Act
        var result = await _xApiService.GetUserTweetsAsync(username);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTweets.Count, result.Count);
        Assert.Equal(expectedTweets[0].Text, result[0].Text);
    }

    [Fact]
    public async Task GetTweetMetricsAsync_ShouldReturnTweetMetrics_WhenApiReturnsSuccess()
    {
        // Arrange
        var tweetId = "12345";
        var expectedTweet = new Tweet
        {
            Id = tweetId,
            Text = "Hello World",
            PublicMetrics = new TweetMetrics { LikeCount = 100 }
        };
        var responseContent = JsonSerializer.Serialize(expectedTweet);

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString().Contains(tweetId)),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        // Act
        var result = await _xApiService.GetTweetMetricsAsync(tweetId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTweet.Id, result.Id);
        Assert.Equal(expectedTweet.PublicMetrics.LikeCount, result.PublicMetrics.LikeCount);
    }

    [Fact]
    public async Task PostTweetAsync_ShouldReturnPostedTweet_WhenApiReturnsSuccess()
    {
        // Arrange
        var tweetText = "Hello, World!";
        var expectedTweet = new Tweet { Id = "12345", Text = tweetText };
        var responseContent = JsonSerializer.Serialize(expectedTweet);

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        // Act
        var result = await _xApiService.PostTweetAsync(tweetText);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTweet.Text, result.Text);
    }

    [Fact]
    public async Task DeleteTweetAsync_ShouldReturnTrue_WhenApiReturnsSuccess()
    {
        // Arrange
        var tweetId = "12345";

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Delete && req.RequestUri.ToString().Contains(tweetId)),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        // Act
        var result = await _xApiService.DeleteTweetAsync(tweetId);

        // Assert
        Assert.True(result);
    }
}
