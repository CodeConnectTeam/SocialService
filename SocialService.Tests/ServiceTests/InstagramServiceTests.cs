using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using RestSharp;
using SocialService.Data;
using SocialService.Interfaces;
using SocialService.Models.InstagramModels;
using Xunit;

public class InstagramServiceTests
{
    private readonly Mock<IRestClientWrapper> _mockClient;
    private readonly Mock<DbContextApplication> _mockDbContext;
    private readonly InstagramService _service;

    public InstagramServiceTests()
    {
        var mockUser = Options.Create(new User
        {
            AccessToken = new AccessToken { AccessTokenLong = "test_token" },
            Id = "test_user"
        });

        _mockClient = new Mock<IRestClientWrapper>();
        _mockDbContext = new Mock<DbContextApplication>();

        _service = new InstagramService(mockUser, _mockClient.Object, _mockDbContext.Object);
    }

    [Fact]
    public async Task GetProfileAsync_ShouldReturnProfile_WhenApiResponseIsSuccessful()
    {
        // Arrange
        var expectedProfile = new InstagramProfile { Id = "12345", Name = "test_user" };
        var response = new RestResponse
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = System.Text.Json.JsonSerializer.Serialize(expectedProfile)
        };
        _mockClient.Setup(client => client.ExecuteAsync(It.IsAny<RestRequest>())).ReturnsAsync(response);

        // Act
        var result = await _service.GetProfileAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedProfile.Id, result.Id);
        Assert.Equal(expectedProfile.Name, result.Name);
    }

    [Fact]
    public async Task GetProfileAsync_ShouldThrowException_WhenApiResponseFails()
    {
        // Arrange
        var response = new RestResponse { StatusCode = System.Net.HttpStatusCode.BadRequest };
        _mockClient.Setup(client => client.ExecuteAsync(It.IsAny<RestRequest>())).ReturnsAsync(response);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetProfileAsync());
    }

    [Fact]
    public async Task CreatePostAsync_ShouldReturnDraftPost_WhenApiResponseIsSuccessful()
    {
        // Arrange
        var expectedPost = new DraftPost { id = "post123" };
        var response = new RestResponse
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = System.Text.Json.JsonSerializer.Serialize(expectedPost)
        };
        _mockClient.Setup(client => client.ExecuteAsync(It.IsAny<RestRequest>())).ReturnsAsync(response);

        // Act
        var result = await _service.CreatePostAsync("http://image.url", "Test Caption", "IMAGE");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPost.id, result.id);
    }

    [Fact]
    public async Task PublishPostAsync_ShouldUpdateDatabaseAndReturnPublishedPost_WhenApiResponseIsSuccessful()
    {
        // Arrange
        var expectedPost = new PublishedPost { id = "published123" };
        var response = new RestResponse
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = System.Text.Json.JsonSerializer.Serialize(expectedPost)
        };

        _mockClient.Setup(client => client.ExecuteAsync(It.IsAny<RestRequest>())).ReturnsAsync(response);

        var mockPost = new instagram_post { id = 1, status = "DRAFT" };
        _mockDbContext.Setup(db => db.instagram_posts.FirstOrDefault(It.IsAny<Func<instagram_post, bool>>()))
                      .Returns(mockPost);

        // Act
        var result = await _service.PublishPostAsync("creation_id", 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPost.id, result.id);
        Assert.Equal("PUBLISHED", mockPost.status);
        _mockDbContext.Verify(db => db.SaveChanges(), Times.Once);
    }
}
