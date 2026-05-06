using InkWell.Notification.Models;
using InkWell.Notification.Repository.Interfaces;
using InkWell.Notification.Services.Interfaces;
using InkWell.Shared.Events;
using MassTransit;

namespace InkWell.Notification.Messaging.Consumers
{
    public class PostPublishedConsumer : IConsumer<PostPublishedEvent>
    {
        private readonly INotificationService _notificationService;

        public PostPublishedConsumer(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Consume(ConsumeContext<PostPublishedEvent> context)
        {
            var message = context.Message;
            await _notificationService.HandlePostPublished(message.PostId, message.Title, message.AuthorId);
        }
    }
}
