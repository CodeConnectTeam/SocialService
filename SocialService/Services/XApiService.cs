using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SocialService.Configurations;
using SocialService.Models;
using Tweetinvi.Models.V2;

public class XApiService
{
    private readonly HttpClient _httpClient;
    private readonly SocialService.Configurations.XApiSettings _settings;

    public XApiService(HttpClient httpClient, IOptions<XApiSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;

        
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _settings.BearerToken);
    }

    public async Task<string?> GetUserIdByUsernameAsync(string username)
    {
        var url = $"https://api.x.com/2/users/by/username/{username}";

        HttpResponseMessage response = await _httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<UserResponse>(content);
            return user?.data?.id;
        }
        else
        {
            throw new Exception("Kullanıcı ID'si alınamadı: " + response.ReasonPhrase);
        }
    }

    public async Task<List<Tweet>> GetUserTweetsAsync(string username, int maxResults = 10)
    {
        

        var userId  = await GetUserIdByUsernameAsync(username);

        var url = $"https://api.x.com/2/users/{userId}/tweets?max_results={maxResults}";

        HttpResponseMessage response = await _httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            var tweetsResponse = JsonSerializer.Deserialize<GetTweetsResponse>(content);
            return tweetsResponse?.Tweets ?? new List<Tweet>();
        }
        else
        {
            throw new Exception("Kullanıcının tweetleri alınamadı: " + response.ReasonPhrase);
        }
    }

    
    public async Task<TweetResponse> GetTweetMetricsAsync(string tweetId)
    {
        var url = $"https://api.x.com/2/tweets/{tweetId}?tweet.fields=public_metrics";

        HttpResponseMessage response = await _httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            var tweet = JsonSerializer.Deserialize<TweetResponse>(content);
            return tweet;
        }
        else
        {
            throw new Exception("Tweet metrikleri alınamadı: " + response.ReasonPhrase);
        }
    }

    
    public async Task<Tweet> PostTweetAsync(string tweetText)
    {
        var url = "https://api.x.com/2/tweets";
        var tweetData = new { text = tweetText };
        var content = new StringContent(JsonSerializer.Serialize(tweetData), Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            string resultContent = await response.Content.ReadAsStringAsync();
            var resultTweet = JsonSerializer.Deserialize<Tweet>(resultContent);
            return resultTweet;
        }
        else
        {
            throw new Exception("Tweet gönderilemedi: " + response.ReasonPhrase);
        }
    }

    public async Task<bool> DeleteTweetAsync(string tweetId)
    {
        var url = $"https://api.x.com/2/tweets/{tweetId}";

        HttpResponseMessage response = await _httpClient.DeleteAsync(url);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            throw new Exception("Tweet silinemedi: " + response.ReasonPhrase);
        }
    }
}
