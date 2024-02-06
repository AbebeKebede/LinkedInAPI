using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LinkedInAPI
{
    public class LinkedInApiClient
    {
        private readonly HttpClient _httpClient;

        public LinkedInApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task PostTextUpdate(string apiUrl, string accessToken, string text)
        {

            // Construct the post payload (text-only)
            string postPayload = $"{{ \"text\": {{ \"text\": \"{text}\" }} }}";

            // Add the access token to the Authorization header
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            // Create the content for the POST request
            StringContent content = new StringContent(postPayload, Encoding.UTF8, "application/json");

            // Make the POST request
            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);

            // Check if the request was successful (status code 201)
            response.EnsureSuccessStatusCode();
        }

        public async Task AddPostWithImage(string apiUrl,string accessToken, string text, IFormFile image)
        {
           
            // Convert the image to a byte array
            byte[] imageData;
            using (MemoryStream ms = new MemoryStream())
            {
                image.CopyTo(ms);
                imageData = ms.ToArray();
            }

            // Construct the post payload (text and image)
            string postPayload = $"{{ \"text\": {{ \"text\": \"{text}\" }}, \"content\": {{ \"contentEntities\": [{{ \"thumbnails\": [{{ \"resolvedUrl\": \"cid:image\" }}] }}], \"title\": \"Sample Share\" }}, \"distribution\": {{ \"linkedInDistributionTarget\": {{ \"visibleToGuest\": true }} }} }}";

            // Add the access token to the Authorization header
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            // Create the content for the POST request
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(postPayload, Encoding.UTF8, "application/json"), "json");
            content.Add(new ByteArrayContent(imageData), "file", "image.jpg");

            // Make the POST request
            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);

            // Check if the request was successful (status code 201)
            response.EnsureSuccessStatusCode();
        }

        public async Task AddPostWithVideo(string apiUrl, string accessToken, string text, IFormFile video)
        {           

            // Convert the video to a byte array
            byte[] videoData;
            using (MemoryStream ms = new MemoryStream())
            {
                video.CopyTo(ms);
                videoData = ms.ToArray();
            }

            // Construct the post payload (text and video)
            string postPayload = $"{{ \"text\": {{ \"text\": \"{text}\" }}, \"content\": {{ \"contentEntities\": [{{ \"thumbnails\": [{{ \"resolvedUrl\": \"cid:image\" }}] }}], \"title\": \"Sample Share\" }}, \"distribution\": {{ \"linkedInDistributionTarget\": {{ \"visibleToGuest\": true }} }} }}";

            // Add the access token to the Authorization header
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            // Create the content for the POST request
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(postPayload, Encoding.UTF8, "application/json"), "json");
            content.Add(new ByteArrayContent(videoData), "file", "video.mp4");

            // Make the POST request
            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);

            // Check if the request was successful (status code 201)
            response.EnsureSuccessStatusCode();
        }

        public async Task DeletePost(string accessToken, string postId)
        {
            string apiUrl = $"https://api.linkedin.com/v2/shares/{postId}";

            // Add the access token to the Authorization header
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            // Make the DELETE request
            HttpResponseMessage response = await _httpClient.DeleteAsync(apiUrl);

            // Check if the request was successful (status code 204)
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                Console.WriteLine("Post successfully deleted on LinkedIn!");
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }

        public async Task AddCommentToPost(string accessToken, string postId, string commentText)
        {
            try
            {
                // Construct the URL for adding a comment to the post
                string apiUrl = $"https://api.linkedin.com/v2/socialActions/{postId}/comments";

                // Add the access token to the Authorization header
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                // Construct the comment payload
                string commentPayload = $"{{ \"text\": \"{commentText}\" }}";

                // Create the content for the POST request
                StringContent content = new StringContent(commentPayload, Encoding.UTF8, "application/json");

                // Make the POST request to add a comment
                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);

                // Check if the request was successful (status code 201)
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Comment successfully added to the post on LinkedIn!");
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                // Handle the exception as needed
            }
        }

        public async Task<List<Comment>> ViewAllCommentsOfPost(string accessToken, string postId)
        {
            try
            {
                // Construct the URL for retrieving comments of the post
                string apiUrl = $"https://api.linkedin.com/v2/socialActions/{postId}/comments";

                // Add the access token to the Authorization header
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                // Make the GET request to retrieve comments
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                // Check if the request was successful (status code 200)
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response content to a list of comments
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var comments = JsonConvert.DeserializeObject<List<Comment>>(responseBody);

                    Console.WriteLine("Comments successfully retrieved from the post on LinkedIn!");
                    return comments;
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                // Handle the exception as needed
                return null;
            }
        }

        public async Task<PostEngagements> ViewPostEngagements(string accessToken, string postId)
        {
            try
            {
                // Construct the URL for retrieving post engagements
                string apiUrl = $"https://api.linkedin.com/v2/socialActions/{postId}";

                // Add the access token to the Authorization header
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                // Make the GET request to retrieve post engagements
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                // Check if the request was successful (status code 200)
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response content to post engagements
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var postEngagements = JsonConvert.DeserializeObject<PostEngagements>(responseBody);

                    Console.WriteLine("Post engagements successfully retrieved from LinkedIn!");
                    return postEngagements;
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                // Handle the exception as needed
                return null;
            }
        }

        public async Task<string> GetPostContent(string accessToken, string postId)
        {
            string apiUrl = $"https://api.linkedin.com/v2/shares/{postId}";

            // Add the access token to the Authorization header
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            // Make the GET request
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            // Check if the request was successful (status code 200)
            if (response.IsSuccessStatusCode)
            {
                // Read and return the content of the post
                return await response.Content.ReadAsStringAsync();
            }

            // Handle errors
            throw new Exception($"Error: {response.StatusCode} - {response.ReasonPhrase}");
        }

        public async Task RepostSelectedPost(string accessToken, string originalPostId)
        {
            try
            {
                // Get the content of the original post
                string originalPostContent = await GetPostContent(accessToken, originalPostId);

                // Create a new post using the content of the original post
                await CreatePost(accessToken, originalPostContent);

                Console.WriteLine("Post successfully reposted on LinkedIn!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                // Handle the exception as needed
            }
        }

        private async Task CreatePost(string accessToken, string postContent)
        {
            try
            {
                // Construct the URL for creating a post
                string apiUrl = "https://api.linkedin.com/v2/shares";

                // Add the access token to the Authorization header
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                // Construct the payload for creating a post
                string postPayload = $"{{ \"comment\": \"{postContent}\" }}";

                // Create the content for the POST request
                StringContent content = new StringContent(postPayload, Encoding.UTF8, "application/json");

                // Make the POST request to create a post
                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);

                // Check if the request was successful (status code 201)
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Post successfully created on LinkedIn!");
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                // Handle the exception as needed
            }
        }

    }



    public class PostEngagements
    {
        [JsonProperty("comments")]
        public int Comments { get; set; }

        [JsonProperty("likes")]
        public int Likes { get; set; }

        [JsonProperty("reshares")]
        public int Reshares { get; set; }

        [JsonProperty("impressionCount")]
        public int ImpressionCount { get; set; }
    }

    public class Comment
    {
        [JsonProperty("actor")]
        public Actor Actor { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }
    }

    public class Actor
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

}
