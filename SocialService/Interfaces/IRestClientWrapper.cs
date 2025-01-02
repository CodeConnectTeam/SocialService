using RestSharp;

namespace SocialService.Interfaces
{
    public interface IRestClientWrapper
    {
        Task<RestResponse> ExecuteAsync(RestRequest request);
    }
}
