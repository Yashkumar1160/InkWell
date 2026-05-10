using InkWell.Newsletter.Models;

namespace InkWell.Newsletter.Repository.Interfaces
{
    public interface ISubscriberRepository
    {
        // Method to find subscriber by email
        Task<Subscriber> GetByEmail(string email);

        // Method to find subscriber by user id
        Task<Subscriber> GetByUserId(int userId);

        // Method to find subscriber by their unique token
        Task<Subscriber> GetByToken(string token);

        // Method to get all subscribers filtered by status
        Task<List<Subscriber>> GetByStatus(string status);

        // Method to get all subscribers
        Task<List<Subscriber>> GetAll();

        // Method to get subscriber by id
        Task<Subscriber> GetById(int id);

        // Method to check if email is already subscribed
        Task<bool> EmailExists(string email);

        // Method to count subscribers by status
        Task<int> CountByStatus(string status);

        // Method to save new subscriber
        Task<Subscriber> Add(Subscriber subscriber);

        // Method to save changes to existing subscriber
        Task<Subscriber> Update(Subscriber subscriber);

        // Method to permanently delete subscriber
        Task DeleteById(int id);
    }
}