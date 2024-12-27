using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialService.Controllers;
using SocialService.Models.XModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class TweetControllerTests
{
    private readonly Mock<XApiService> _xApiServiceMock;
    private readonly TweetController _tweetController;

    public TweetControllerTests()
    {
        _xApiServiceMock = new Mock<XApiService>();
        _tweetController = new TweetController(_xApiServiceMock.Object);
    }

    [Fact]
    public async Task GetUserTweets_ShouldReturnOk_WhenServiceReturnsTweets()
    {
        // Arrange
        var username = "testuser";
        var mockTweets = new List<Tweet>
        {
            new Tweet { Id = "1", Text = "First tweet" },
            new Tweet { Id = "2", Text = "Second tweet" }
        };

        _xApiServiceMock
            .Setup(s => s.GetUserTweetsAsync(username))
            .ReturnsAsync(mockTweets);

        // Act
        var result = await _tweetController.GetUserTweets(username);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var tweets = Assert.IsType<List<Tweet>>(okResult.Value);
        Assert.Equal(2, tweets.Count);
        Assert.Equal("First tweet", tweets[0].Text);
    }

    [Fact]
    public async Task GetUserTweets_ShouldReturnBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var username = "testuser";

        _xApiServiceMock
            .Setup(s => s.GetUserTweetsAsync(username))
            .ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _tweetController.GetUserTweets(username);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Service error", badRequestResult.Value);
    }

    [Fact]
    public async Task PostTweet_ShouldReturnOk_WhenServiceCreatesTweet()
    {
        // Arrange
        var tweetText = "Hello from unit test!";
        var mockTweet = new Tweet
        {
            Id = "123",
            Text = tweetText
        };

        _xApiServiceMock
            .Setup(s => s.PostTweetAsync(tweetText))
            .ReturnsAsync(mockTweet);

        // Act
        var result = await _tweetController.PostTweet(tweetText);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTweet = Assert.IsType<Tweet>(okResult.Value);
        Assert.Equal("123", returnedTweet.Id);
        Assert.Equal(tweetText, returnedTweet.Text);
    }

    [Fact]
    public async Task PostTweet_ShouldReturnBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var tweetText = "This will fail";

        _xApiServiceMock
            .Setup(s => s.PostTweetAsync(tweetText))
            .ThrowsAsync(new Exception("Posting error"));

        // Act
        var result = await _tweetController.PostTweet(tweetText);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Posting error", badRequestResult.Value);
    }

    [Fact]
    public async Task DeleteTweet_ShouldReturnOk_WhenDeletionIsSuccessful()
    {
        // Arrange
        var tweetId = "123";

        _xApiServiceMock
            .Setup(s => s.DeleteTweetAsync(tweetId))
            .ReturnsAsync(true);

        // Act
        var result = await _tweetController.DeleteTweet(tweetId);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteTweet_ShouldReturnBadRequest_WhenDeletionFails()
    {
        // Arrange
        var tweetId = "123";

        _xApiServiceMock
            .Setup(s => s.DeleteTweetAsync(tweetId))
            .ReturnsAsync(false);

        // Act
        var result = await _tweetController.DeleteTweet(tweetId);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteTweet_ShouldReturnBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var tweetId = "123";

        _xApiServiceMock
            .Setup(s => s.DeleteTweetAsync(tweetId))
            .ThrowsAsync(new Exception("Deletion error"));

        // Act
        var result = await _tweetController.DeleteTweet(tweetId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Deletion error", badRequestResult.Value);
    }
}
