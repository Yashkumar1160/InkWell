using InkWell.Post.DTOs;
using InkWell.Post.Models;
using InkWell.Post.Repository.Interfaces;
using InkWell.Post.Service.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using MassTransit;
using InkWell.Shared.Events;

namespace InkWell.Post.Service.Services
{
    public class PostServiceImpl : IPostService
    {
        // IPostRepository instance
        private IPostRepository postRepository;
        private IDistributedCache cache;
        private IPublishEndpoint publishEndpoint;

        // Constructor Dependency Injection
        public PostServiceImpl(IPostRepository repository, IDistributedCache cacheService, IPublishEndpoint publish)
        {
            postRepository = repository;
            cache = cacheService;
            publishEndpoint = publish;
        }
        // Method to create a post 
        public async Task<PostResponseDTO> CreatePost(int authorId, string authorName, CreatePostDTO dto)
        {
            // Generate slug from title "My First Post" becomes "my-first-post"
            string slug = GenerateSlug(dto.Title);

            // if slug already exists add a number at the end
            bool slugTaken = await postRepository.SlugExists(slug);

            if (slugTaken == true)
            {
                slug = slug + "-" + DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }

            // Calculate read time from word count
            int wordCount = string.IsNullOrEmpty(dto.Content) ? 0 : dto.Content.Split(' ').Length;
            int readTime = wordCount / 200;
            if (readTime < 1)
            {
                readTime = 1;
            }

            // Create new post
            PostModel newPost = new PostModel
            {
                AuthorId = authorId,
                AuthorName = authorName,
                Title = dto.Title,
                Slug = slug,
                Content = dto.Content,
                Excerpt = dto.Excerpt,
                FeaturedImageUrl = dto.FeaturedImageUrl,
                Status = "DRAFT",
                ReadTimeMinutes = readTime,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // add post using postRepository
            PostModel saved = await postRepository.Add(newPost);

            return MapToDTO(saved);
        }

        // Method to get post by id
        public async Task<PostResponseDTO> GetById(int id, int currentUserId = 0)
        {
            // Find post by id using postRepository
            PostModel post = await postRepository.GetById(id);

            // if post is null
            if (post == null)
            {
                throw new Exception("Post not found.");
            }

            // check if user liked it
            bool isLiked = currentUserId > 0 && await postRepository.GetLike(id, currentUserId) != null;
            return MapToDTO(post, isLiked);
        }

        // Method to Get post by slug
        public async Task<PostResponseDTO> GetBySlug(string slug, int currentUserId = 0)
        {
            // Get post by slug using postRepository
            PostModel post = await postRepository.GetBySlug(slug);
            if (post == null)
            {
                throw new Exception("Post not found.");
            }

            // check if user liked it
            bool isLiked = currentUserId > 0 && await postRepository.GetLike(post.PostId, currentUserId) != null;
            return MapToDTO(post, isLiked);
        }

        // Get post by author
        public async Task<List<PostResponseDTO>> GetByAuthor(int authorId)
        {
            // find all posts by author id using postRepository
            List<PostModel> posts = await postRepository.GetByAuthorId(authorId);
            List<PostResponseDTO> result = new List<PostResponseDTO>();

            // loop to add posts to result
            foreach (PostModel post in posts)
            {
                result.Add(MapToDTO(post));
            }
            return result;
        }

        // Method to get post that are published
        public async Task<List<PostResponseDTO>> GetPublished()
        {
            string cacheKey = "published_posts";
            string cachedData = null;
            try { cachedData = await cache.GetStringAsync(cacheKey); } catch { /* Redis down, fallback to DB */ }

            if (!string.IsNullOrEmpty(cachedData))
            {
                try { return JsonSerializer.Deserialize<List<PostResponseDTO>>(cachedData); } catch { /* Corrupt cache, fallback to DB */ }
            }

            // Fetch directly from database if not in cache
            List<PostModel> posts = await postRepository.GetPublished();
            List<PostResponseDTO> result = new List<PostResponseDTO>();

            foreach (PostModel post in posts)
            {
                result.Add(MapToDTO(post));
            }

            // Store in cache for 10 minutes
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };
            try { await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), cacheOptions); } catch { /* Redis down */ }

