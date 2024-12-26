using Microsoft.AspNetCore.Mvc;
using SocialService.Models;
using SocialService.Models.XModels;

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

        [HttpGet("GetUserTweets/{username}")]
        public async Task<ActionResult<List<Tweet>>> GetUserTweets(string username)
        {

            try
            {
                var tweets = await _xApiService.GetUserTweetsAsync(username);
                return Ok(tweets);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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

        
        


        [HttpDelete("DeleteTweet/{tweetId}")]
        public async Task<ActionResult> DeleteTweet(string tweetId)
        {
            try
            {
                var result = await _xApiService.DeleteTweetAsync(tweetId);
                if (result)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }
}
