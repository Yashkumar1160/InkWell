using InkWell.Notification.Services.Interfaces;
using InkWell.Shared.Events;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace InkWell.Notification.Messaging.Consumers
{
    public class SendInAppPostNotificationConsumer : IConsumer<SendInAppPostNotificationEvent>
    {
        private readonly INotificationService _notificationService;

        public SendInAppPostNotificationConsumer(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Consume(ConsumeContext<SendInAppPostNotificationEvent> context)
        {
            var message = context.Message;
            Console.WriteLine($"[Notification Service] Received SendInAppPostNotificationEvent for PostId: {message.PostId}, {message.RecipientIds.Count} recipients");

            foreach (var recipientId in message.RecipientIds)
            {
                try
                {
                    // Send targeted in-app notification to the subscriber
                    await _notificationService.Send(
                        recipientId: recipientId,
                        actorId: message.AuthorId,
                        type: "NEW_POST",
                        title: "New post published!",
                        message: $"A new post was published: {message.Title}",
                        relatedId: message.PostId,
                        relatedType: "Post"
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Notification Service] Failed to send targeted in-app notification to recipient {recipientId}: {ex.Message}");
                }
            }

            Console.WriteLine($"[Notification Service] Successfully finished targeted notifications for PostId: {message.PostId}");
        }
    }
}
