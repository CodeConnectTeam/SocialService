using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialService.Controllers;
using SocialService.Models.XModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

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
    public async Task GetTweetMetrics_ShouldReturnOk_WhenTweetExists()
    {
        // Arrange
        var tweetId = "12345";
        var expectedTweet = new Tweet { Id = tweetId, Text = "Hello World!" };

        _xApiServiceMock
            .Setup(service => service.GetTweetMetricsAsync(tweetId))
            .ReturnsAsync(expectedTweet);

        // Act
        var result = await _controller.GetTweetMetrics(tweetId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTweet = Assert.IsType<Tweet>(okResult.Value);
        Assert.Equal(expectedTweet.Id, returnedTweet.Id);
    }

    [Fact]
    public async Task GetTweetMetrics_ShouldReturnBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        var tweetId = "12345";

        _xApiServiceMock
            .Setup(service => service.GetTweetMetricsAsync(tweetId))
            .ThrowsAsync(new Exception("Error fetching tweet metrics"));

        // Act
        var result = await _controller.GetTweetMetrics(tweetId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Error fetching tweet metrics", badRequestResult.Value);
    }

    [Fact]
    public async Task PostTweet_ShouldReturnOk_WhenTweetIsCreated()
    {
        // Arrange
        var tweetText = "Hello World!";
        var expectedTweet = new Tweet { Id = "12345", Text = tweetText };

        _xApiServiceMock
            .Setup(service => service.PostTweetAsync(tweetText))
            .ReturnsAsync(expectedTweet);

        // Act
        var result = await _controller.PostTweet(tweetText);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTweet = Assert.IsType<Tweet>(okResult.Value);
        Assert.Equal(expectedTweet.Text, returnedTweet.Text);
    }

    [Fact]
    public async Task PostTweet_ShouldReturnBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        var tweetText = "Hello World!";

        _xApiServiceMock
            .Setup(service => service.PostTweetAsync(tweetText))
            .ThrowsAsync(new Exception("Error posting tweet"));

        // Act
        var result = await _controller.PostTweet(tweetText);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Error posting tweet", badRequestResult.Value);
    }

    [Fact]
    public async Task GetUserTweets_ShouldReturnOk_WhenTweetsExist()
    {
        // Arrange
        var username = "testuser";
        var expectedTweets = new List<Tweet>
        {
            new Tweet { Id = "1", Text = "Tweet 1" },
            new Tweet { Id = "2", Text = "Tweet 2" }
        };

        _xApiServiceMock
            .Setup(service => service.GetUserTweetsAsync(username, It.IsAny<int>()))
            .ReturnsAsync(expectedTweets);

        // Act
        var result = await _controller.GetUserTweets(username);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTweets = Assert.IsType<List<Tweet>>(okResult.Value);
        Assert.Equal(expectedTweets.Count, returnedTweets.Count);
    }

    [Fact]
    public async Task GetUserTweets_ShouldReturnBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        var username = "testuser";

        _xApiServiceMock
            .Setup(service => service.GetUserTweetsAsync(username, It.IsAny<int>()))
            .ThrowsAsync(new Exception("Error fetching user tweets"));

        // Act
        var result = await _controller.GetUserTweets(username);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Error fetching user tweets", badRequestResult.Value);
    }

    [Fact]
    public async Task DeleteTweet_ShouldReturnOk_WhenTweetIsDeleted()
    {
        // Arrange
        var tweetId = "12345";

        _xApiServiceMock
            .Setup(service => service.DeleteTweetAsync(tweetId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteTweet(tweetId);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteTweet_ShouldReturnBadRequest_WhenDeletionFails()
    {
        // Arrange
        var tweetId = "12345";

        _xApiServiceMock
            .Setup(service => service.DeleteTweetAsync(tweetId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteTweet(tweetId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteTweet_ShouldReturnBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        var tweetId = "12345";

        _xApiServiceMock
            .Setup(service => service.DeleteTweetAsync(tweetId))
            .ThrowsAsync(new Exception("Error deleting tweet"));

        // Act
        var result = await _controller.DeleteTweet(tweetId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Error deleting tweet", badRequestResult.Value);
    }
}
