using MassTransit;
using InkWell.Shared.Events;
using InkWell.Comment.Repository.Interfaces;

namespace InkWell.Comment.Messaging.Consumers
{
    public class PostDeletedConsumer : IConsumer<PostDeletedEvent>
    {
        private readonly ICommentRepository _commentRepository;

        public PostDeletedConsumer(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task Consume(ConsumeContext<PostDeletedEvent> context)
        {
            var postId = context.Message.PostId;
            
            // Delete all comments for this post
            await _commentRepository.DeleteAllForPost(postId);
            
            Console.WriteLine($"[CommentService] Deleted all comments for Post {postId}");
        }
    }
}
