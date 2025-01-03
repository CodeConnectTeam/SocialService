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

        [HttpGet("GetUserTweets")]
        public async Task<ActionResult<List<Tweet>>> GetUserTweets(string username= "mediasync252408")
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

        [HttpPost("PostTweet")]
        public async Task<ActionResult<Tweet>> PostTweet([FromBody] TweetRequest request)
        {
            try
            {
                var tweet = await _xApiService.PostTweetAsync(request.tweetText,request.postId);
                return Ok(tweet);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        


        [HttpDelete("DeleteTweet/{postId}")]
        public async Task<ActionResult> DeleteTweet(string postId)
        {
            try
            {
                var result = await _xApiService.DeleteTweetAsync(postId);
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

        public class TweetRequest
        {
            public string tweetText { get; set; }

            public int postId { get; set; }
        }

    }
}
