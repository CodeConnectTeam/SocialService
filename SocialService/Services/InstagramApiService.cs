using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using SocialService.Configurations;
using SocialService.Data;
using SocialService.Interfaces;
using SocialService.Models.InstagramModels;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;

public class InstagramService
{
    private readonly User _users;
    private readonly IRestClientWrapper _client;
    private readonly DbContextApplication _db;

    public InstagramService(IOptions<User> users, IRestClientWrapper client, DbContextApplication db)
    {
        _users = users.Value;
        _client = client;
        _db = db;
    }

    public async Task<InstagramProfile> GetProfileAsync()
    {
        var request = new RestRequest("/me", Method.Get);

        request.AddQueryParameter("fields", "id,username");
        request.AddQueryParameter("access_token", _users.AccessToken.AccessTokenLong);

        var response = await _client.ExecuteAsync(request);
        if (!response.IsSuccessful)
        {
            throw new Exception("Failed to fetch profile: " + response.ErrorMessage);
        }

        var result = System.Text.Json.JsonSerializer.Deserialize<InstagramProfile>(response.Content);
        return result ?? new InstagramProfile();
    }

    public async Task<DraftPost> CreatePostAsync(string imageUrl = null,
                                              string caption = null,
                                              string media_type = null)
    {
        var request = new RestRequest($"{_users.Id}/media", Method.Post);

        if (!string.IsNullOrEmpty(imageUrl))
            request.AddParameter("image_url", imageUrl);

        if (!string.IsNullOrEmpty(caption))
            request.AddParameter("caption", caption);


        if (!string.IsNullOrEmpty(media_type))
            request.AddParameter("media_type", media_type);


        request.AddParameter("access_token", _users.AccessToken.AccessTokenLong);

        var response = await _client.ExecuteAsync(request);
        if (!response.IsSuccessful)
        {
            throw new Exception("Failed to create post: " + response.ErrorMessage);
        }

        var result = System.Text.Json.JsonSerializer.Deserialize<DraftPost>(response.Content);
        return result ?? new DraftPost();
    }

    public async Task<PublishedPost> PublishPostAsync(string creationId, int postId)
    {
        var client = new RestClient("https://graph.instagram.com/v21.0");
        var request = new RestRequest($"{_users.Id}/media_publish", Method.Post);

        request.AddParameter("creation_id", creationId);
        request.AddParameter("access_token", _users.AccessToken.AccessTokenLong);

        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful)
        {
            throw new Exception("Failed to publish post: " + response.ErrorMessage);
        }


        var result = System.Text.Json.JsonSerializer.Deserialize<PublishedPost>(response.Content);

        //DB logic
        var publishedPost = _db.instagram_posts.FirstOrDefault(x => x.id == postId);
        publishedPost.status = "PUBLISHED";
        publishedPost.platform_id = result.id;
        publishedPost.created_at= DateTime.Now;
        _db.SaveChanges();

        return result ?? new PublishedPost();
    }

    public async Task<List<InstagramMedia>> GetMetricsAsync()
    {
        var request = new RestRequest($"{_users.Id}/media", Method.Get);

        request.AddQueryParameter("fields",
            "caption,like_count,comments_count,media_url,permalink,media_type");
        request.AddQueryParameter("access_token", _users.AccessToken.AccessTokenLong);

        var response = await _client.ExecuteAsync(request);

        if (!response.IsSuccessful)
            throw new Exception("Failed to fetch metrics: " + response.ErrorMessage);


        var result = System.Text.Json.JsonSerializer.Deserialize<GetMetricsResponse>(response.Content);

        foreach (var post in result.Data.ToList())
        {
            try
            {
                var eachpost = _db.instagram_posts.FirstOrDefault(x => x.platform_id == post.Id);

                if (eachpost == null)
                    continue; // Skip this iteration if no matching row is found

                // Update the fields
                eachpost.like_count = post.LikeCount;
                eachpost.comment_count = post.CommentsCount;
            }
            catch (Exception ex)
            {
                // Log the exception to track unexpected errors
                Console.WriteLine($"Error updating post with ID {post.Id}: {ex.Message}");
            }

        }
        _db.SaveChanges();
        return result?.Data ?? new List<InstagramMedia>();
    }

}
