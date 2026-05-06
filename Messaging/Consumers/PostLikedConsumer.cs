using InkWell.Notification.Services.Interfaces;
using InkWell.Shared.Events;
using MassTransit;

namespace InkWell.Notification.Messaging.Consumers
{
    public class PostLikedConsumer : IConsumer<PostLikedEvent>
    {
        private readonly INotificationService _notificationService;

        public PostLikedConsumer(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Consume(ConsumeContext<PostLikedEvent> context)
        {
            var message = context.Message;
            await _notificationService.HandlePostLikedPersonalized(message.PostId, message.PostAuthorId, message.ActorId, message.ActorName);
        }
    }
}
