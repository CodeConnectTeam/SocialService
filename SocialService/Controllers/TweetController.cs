using Microsoft.AspNetCore.Mvc;
using SocialService.Models;

namespace SocialService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TweetController : ControllerBase
    {
        private readonly XApiService _xApiService;

        public TweetController(XApiService xApiService)
        {
            _xApiService = xApiService;
        }



        
        [HttpGet("GetTweetMetrics/{tweetId}")]
        public async Task<ActionResult<Tweet>> GetTweetMetrics(string tweetId)
        {

            try
            {
                var tweet = await _xApiService.GetTweetMetricsAsync(tweetId);
                return Ok(tweet);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        
        [HttpPost]
        public async Task<ActionResult<Tweet>> PostTweet([FromBody] string tweetText)
        {
            try
            {
                var tweet = await _xApiService.PostTweetAsync(tweetText);
                return Ok(tweet);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        [HttpGet("GetUserTweets/{username}")]
        public async Task<ActionResult<List<Tweet>>> GetUserTweets(string username, int maxResults = 10)
        {

            try
            {
                var tweets = await _xApiService.GetUserTweetsAsync(username, maxResults);
                return Ok(tweets);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
