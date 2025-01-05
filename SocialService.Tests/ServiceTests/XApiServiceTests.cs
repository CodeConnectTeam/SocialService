using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using SocialService.Configurations;
using SocialService.Data;
using SocialService.Models.XModels;
using Xunit;

namespace SocialService.Tests.Services
{
    public class XApiServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<IOptions<XApiSettings>> _settingsMock;
        private readonly Mock<DbContextApplication> _dbContextMock;
        private readonly XApiService _xApiService;

        public XApiServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _settingsMock = new Mock<IOptions<XApiSettings>>();
            _dbContextMock = new Mock<DbContextApplication>();

            _settingsMock.Setup(s => s.Value).Returns(new XApiSettings
            {
                BearerToken = "TestBearerToken",
                ApiKey = "TestApiKey",
                ApiSecretKey = "TestApiSecret",
                AccessToken = "TestAccessToken",
                AccessSecret = "TestAccessSecret"
            });

            _xApiService = new XApiService(_httpClient, _settingsMock.Object, _dbContextMock.Object);
        }

        // GetUserIdByUsernameAsync Testleri
        [Fact]
        public async Task GetUserIdByUsernameAsync_ReturnsUserId_WhenSuccessful()
        {
            // Arrange
            var username = "testuser";
            var mockResponse = new UserResponse
            {
                data = new User { id = "12345" }
            };
            var responseJson = JsonSerializer.Serialize(mockResponse);

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                });

            // Act
            var userId = await _xApiService.GetUserIdByUsernameAsync(username);

            // Assert
            Assert.Equal("12345", userId);
        }

        [Fact]
        public async Task GetUserIdByUsernameAsync_ThrowsException_WhenRequestFails()
        {
            // Arrange
            var username = "testuser";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _xApiService.GetUserIdByUsernameAsync(username));
            Assert.Contains("Kullanıcı ID'si alınamadı", exception.Message);
        }

        // GetUserTweetsAsync Testleri
        [Fact]
        public async Task GetUserTweetsAsync_ReturnsTweets_WhenSuccessful()
        {
            // Arrange
            var username = "testuser";
            var mockTweetsResponse = new GetTweetsResponse
            {
                Tweets = new List<Tweet>
                {
                    new Tweet
                    {
                        Id = "1",
                        Text = "Test tweet",
                        PublicMetrics = new TweetMetrics
                        {
                            LikeCount = 10,
                            ReplyCount = 5,
                            BookmarkCount = 2,
                            ImpressionCount = 50,
                            TweetCount = 1
                        }
                    }
                }
            };
            var responseJson = JsonSerializer.Serialize(mockTweetsResponse);

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                });

            _dbContextMock.Setup(db => db.twitter_posts.FirstOrDefault(It.IsAny<Func<twitter_post, bool>>()))
                .Returns(new twitter_post { platform_id = "1", status = "PUBLISHED" });

            // Act
            var tweets = await _xApiService.GetUserTweetsAsync(username);

            // Assert
            Assert.NotNull(tweets);
            Assert.Single(tweets);
            Assert.Equal("1", tweets[0].Id);
        }

        [Fact]
        public async Task GetUserTweetsAsync_ThrowsException_WhenRequestFails()
        {
            // Arrange
            var username = "testuser";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _xApiService.GetUserTweetsAsync(username));
            Assert.Contains("Kullanıcının tweetleri alınamadı", exception.Message);
        }

        // GetTweetMetricsAsync Testleri
        [Fact]
        public async Task GetTweetMetricsAsync_ReturnsMetrics_WhenSuccessful()
        {
            // Arrange
            var tweetId = "1";
            var mockMetricsResponse = new TweetResponse
            {
                Data = new TweetMetrics
                {
                    LikeCount = 10,
                    ReplyCount = 5,
                    BookmarkCount = 2,
                    QuoteCount = 3,
                    TweetCount = 1,
                    ImpressionCount = 50
                }
            };

            var responseJson = Newtonsoft.Json.JsonConvert.SerializeObject(mockMetricsResponse);

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                });

            // Act
            var metrics = await _xApiService.GetTweetMetricsAsync(tweetId);

            // Assert
            Assert.NotNull(metrics);
            Assert.Equal(10, metrics.Data.TweetCount);
            Assert.Equal(5, metrics.Data.ReplyCount);
            Assert.Equal(2, metrics.Data.BookmarkCount);
            Assert.Equal(3, metrics.Data.QuoteCount);
            Assert.Equal(1, metrics.Data.TweetCount);
            Assert.Equal(50, metrics.Data.ImpressionCount);
        }


        [Fact]
        public async Task GetTweetMetricsAsync_ThrowsException_WhenRequestFails()
        {
            // Arrange
            var tweetId = "1";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _xApiService.GetTweetMetricsAsync(tweetId));
            Assert.Contains("Tweet metrikleri alınamadı", exception.Message);
        }


        [Fact]
        public async Task PostTweetAsync_ReturnsPostResponse_WhenSuccessful()
        {
            // Arrange
            var tweetText = "Test tweet";
            var postId = 1;
            var mockResponse = new PostResponse
            {
                Data = new PostResponseTweet { Id = "123", Text = "Test tweet" }
            };
            var responseJson = JsonSerializer.Serialize(mockResponse);

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                });

            _dbContextMock.Setup(db => db.twitter_posts.Find(It.IsAny<int>()))
                .Returns(new twitter_post { id = postId });

            // Act
            var result = await _xApiService.PostTweetAsync(tweetText, postId);

            // Assert
            Assert.Equal("123", result.Data.Id);
            Assert.Equal("Test tweet", result.Data.Text);
        }

        [Fact]
        public async Task DeleteTweetAsync_ReturnsTrue_WhenSuccessful()
        {
            // Arrange
            var platformId = "123";
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            _dbContextMock.Setup(db => db.twitter_posts.FirstOrDefault(It.IsAny<Func<twitter_post, bool>>()))
                .Returns(new twitter_post { platform_id = platformId });

            // Act
            var result = await _xApiService.DeleteTweetAsync(platformId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteTweetAsync_ThrowsException_WhenRequestFails()
        {
            // Arrange
            var platformId = "123";
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _xApiService.DeleteTweetAsync(platformId));
            Assert.Contains("Tweet silinemedi", exception.Message);
        }
    }
}


