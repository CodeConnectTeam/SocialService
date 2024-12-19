using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SocialService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstagramController : ControllerBase
    {
        private readonly InstagramService _instagramService;

        public InstagramController(InstagramService instagramService)
        {
            _instagramService = instagramService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var profile = await _instagramService.GetProfileAsync();
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create-post")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
        {
            try
            {
                var result = await _instagramService.CreatePostAsync(request.ImageUrl, request.Caption, request.VideoUrl, request.Is_Carousel_Item, request.Media_Type, request.Children);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("publish-post")]
        public async Task<IActionResult> PublishPost([FromBody] PublishPostRequest request)
        {
            try
            {
                var result = await _instagramService.PublishPostAsync(request.CreationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-metrics")]
        public async Task<IActionResult> GetMetrics()
        {
            try
            {
                var metrics = await _instagramService.GetMetricsAsync();
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-comments/{mediaId}")]
        public async Task<IActionResult> GetComments(string mediaId)
        {
            try
            {
                var comments = await _instagramService.GetCommentsAsync(mediaId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class CreatePostRequest
        {
            public string? ImageUrl { get; set; }
            public string? Caption { get; set; }
            public string? VideoUrl { get; set; }
            public bool? Is_Carousel_Item { get; set; }
            public string? Media_Type { get; set; }
            public string? Children { get; set; }
        }

        public class PublishPostRequest
        {
            public string CreationId { get; set; }
        }
    }
}
