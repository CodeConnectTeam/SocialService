using RestSharp;

namespace SocialService.Interfaces
{
    public class RestClientWrapper : IRestClientWrapper
    {
        private readonly RestClient _client;

        public RestClientWrapper(string baseUrl)
        {
            _client = new RestClient(baseUrl);
        }

        public async Task<RestResponse> ExecuteAsync(RestRequest request)
        {
            return await _client.ExecuteAsync(request);
        }
    }
}
