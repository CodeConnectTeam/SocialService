using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialService.Controllers;
using SocialService.Models;
using SocialService.Models.XModels;
using Xunit;

namespace SocialService.Tests.Controllers
{
    public class TweetControllerTests
    {
        private readonly Mock<XApiService> _xApiServiceMock;
        private readonly TweetController _controller;

        public TweetControllerTests()
        {
            _xApiServiceMock = new Mock<XApiService>();
            _controller = new TweetController(_xApiServiceMock.Object);
        }

        [Fact]
        public async Task GetUserTweets_ReturnsOkResult_WhenServiceSucceeds()
        {
            // Arrange
            var mockTweets = new List<Tweet>
            {
                new Tweet { Id = "1", Text = "Hello World" },
                new Tweet { Id = "2", Text = "Second tweet" }
            };
            _xApiServiceMock.Setup(s => s.GetUserTweetsAsync(It.IsAny<string>()))
                .ReturnsAsync(mockTweets);

            // Act
            var result = await _controller.GetUserTweets("testuser");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(mockTweets, okResult.Value);
        }

        [Fact]
        public async Task GetUserTweets_ReturnsBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            _xApiServiceMock.Setup(s => s.GetUserTweetsAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.GetUserTweets("testuser");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Service error", badRequestResult.Value);
        }

        [Fact]
        public async Task PostTweet_ReturnsOkResult_WhenServiceSucceeds()
        {
            // Arrange
            var mockResponse = new PostResponse
            {
                Data = new PostResponseTweet
                {
                    Id = "123",
                    Text = "Posted successfully"
                }
            };

            var request = new TweetController.TweetRequest
            {
                tweetText = "Test tweet",
                post_id = 1
            };

            _xApiServiceMock.Setup(s => s.PostTweetAsync(request.tweetText, request.post_id))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.PostTweet(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(mockResponse, okResult.Value);
        }

        [Fact]
        public async Task PostTweet_ReturnsBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            var request = new TweetController.TweetRequest
            {
                tweetText = "Test tweet",
                post_id = 1
            };

            _xApiServiceMock.Setup(s => s.PostTweetAsync(request.tweetText, request.post_id))
                .ThrowsAsync(new Exception("Failed to post tweet"));

            // Act
            var result = await _controller.PostTweet(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Failed to post tweet", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteTweet_ReturnsOkResult_WhenServiceSucceeds()
        {
            // Arrange
            var platformId = "123";
            _xApiServiceMock.Setup(s => s.DeleteTweetAsync(platformId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTweet(platformId);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteTweet_ReturnsBadRequest_WhenServiceFails()
        {
            // Arrange
            var platformId = "123";
            _xApiServiceMock.Setup(s => s.DeleteTweetAsync(platformId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteTweet(platformId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteTweet_ReturnsBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            var platformId = "123";
            _xApiServiceMock.Setup(s => s.DeleteTweetAsync(platformId))
                .ThrowsAsync(new Exception("Failed to delete tweet"));

            // Act
            var result = await _controller.DeleteTweet(platformId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to delete tweet", badRequestResult.Value);
        }
    }
}
