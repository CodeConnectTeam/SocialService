using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using SocialService.Configurations;
using SocialService.Models.InstagramModels;

public class InstagramService
{
    private readonly User _users;

    public InstagramService(IOptions<User> users)
    {
        _users = users.Value;
    }

    public async Task<string> GetProfileAsync()
    {
        var client = new RestClient("https://graph.instagram.com/v21.0");
        var request = new RestRequest("/me", Method.Get);

        request.AddQueryParameter("fields", "id,username");
        request.AddQueryParameter("access_token", _users.AccessToken.AccessTokenLong);

        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful)
        {
            throw new Exception("Failed to fetch profile: " + response.ErrorMessage);
        }

        return response.Content;
    }

    public async Task<string> CreatePostAsync(string imageUrl, string caption)
    {
        var client = new RestClient("https://graph.instagram.com/v21.0");
        var request = new RestRequest($"{_users.Id}/media", Method.Post);

        request.AddParameter("image_url", imageUrl);
        request.AddParameter("caption", caption);
        request.AddParameter("access_token", _users.AccessToken.AccessTokenLong);

        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful)
        {
            throw new Exception("Failed to create post: " + response.ErrorMessage);
        }

        return response.Content;
    }

    public async Task<string> PublishPostAsync(string creationId)
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

        return response.Content;
    }

    public async Task<string> GetMetricsAsync()
    {
        var client = new RestClient("https://graph.instagram.com/v21.0");
        var request = new RestRequest($"{_users.Id}/media", Method.Get);

        request.AddQueryParameter("fields", "caption,like_count,comments_count,media_url,permalink,media_type");
        request.AddQueryParameter("access_token", _users.AccessToken.AccessTokenLong);

        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful)
        {
            throw new Exception("Failed to fetch metrics: " + response.ErrorMessage);
        }

        return response.Content;
    }

    public async Task<string> GetCommentsAsync(string mediaId)
    {
        var client = new RestClient("https://graph.instagram.com/v21.0");
        var request = new RestRequest($"{mediaId}/comments", Method.Get);

        request.AddQueryParameter("fields", "like_count,replies,username,text");
        request.AddQueryParameter("access_token", _users.AccessToken.AccessTokenLong);

        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful)
        {
            throw new Exception("Failed to fetch comments: " + response.ErrorMessage);
        }

        return response.Content;
    }
}
