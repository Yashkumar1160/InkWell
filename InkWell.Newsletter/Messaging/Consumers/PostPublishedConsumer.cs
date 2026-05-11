using InkWell.Newsletter.DTOs;
using InkWell.Newsletter.Services.Interfaces;
using InkWell.Shared.Events;
using MassTransit;

namespace InkWell.Newsletter.Messaging.Consumers
{
    public class PostPublishedConsumer : IConsumer<PostPublishedEvent>
    {
        private readonly INewsletterService _newsletterService;

        public PostPublishedConsumer(INewsletterService newsletterService)
        {
            _newsletterService = newsletterService;
        }

        public async Task Consume(ConsumeContext<PostPublishedEvent> context)
        {
            var message = context.Message;
            
            var dto = new NewPostNotificationDTO
            {
                PostId = message.PostId,
                Title = message.Title,
                Slug = message.Slug,
                AuthorId = message.AuthorId
            };

            await _newsletterService.SendPostNotification(dto);
        }
    }
}
