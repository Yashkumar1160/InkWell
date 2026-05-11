using InkWell.Notification.Services.Interfaces;
using InkWell.Shared.Events;
using MassTransit;

namespace InkWell.Notification.Messaging.Consumers
{
    public class NewsletterPublishedConsumer : IConsumer<NewsletterPublishedEvent>
    {
        private readonly INotificationService _notificationService;

        public NewsletterPublishedConsumer(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Consume(ConsumeContext<NewsletterPublishedEvent> context)
        {
            var message = context.Message;
            
            // For general newsletters, we create a global notification
            await _notificationService.Send(
                recipientId: 0, // Global
                actorId: 0, // System
                type: "NEWSLETTER",
                title: "New Newsletter: " + message.Subject,
                message: "A new newsletter campaign was sent out. Check your email for details!",
                relatedId: 0,
                relatedType: "Newsletter"
            );
        }
    }
}
