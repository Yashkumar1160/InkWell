using System.Collections.Generic;
using System.Threading.Tasks;
using InkWell.Newsletter.DTOs;

namespace InkWell.Newsletter.Services.Interfaces
{
    public interface INewsletterService
    {
        // Method to subscribe with email
        Task<SubscriberResponseDTO> Subscribe(SubscribeDTO dto);

        // Method to confirm subscription via token link in email
        Task ConfirmSubscription(string token);

        // Method to one click unsubscribe via token link in email
        Task Unsubscribe(string token);

        // Method to get subscriber by email
        Task<SubscriberResponseDTO> GetByEmail(string email);

        // Method to get all subscribers (Admin only)
        Task<List<SubscriberResponseDTO>> GetAllSubscribers();

        // Method to get subscribers filtered by status
        Task<List<SubscriberResponseDTO>> GetByStatus(string status);

        // Method to send newsletter campaign to active subscribers
        Task SendNewsletter(SendNewsletterDTO dto);

        // Method to be called by post service when new post published
        // sends new post notification to all active subscribers
        Task SendPostNotification(NewPostNotificationDTO dto);

        // Method to update subscriber preferences
        Task UpdatePreferences(string token, UpdatePreferencesDTO dto);

        // Method to get count of active subscribers
        Task<int> GetSubscriberCount();

        // Method to permanently remove a subscriber (Admin only)
        Task DeleteSubscriber(int subscriberId);

        // Method to update preferences by user id (for logged in users)
        Task UpdatePreferencesByUserId(int userId, UpdatePreferencesDTO dto);

        // Method to unsubscribe by user id and email (for logged in users)
        Task UnsubscribeByUser(int userId, string email);

        // Method to get subscription status by user id
        Task<SubscriberResponseDTO> GetByUserId(int userId);
    }
}