using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RestSharp.Authenticators.OAuth;
using RestSharp.Authenticators;
using RestSharp;
using SocialService.Configurations;
using SocialService.Models.XModels;


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

    public async Task<List<Tweet>> GetUserTweetsAsync(string username)
    {
        

        var userId  = await GetUserIdByUsernameAsync(username);

        var url = $"https://api.x.com/2/users/{userId}/tweets";

        HttpResponseMessage response = await _httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            var tweetsResponse = JsonSerializer.Deserialize<GetTweetsResponse>(content);
            var tweetList = tweetsResponse?.Tweets;
            foreach(var tw in tweetList)
            {
                tw.PublicMetrics = GetTweetMetricsAsync(tw.Id).Result.Data;

            }

            return tweetList;
             
            
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
        var authenticator = OAuth1Authenticator.ForAccessToken(
        _settings.ApiKey,
        _settings.ApiSecretKey,
        _settings.AccessToken,
        _settings.AccessSecret,
        OAuthSignatureMethod.HmacSha1
        );

        var options = new RestClientOptions("https://api.x.com")
        {
            Authenticator = authenticator
        };

        var client = new RestClient(options);

        
        var request = new RestRequest("/2/tweets", Method.Post);
        request.AddHeader("Content-Type", "application/json");

        
        var bodyObj = new { text = tweetText };
        request.AddStringBody(JsonSerializer.Serialize(bodyObj), DataFormat.Json);

        
        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful)
        {
            
            throw new Exception($"Tweet gönderilemedi. {response.StatusCode} - {response.Content}");
        }

        
        var resultTweet = JsonSerializer.Deserialize<Tweet>(response.Content);
        return resultTweet;
    }

    public async Task<bool> DeleteTweetAsync(string tweetId)
    {
        var authenticator = OAuth1Authenticator.ForAccessToken(
        _settings.ApiKey,
        _settings.ApiSecretKey,
        _settings.AccessToken,
        _settings.AccessSecret,
        OAuthSignatureMethod.HmacSha1
        );

        var options = new RestClientOptions("https://api.x.com")
        {
            Authenticator = authenticator
        };

        var client = new RestClient(options);


        var request = new RestRequest($"/2/tweets/{tweetId}", Method.Delete);
        var response = await client.ExecuteAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            throw new Exception($"Tweet silinemedi:  + { response.StatusCode } - { response.Content}");
        }
    }
}
