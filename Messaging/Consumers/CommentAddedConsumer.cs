using InkWell.Notification.DTOs;
using InkWell.Notification.Services.Interfaces;
using InkWell.Shared.Events;
using MassTransit;

namespace InkWell.Notification.Messaging.Consumers
{
    public class CommentAddedConsumer : IConsumer<CommentAddedEvent>
    {
        private readonly INotificationService _notificationService;

        public CommentAddedConsumer(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Consume(ConsumeContext<CommentAddedEvent> context)
        {
            var message = context.Message;
            
            var dto = new CommentAddedDTO
            {
                PostId = message.PostId,
                CommentId = message.CommentId,
                CommentAuthorId = message.CommentAuthorId,
                PostAuthorId = message.PostAuthorId,
                ParentCommentId = message.ParentCommentId,
                ParentCommentAuthorId = message.ParentCommentAuthorId,
                NotificationType = message.NotificationType
            };

            await _notificationService.HandleCommentAdded(dto);
        }
    }
}
