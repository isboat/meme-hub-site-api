using Meme.Hub.Site.Models;
using Meme.Hub.Site.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Meme.Hub.Site.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // e.g., /api/posts
    public class PostsController : ControllerBase
    {
        private readonly DataStore _dataStore;

        public PostsController(DataStore dataStore)
        {
            _dataStore = dataStore;
        }

        // GET: api/posts
        [HttpGet]
        public ActionResult<IEnumerable<Post>> GetPosts()
        {
            // Return posts ordered by creation date, newest first
            return Ok(_dataStore.Posts.OrderByDescending(p => p.CreatedAt).ToList());
        }

        // GET: api/posts/{postId}
        [HttpGet("{postId}")]
        public ActionResult<Post> GetPost(string postId)
        {
            var post = _dataStore.Posts.FirstOrDefault(p => p._id == postId);
            if (post == null)
            {
                return NotFound("Post not found.");
            }
            return Ok(post);
        }

        // GET: api/posts/user/{userId}
        [HttpGet("user/{userId}")]
        public ActionResult<IEnumerable<Post>> GetPostsByUser(string userId)
        {
            var posts = _dataStore.Posts.Where(p => p.UserId == userId).OrderByDescending(p => p.CreatedAt).ToList();
            return Ok(posts);
        }

        // POST: api/posts/{userId} - Create a new post
        [HttpPost("{userId}")]
        public ActionResult<Post> CreatePost(string userId, [FromBody] CreatePostDto createPostDto)
        {
            var userExists = _dataStore.Users.Any(u => u._id == userId);
            if (!userExists)
            {
                return NotFound("User not found.");
            }

            var newPost = new Post
            {
                _id = Guid.NewGuid().ToString(),
                UserId = userId,
                Content = createPostDto.Content,
                ImageUrl = createPostDto.ImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            _dataStore.Posts.Add(newPost);
            return CreatedAtAction(nameof(GetPost), new { postId = newPost._id }, newPost);
        }

        // POST: api/posts/{postId}/like/{likerId}
        [HttpPost("{postId}/like/{likerId}")]
        public IActionResult LikePost(string postId, string likerId)
        {
            var post = _dataStore.Posts.FirstOrDefault(p => p._id == postId);
            var userExists = _dataStore.Users.Any(u => u._id == likerId);

            if (post == null || !userExists)
            {
                return NotFound("Post or user not found.");
            }

            if (!post.Likes.Contains(likerId))
            {
                post.Likes.Add(likerId);
            }

            return NoContent();
        }

        // POST: api/posts/{postId}/unlike/{unlikerId}
        [HttpPost("{postId}/unlike/{unlikerId}")]
        public IActionResult UnlikePost(string postId, string unlikerId)
        {
            var post = _dataStore.Posts.FirstOrDefault(p => p._id == postId);
            var userExists = _dataStore.Users.Any(u => u._id == unlikerId);

            if (post == null || !userExists)
            {
                return NotFound("Post or user not found.");
            }

            post.Likes.Remove(unlikerId);

            return NoContent();
        }

        // POST: api/posts/{postId}/comment/{commenterId}
        [HttpPost("{postId}/comment/{commenterId}")]
        public ActionResult<Comment> AddComment(string postId, string commenterId, [FromBody] AddCommentDto addCommentDto)
        {
            var post = _dataStore.Posts.FirstOrDefault(p => p._id == postId);
            var userExists = _dataStore.Users.Any(u => u._id == commenterId);

            if (post == null || !userExists)
            {
                return NotFound("Post or user not found.");
            }
            if (string.IsNullOrEmpty(addCommentDto.Text))
            {
                return BadRequest("Comment text cannot be empty.");
            }

            var newComment = new Comment
            {
                _id = Guid.NewGuid().ToString(),
                UserId = commenterId,
                Text = addCommentDto.Text,
                CreatedAt = DateTime.UtcNow
            };

            post.Comments.Add(newComment);
            return CreatedAtAction(nameof(GetPost), new { postId = post._id }, newComment); // Returns the comment
        }
    }
}