            return result;
        }

        // Method to search posts by keyword in title
        public async Task<List<PostResponseDTO>> Search(string keyword)
        {
            // Find posts that contains keyword in title using postRepository
            List<PostModel> posts = await postRepository.SearchByTitle(keyword);
            List<PostResponseDTO> result = new List<PostResponseDTO>();

            foreach (PostModel post in posts)
            {
                result.Add(MapToDTO(post));
            }
            return result;
        }

        // Method to get all posts - admin sees all posts
        public async Task<List<PostResponseDTO>> GetAll()
        {
            // Find all posts using postRepository
            List<PostModel> posts = await postRepository.GetAll();
            List<PostResponseDTO> result = new List<PostResponseDTO>();

            foreach (PostModel post in posts)
            {
                result.Add(MapToDTO(post));
            }
            return result;
        }


        // Method to get posts by status
        public async Task<List<PostResponseDTO>> GetByStatus(string status)
        {
            // validate status value
            if (status != "DRAFT" && status != "PUBLISHED"
                && status != "UNPUBLISHED" && status != "ARCHIVED")
            {
                throw new Exception("Status must be DRAFT, PUBLISHED, UNPUBLISHED or ARCHIVED.");
            }

            List<PostModel> posts = await postRepository.GetByStatus(status);
            List<PostResponseDTO> result = new List<PostResponseDTO>();

            foreach (PostModel post in posts)
            {
                result.Add(MapToDTO(post));
            }
            return result;
        }

        // Method to update post (edit title, content, excerpt, image)
        public async Task<PostResponseDTO> UpdatePost(int postId, int authorId, UpdatePostDTO dto)
        {
            // Find post by id using postRepository
            PostModel post = await postRepository.GetById(postId);

            // if post is null
            if (post == null)
            {
                throw new Exception("Post not found.");
            }

            // if authorid mismatched
            if (post.AuthorId != authorId)
            {
                throw new Exception("You can only edit your own posts.");
            }

            // update fields
            post.Title = dto.Title;
            post.Content = dto.Content;
            post.Excerpt = dto.Excerpt;
            post.FeaturedImageUrl = dto.FeaturedImageUrl;
            post.UpdatedAt = DateTime.UtcNow;

            // Calculate read time after content update
            int wordCount = string.IsNullOrEmpty(dto.Content) ? 0 : dto.Content.Split(' ').Length;
            int readTime = wordCount / 200;

            //if read time is less than 1
            if (readTime < 1)
            {
                readTime = 1;
            }

            post.ReadTimeMinutes = readTime;

            // Update post using postRepository
            PostModel updated = await postRepository.Update(post);

            // Invalidate cache since data changed
            try { await cache.RemoveAsync("published_posts"); } catch { /* Redis down */ }

            return MapToDTO(updated);
        }

        // Method to publish post (change status to PUBLISHED)
        public async Task<PostResponseDTO> PublishPost(int postId, int authorId)
        {
            PostModel post = await postRepository.GetById(postId);
            if (post == null)
            {
                throw new Exception("Post not found.");
            }

            if (post.AuthorId != authorId)
            {
                throw new Exception("You can only publish your own posts.");
            }

            // change status to published
            post.Status = "PUBLISHED";
            post.PublishedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;

            PostModel updated = await postRepository.Update(post);

            // Invalidate cache so the new post shows up in the list
            try { await cache.RemoveAsync("published_posts"); } catch { /* Redis down */ }

            // notify subscribers about the new post
            await NotifyNewsletterService(updated);

            return MapToDTO(updated);
        }

        // Method to unpublish post (hide post from public)
        public async Task<PostResponseDTO> UnpublishPost(int postId, int authorId)
        {
            // Find post by id 
            PostModel post = await postRepository.GetById(postId);

            if (post == null)
            {
                throw new Exception("Post not found.");
            }

            if (post.AuthorId != authorId)
            {
                throw new Exception("You can only unpublish your own posts.");
            }

            // update status
            post.Status = "UNPUBLISHED";
            post.UpdatedAt = DateTime.UtcNow;

            PostModel updated = await postRepository.Update(post);
            try { await cache.RemoveAsync("published_posts"); } catch { }
            return MapToDTO(updated);
        }


        // Method to ARCHIVE post
        public async Task<PostResponseDTO> ArchivePost(int postId, int authorId)
        {
            PostModel post = await postRepository.GetById(postId);

            if (post == null)
            {
                throw new Exception("Post not found.");
            }

            if (post.AuthorId != authorId)
            {
                throw new Exception("You can only archive your own posts.");
            }

            post.Status = "ARCHIVED";
            post.UpdatedAt = DateTime.UtcNow;

            PostModel updated = await postRepository.Update(post);
            try { await cache.RemoveAsync("published_posts"); } catch { }
            return MapToDTO(updated);
        }

        // Method to UNARCHIVE post (restore to DRAFT)
        public async Task<PostResponseDTO> UnarchivePost(int postId, int authorId)
        {
            PostModel post = await postRepository.GetById(postId);

            if (post == null)
            {
                throw new Exception("Post not found.");
            }

            if (post.AuthorId != authorId)
            {
                throw new Exception("You can only restore your own posts.");
            }

            // Restore to DRAFT so author can edit/review before publishing again
            post.Status = "DRAFT";
            post.UpdatedAt = DateTime.UtcNow;

            PostModel updated = await postRepository.Update(post);
            return MapToDTO(updated);
        }

        // Method to delete post
        public async Task DeletePost(int postId, int authorId, string callerRole)
        {
            // Find post by id
            PostModel post = await postRepository.GetById(postId);
            if (post == null)
            {
                throw new Exception("Post not found.");
            }

            // author can delete own post
            // admin can delete any post (checked in controller)
            if (callerRole != "ADMIN" && post.AuthorId != authorId)
            {
                throw new Exception("You can only delete your own posts.");
            }

            await postRepository.Delete(postId);
            
            // Notify other services (Comments, Category) to clean up
            await publishEndpoint.Publish(new PostDeletedEvent { PostId = postId });

            // Invalidate cache
            try { await cache.RemoveAsync("published_posts"); } catch { /* Redis down */ }
        }

        // Method to increase views count
        public async Task IncrementViews(int postId)
        {
            // Find post by id
            PostModel post = await postRepository.GetById(postId);
            if (post == null)
            {
                throw new Exception("Post not found.");
            }

            // increase post count
            post.ViewCount = post.ViewCount + 1;

            // update post
            await postRepository.Update(post);

            // Invalidate home page cache
            try { await cache.RemoveAsync("published_posts"); } catch { /* Redis down */ }
        }

        // Method to increase like count 
        public async Task LikePost(int postId, int actorId, string actorName)
        {
            PostModel post = await postRepository.GetById(postId);
            if (post == null)
            {
                throw new Exception("Post not found.");
            }

            // check if user has already liked this post
            var existingLike = await postRepository.GetLike(postId, actorId);
            if (existingLike != null)
            {
                // user already liked, don't add another one
                return;
            }

            // increment total likes on post
            post.LikesCount = post.LikesCount + 1;
            await postRepository.Update(post);

            // save the individual like to database
            await postRepository.AddLike(new LikeModel { PostId = postId, UserId = actorId });

            // Invalidate home page cache so counts are accurate
            try { await cache.RemoveAsync("published_posts"); } catch { /* Redis down */ }

            // notify post author that someone liked their post
            await NotifyNotificationService(postId, post.AuthorId, actorId, actorName);
        }

        // Method to notify Notification Service via RabbitMQ
        private async Task NotifyNotificationService(int postId, int postAuthorId, int actorId, string actorName)
        {
            try
            {
                // Publish event to RabbitMQ
                await publishEndpoint.Publish(new PostLikedEvent
                {
                    PostId = postId,
                    PostAuthorId = postAuthorId,
                    ActorId = actorId,
                    ActorName = actorName
                });
            }
            catch (Exception ex)
            {
                // log error but don't fail the request
                Console.WriteLine($"RabbitMQ Error: {ex.Message}");
            }
        }

        // Method to unlike a post
        public async Task UnlikePost(int postId, int actorId)
        {
            // Find post by id
            PostModel post = await postRepository.GetById(postId);
            if (post == null)
            {
                throw new Exception("Post not found.");
            }

            // Check if user actually liked this post first
            var existingLike = await postRepository.GetLike(postId, actorId);
            if (existingLike == null)
            {
                // user didn't like this post, so can't unlike
                return;
            }

            // check if likes are below zero
            if (post.LikesCount > 0)
            {
                // decrease likes count
                post.LikesCount = post.LikesCount - 1;
                await postRepository.Update(post);

                // remove the like record from database
                await postRepository.RemoveLike(postId, actorId);

                // Invalidate home page cache so counts are accurate
                try { await cache.RemoveAsync("published_posts"); } catch { /* Redis down */ }
            }
        }


        // Method to find how many posts an author has written
        public async Task<int> GetPostCount(int authorId)
        {
            int count = await postRepository.CountByAuthorId(authorId);
            return count;
        }


        // Method for admin to pin post to top of feed
        public async Task<PostResponseDTO> FeaturePost(int postId)
        {
            PostModel post = await postRepository.GetById(postId);
            if (post == null)
            {
                throw new Exception("Post not found.");
            }

            post.IsFeatured = true;
            post.UpdatedAt = DateTime.UtcNow;

            PostModel updated = await postRepository.Update(post);
            return MapToDTO(updated);
        }

        // Method for admin to remove pin from post
        public async Task<PostResponseDTO> UnfeaturePost(int postId)
        {
            PostModel post = await postRepository.GetById(postId);
            if (post == null)
            {
                throw new Exception("Post not found.");
            }

            post.IsFeatured = false;
            post.UpdatedAt = DateTime.UtcNow;

            PostModel updated = await postRepository.Update(post);
            return MapToDTO(updated);
        }



        // Method to notify Newsletter Service via RabbitMQ
        private async Task NotifyNewsletterService(PostModel post)
        {
            try
            {
                Console.WriteLine($"[Post Service] Attempting to publish PostPublishedEvent for PostId: {post.PostId}");
                // Publish event to RabbitMQ
                await publishEndpoint.Publish(new PostPublishedEvent
                {
                    PostId = post.PostId,
                    Title = post.Title,
                    Slug = post.Slug,
                    AuthorId = post.AuthorId
                });
                Console.WriteLine($"[Post Service] PostPublishedEvent published successfully for PostId: {post.PostId}");
            }
            catch (Exception ex)
            {
                // log error but don't fail the request
                Console.WriteLine($"[Post Service] RabbitMQ Error: {ex.Message}");
            }
        }

        // Method to generates url safe slug from title
        private string GenerateSlug(string title)
        {
            // convert to lowercase
            string slug = title.ToLower();

            // replace spaces with hyphens
            slug = slug.Replace(" ", "-");

            // remove special characters
            string cleanSlug = "";

            foreach (char c in slug)
            {
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '-')
                {
                    cleanSlug = cleanSlug + c;
                }
            }

            return cleanSlug;
        }

        // Method to convert Post model to PostResponseDTO
        private PostResponseDTO MapToDTO(PostModel post, bool isLiked = false)
        {
            PostResponseDTO dto = new PostResponseDTO
            {
                PostId = post.PostId,
                AuthorId = post.AuthorId,
                AuthorName = post.AuthorName,
                Title = post.Title,
                Slug = post.Slug,
                Content = post.Content,
                Excerpt = post.Excerpt,
                FeaturedImageUrl = post.FeaturedImageUrl,
                Status = post.Status,
                ReadTimeMinutes = post.ReadTimeMinutes,
                ViewCount = post.ViewCount,
                LikesCount = post.LikesCount,
                IsFeatured = post.IsFeatured,
                IsLiked = isLiked,
                CreatedAt = post.CreatedAt,
                PublishedAt = post.PublishedAt
            };
            return dto;
        }

    }
}