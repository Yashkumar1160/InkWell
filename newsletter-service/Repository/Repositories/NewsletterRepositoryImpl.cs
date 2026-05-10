using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InkWell.Newsletter.Context;
using InkWell.Newsletter.Models;
using InkWell.Newsletter.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InkWell.Newsletter.Repository.Repositories
{
    public class SubscriberRepositoryImpl : ISubscriberRepository
    {
        // NewsletterDbContext instance
        private NewsletterDbContext dbContext;

        // Constructor Dependency Injection
        public SubscriberRepositoryImpl(NewsletterDbContext context)
        {
            dbContext = context;
        }

        // Method to find by email
        public async Task<Subscriber> GetByEmail(string email)
        {
            Subscriber subscriber = await dbContext.Subscribers
                .FirstOrDefaultAsync(s => s.Email == email);

            return subscriber;
        }

        // Method to find by user id
        public async Task<Subscriber> GetByUserId(int userId)
        {
            Subscriber subscriber = await dbContext.Subscribers
                .FirstOrDefaultAsync(s => s.UserId == userId);

            return subscriber;
        }

        // Method to find by token 
        public async Task<Subscriber> GetByToken(string token)
        {
            Subscriber subscriber = await dbContext.Subscribers
                .FirstOrDefaultAsync(s => s.Token == token);

            return subscriber;
        }

        // Method to get all subscribers with a specific status
        public async Task<List<Subscriber>> GetByStatus(string status)
        {
            List<Subscriber> subscribers = await dbContext.Subscribers
                .Where(s => s.Status == status)
                .OrderByDescending(s => s.SubscribedAt)
                .ToListAsync();

            return subscribers;
        }

        // Method to get all subscribers ordered newest first
        public async Task<List<Subscriber>> GetAll()
        {
            List<Subscriber> subscribers = await dbContext.Subscribers
                .OrderByDescending(s => s.SubscribedAt)
                .ToListAsync();

            return subscribers;
        }

        // Method to get subscriber by id 
        public async Task<Subscriber> GetById(int id)
        {
            Subscriber subscriber = await dbContext.Subscribers.FindAsync(id);
            return subscriber;
        }

        // Method to check if email already exists
        public async Task<bool> EmailExists(string email)
        {
            bool exists = await dbContext.Subscribers.AnyAsync(s => s.Email == email);

            return exists;
        }

        // Method to count subscribers with a specific status
        public async Task<int> CountByStatus(string status)
        {
            int count = await dbContext.Subscribers.CountAsync(s => s.Status == status);

            return count;
        }

        // Method to save new subscriber
        public async Task<Subscriber> Add(Subscriber subscriber)
        {
            dbContext.Subscribers.Add(subscriber);
            await dbContext.SaveChangesAsync();

            return subscriber;
        }

        // Method to save changes
        public async Task<Subscriber> Update(Subscriber subscriber)
        {
            dbContext.Subscribers.Update(subscriber);
            await dbContext.SaveChangesAsync();

            return subscriber;
        }

        // Method to permanently delete
        public async Task DeleteById(int id)
        {
            Subscriber subscriber = await dbContext.Subscribers.FindAsync(id);

            if (subscriber != null)
            {
                dbContext.Subscribers.Remove(subscriber);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}