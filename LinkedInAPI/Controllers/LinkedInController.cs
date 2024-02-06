using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LinkedInAPI.Controllers
{
    [ApiController]
    [Route("api/linkedin")]
    public class LinkedInController : Controller
    {
        private readonly string LinkedInApiUrl = "https://api.linkedin.com/v2/shares";
        private readonly string AccessToken = "AccessToken";

        private readonly LinkedInApiClient _linkedInApiClient;

        public LinkedInController(LinkedInApiClient linkedInApiClient)
        {
            _linkedInApiClient = linkedInApiClient;
        }

        [HttpPost("add-text-post")]
        public async Task<IActionResult> AddTextOnlyPost([FromBody] LinkedInTextPost post)
        {
            if (post == null || string.IsNullOrEmpty(post.PostedText?.PostedText))
            {
                return BadRequest("Invalid post data");
            }
            
            try
            {
                // Call the LinkedIn API to post a text-only update
                await _linkedInApiClient.PostTextUpdate(LinkedInApiUrl, AccessToken, post.PostedText.PostedText);

                return Ok("Text-only post successfully added to LinkedIn!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        //[HttpPost("add-image-post")]
        //public async Task<IActionResult> AddPostWithImage([FromBody] LinkedInPostWithImage postWithImage)
        //{
        //    if (postWithImage == null || string.IsNullOrEmpty(postWithImage.Text?.PostedText))
        //    {
        //        return BadRequest("Invalid post data");
        //    }

        //    if (postWithImage.Image == null || postWithImage.Image.Length == 0)
        //    {
        //        return BadRequest("Image is required");
        //    }

        //    try
        //    {
        //        // Call the LinkedIn API to post an update with an image
        //        await _linkedInApiClient.AddPostWithImage(LinkedInApiUrl, AccessToken, postWithImage.Text.PostedText, postWithImage.Image);

        //        return Ok("Post with image successfully added to LinkedIn!");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error: {ex.Message}");
        //    }
        //}

        //[HttpPost("add-video-post")]
        //public async Task<IActionResult> AddPostWithVideo([FromBody] LinkedInPostWithVideo postWithVideo)
        //{
        //    if (postWithVideo == null || string.IsNullOrEmpty(postWithVideo.Text?.PostedText))
        //    {
        //        return BadRequest("Invalid post data");
        //    }

        //    if (postWithVideo.Video == null || postWithVideo.Video.Length == 0)
        //    {
        //        return BadRequest("Video is required");
        //    }


        //    try
        //    {
        //        // Call the LinkedIn API to post an update with a video
        //        await _linkedInApiClient.AddPostWithVideo(LinkedInApiUrl, AccessToken, postWithVideo.Text.PostedText, postWithVideo.Video);

        //        return Ok("Post with video successfully added to LinkedIn!");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error: {ex.Message}");
        //    }
        //}

        [HttpDelete("delete-post/{postId}")]
        public async Task<IActionResult> DeletePost(string postId)
        {
            if (string.IsNullOrEmpty(postId))
            {
                return BadRequest("Post ID is required");
            }

            try
            {
                // Call the LinkedIn API to delete a post
                await _linkedInApiClient.DeletePost(AccessToken, postId);

                return Ok("Post successfully deleted on LinkedIn!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost("add-comment-to-post/{postId}")]
        public async Task<IActionResult> AddCommentToPost(string postId, [FromBody] CommentRequest commentRequest)
        {
            if (string.IsNullOrEmpty(postId) || commentRequest == null || string.IsNullOrEmpty(commentRequest.CommentText))
            {
                return BadRequest("Invalid request data");
            }

            try
            {
                // Call the LinkedIn API to add a comment to the post
                await _linkedInApiClient.AddCommentToPost(AccessToken, postId, commentRequest.CommentText);

                return Ok("Comment successfully added to the post on LinkedIn!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("view-all-comments/{postId}")]
        public async Task<IActionResult> ViewAllCommentsOfPost(string postId)
        {
            if (string.IsNullOrEmpty(postId))
            {
                return BadRequest("Post ID is required");
            }

            try
            {
                // Call the LinkedIn API to view all comments of the post
                var comments = await _linkedInApiClient.ViewAllCommentsOfPost(AccessToken, postId);

                if (comments != null)
                {
                    return Ok(comments);
                }
                else
                {
                    return StatusCode(500, "Failed to retrieve comments from LinkedIn.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("view-post-engagements/{postId}")]
        public async Task<IActionResult> ViewPostEngagements(string postId)
        {
            if (string.IsNullOrEmpty(postId))
            {
                return BadRequest("Post ID is required");
            }
                        
            try
            {
                // Call the LinkedIn API to view post engagements
                var postEngagements = await _linkedInApiClient.ViewPostEngagements(AccessToken, postId);

                if (postEngagements != null)
                {
                    return Ok(postEngagements);
                }
                else
                {
                    return StatusCode(500, "Failed to retrieve post engagements from LinkedIn.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost("repost/{originalPostId}")]
        public async Task<IActionResult> Repost(string originalPostId)
        {
            if (string.IsNullOrEmpty(originalPostId))
            {
                return BadRequest("Original Post ID is required");
            }

            try
            {
                // Call the LinkedIn API to repost the original post
                await _linkedInApiClient.RepostSelectedPost(AccessToken, originalPostId);

                return Ok("Post successfully reposted on LinkedIn!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost("image-post-imageUrl")]
        public async Task<IActionResult> PostImage([FromBody] string imageUrl)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");

                    // Download the image from the URL
                    byte[] imageData = await httpClient.GetByteArrayAsync(imageUrl);

                    // Prepare the request to post the image to LinkedIn
                    var content = new MultipartFormDataContent();
                    content.Add(new ByteArrayContent(imageData), "file", "image.jpg");

                    // Replace the following URL with the LinkedIn API endpoint for posting images
                    var postImageUrl = "https://api.linkedin.com/v2/assets?action=registerUpload";
                    var response = await httpClient.PostAsync(postImageUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Successfully posted the image
                        return Ok("Image posted to LinkedIn successfully.");
                    }
                    else
                    {
                        // Handle error response
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        return BadRequest($"Failed to post image: {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error posting image: {ex.Message}");
            }
        }

        [HttpPost("video-post-videoUrl")]
        public async Task<IActionResult> PostVideo([FromBody] string videoUrl)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");

                    // Download the video from the URL
                    byte[] videoData = await httpClient.GetByteArrayAsync(videoUrl);

                    // Prepare the request to post the video to LinkedIn
                    var content = new MultipartFormDataContent();
                    content.Add(new ByteArrayContent(videoData), "file", "video.mp4");

                    // Replace the following URL with the LinkedIn API endpoint for posting videos
                    var postVideoUrl = "https://api.linkedin.com/v2/assets?action=registerUpload";
                    var response = await httpClient.PostAsync(postVideoUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Successfully posted the video
                        return Ok("Video posted to LinkedIn successfully.");
                    }
                    else
                    {
                        // Handle error response
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        return BadRequest($"Failed to post video: {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error posting video: {ex.Message}");
            }
        }

        public class CommentRequest
        {
            public required string CommentText { get; set; }
        }

        public class LinkedInPostWithImage
        {
            public required Text Text { get; set; }
            public required IFormFile Image { get; set; }
        }

        public class LinkedInPostWithVideo
        {
            public required Text Text { get; set; }
            public required IFormFile Video { get; set; }
        }

        public class LinkedInTextPost
        {
            public required Text PostedText { get; set; }
        }

        public class Text
        {
            public required string PostedText { get; set; }
        }
    }
    
}
