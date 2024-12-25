using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using SocialService.Configurations;
using SocialService.Models.InstagramModels;
using System.Text.Json;

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

    public async Task<string> CreatePostAsync(string imageUrl = null,
                                              string caption = null,
                                              string videoUrl = null,
                                              bool? is_carousel_item = null,
                                              string media_type = null,
                                              string children = null)
    {
        var client = new RestClient("https://graph.instagram.com/v21.0");
        var request = new RestRequest($"{_users.Id}/media", Method.Post);

        if (!string.IsNullOrEmpty(imageUrl))
            request.AddParameter("image_url", imageUrl);

        if (!string.IsNullOrEmpty(caption))
            request.AddParameter("caption", caption);

        if (!string.IsNullOrEmpty(videoUrl))
            request.AddParameter("video_url", videoUrl);

        if (is_carousel_item.HasValue)
            request.AddParameter("is_carousel_item", is_carousel_item.Value);

        if (!string.IsNullOrEmpty(media_type))
            request.AddParameter("media_type", media_type);

        if (!string.IsNullOrEmpty(children))
            request.AddParameter("children", children);

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

    public async Task<List<InstagramMedia>> GetMetricsAsync()
    {
        var client = new RestClient("https://graph.instagram.com/v21.0");
        var request = new RestRequest($"{_users.Id}/media", Method.Get);

        request.AddQueryParameter("fields",
            "caption,like_count,comments_count,media_url,permalink,media_type");
        request.AddQueryParameter("access_token", _users.AccessToken.AccessTokenLong);

        var response = await client.ExecuteAsync(request);

        if (!response.IsSuccessful)
            throw new Exception("Failed to fetch metrics: " + response.ErrorMessage);

        
        var result = System.Text.Json.JsonSerializer.Deserialize<GetMetricsResponse>(response.Content);

        
        return result?.Data ?? new List<InstagramMedia>();
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
