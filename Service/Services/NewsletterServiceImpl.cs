using InkWell.Newsletter.DTOs;
using InkWell.Newsletter.Models;
using InkWell.Newsletter.Repository.Interfaces;
using InkWell.Newsletter.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace InkWell.Newsletter.Services.Services
{
    public class NewsletterServiceImpl : INewsletterService
    {
        // ISubscriberRepository instance
        private ISubscriberRepository subscriberRepository;

        // IConfiguration instance
        private IConfiguration configuration;

        // Constructor Dependency Injection
        public NewsletterServiceImpl(ISubscriberRepository repository, IConfiguration config)
        {
            subscriberRepository = repository;
            configuration = config;
        }

        // Method to subscribe with email
        public async Task<SubscriberResponseDTO> Subscribe(SubscribeDTO dto)
        {
            // check if email is already subscribed
            bool emailExists = await subscriberRepository.EmailExists(dto.Email);
            if (emailExists == true)
            {
                // if they previously unsubscribed let them resubscribe
                Subscriber existing = await subscriberRepository.GetByEmail(dto.Email);

                if (existing.Status == "UNSUBSCRIBED")
                {
                    // reset their status and give them a new token
                    existing.Status = "PENDING";
                    existing.Token = Guid.NewGuid().ToString();

                    // reset expiry clock
                    existing.TokenCreatedAt = DateTime.UtcNow;
                    existing.SubscribedAt = DateTime.UtcNow;
                    existing.UnsubscribedAt = null;

                    await subscriberRepository.Update(existing);
                    await SendConfirmationEmail(existing);
                  
                    return MapToDTO(existing);
                }

                throw new Exception("This email is already subscribed.");
            }

            // create new subscriber with PENDING status
            Subscriber newSubscriber = new Subscriber();
            newSubscriber.Email = dto.Email;
            newSubscriber.FullName = dto.FullName;
            newSubscriber.UserId = dto.UserId;
            newSubscriber.Status = "PENDING";
            newSubscriber.SubscribedAt = DateTime.UtcNow;

            Subscriber saved = await subscriberRepository.Add(newSubscriber);

            // send confirmation email immediately after saving
            await SendConfirmationEmail(saved);

            return MapToDTO(saved);
        }

        // Method to confirm subscription via token link in email
        public async Task ConfirmSubscription(string token)
        {
            Subscriber subscriber = await subscriberRepository.GetByToken(token);

            if (subscriber == null)
            {
                throw new Exception("Invalid confirmation link.");
            }

            // if already subscribed
            if (subscriber.Status == "ACTIVE")
            {
                throw new Exception("This subscription is already confirmed.");
            }

            // if not subscribed
            if (subscriber.Status == "UNSUBSCRIBED")
            {
                throw new Exception("This subscription has been cancelled.");
            }

            // check if token has expired after 24 hours
            double hoursElapsed = (DateTime.UtcNow - subscriber.TokenCreatedAt).TotalHours;
            if (hoursElapsed > 24)
            {
                throw new Exception("Confirmation link has expired. Please subscribe again.");
            }

            // update subscription status
            subscriber.Status = "ACTIVE";
            await subscriberRepository.Update(subscriber);

            await SendWelcomeEmail(subscriber);
        }

        // Method to one click unsubscribe via token link in email
        public async Task Unsubscribe(string token)
        {
            Subscriber subscriber = await subscriberRepository.GetByToken(token);

            if (subscriber == null)
            {
                throw new Exception("Invalid unsubscribe link.");
            }

            if (subscriber.Status == "UNSUBSCRIBED")
            {
                throw new Exception("Already unsubscribed.");
            }

            subscriber.Status = "UNSUBSCRIBED";
            subscriber.UnsubscribedAt = DateTime.UtcNow;
            
            await subscriberRepository.Update(subscriber);
        }

        // Method to get subscriber by email
        public async Task<SubscriberResponseDTO> GetByEmail(string email)
        {
            Subscriber subscriber = await subscriberRepository.GetByEmail(email);
            if (subscriber == null)
            {
                throw new Exception("Subscriber not found.");
            }
            return MapToDTO(subscriber);
        }

        // Method to get all subscribers (Admin only)
        public async Task<List<SubscriberResponseDTO>> GetAllSubscribers()
        {
            List<Subscriber> subscribers = await subscriberRepository.GetAll();
            List<SubscriberResponseDTO> result = new List<SubscriberResponseDTO>();

            foreach (Subscriber subscriber in subscribers)
            {
                result.Add(MapToDTO(subscriber));
            }
            
            return result;
        }
        
        // Method to get subscribers filtered by status
        public async Task<List<SubscriberResponseDTO>> GetByStatus(string status)
        {
            if (status != "PENDING" && status != "ACTIVE" && status != "UNSUBSCRIBED")
            {
                throw new Exception("Status must be PENDING, ACTIVE or UNSUBSCRIBED.");
            }

            List<Subscriber> subscribers = await subscriberRepository.GetByStatus(status);
            List<SubscriberResponseDTO> result = new List<SubscriberResponseDTO>();

            foreach (Subscriber subscriber in subscribers)
            {
                result.Add(MapToDTO(subscriber));
            }
           
            return result;
        }

        // Method to send newsletter campaign to active subscribers
        public async Task SendNewsletter(SendNewsletterDTO dto)
        {
            // get all active subscribers
            List<Subscriber> subscribers = await subscriberRepository.GetByStatus("ACTIVE");

            // if preference filter is set only send to matching subscribers
            List<Subscriber> targets = new List<Subscriber>();

            foreach (Subscriber subscriber in subscribers)
            {
                if (string.IsNullOrEmpty(dto.PreferenceFilter))
                {
                    // no filter - send to everyone active
                    targets.Add(subscriber);
                }
                else
                {
                    // only send to subscribers who have this preference
                    if (subscriber.Preferences.Contains(dto.PreferenceFilter))
                    {
                        targets.Add(subscriber);
                    }
                }
            }

            // send email to each target subscriber
            foreach (Subscriber subscriber in targets)
            {
                // build email body with unsubscribe link at the bottom
                // unsubscribe link uses their unique token so no login needed
                string appBaseUrl = configuration["App:BaseUrl"];
                string unsubscribeLink = appBaseUrl + "/api/newsletter/unsubscribe/" + subscriber.Token;

                string fullBody = dto.Body
                    + "<br><br><hr>"
                    + "<p style='font-size:12px;color:gray;'>"
                    + "You are receiving this because you subscribed to InkWell. "
                    + "<a href='" + unsubscribeLink + "'>Unsubscribe</a>"
                    + "</p>";

                await SendEmail(subscriber.Email, subscriber.FullName, dto.Subject, fullBody);
            }
        }

        // Method to be called by post service when new post published
        // sends new post notification to all active subscribers
        public async Task SendPostNotification(NewPostNotificationDTO dto)
        {
            List<Subscriber> subscribers = await subscriberRepository.GetByStatus("ACTIVE");

            string appBaseUrl = configuration["App:BaseUrl"];

            foreach (Subscriber subscriber in subscribers)
            {
                string subject = "New Post on InkWell: " + dto.Title;

                string unsubscribeLink = appBaseUrl + "/api/newsletter/unsubscribe/" + subscriber.Token;

                string body = "<h2>" + dto.Title + "</h2>"
                    + "<p>A new post has been published on InkWell.</p>"
                    + "<a href='http://localhost:4200/blog/" + dto.Slug + "'>Read the post</a>"
                    + "<br><br><hr>"
                    + "<p style='font-size:12px;color:gray;'>"
                    + "<a href='" + unsubscribeLink + "'>Unsubscribe</a>"
                    + "</p>";

                await SendEmail(subscriber.Email, subscriber.FullName, subject, body);
            }
        }

        // Method to update subscriber preferences
        public async Task UpdatePreferences(string token, UpdatePreferencesDTO dto)
        {
            Subscriber subscriber = await subscriberRepository.GetByToken(token);

            if (subscriber == null)
            {
                throw new Exception("Invalid token.");
            }

            subscriber.Preferences = dto.Preferences;
            await subscriberRepository.Update(subscriber);
        }

        // Method to get count of active subscribers
        public async Task<int> GetSubscriberCount()
        {
            int count = await subscriberRepository.CountByStatus("ACTIVE");
            return count;
        }


        // Method to delete subscriber permanently
        public async Task DeleteSubscriber(int subscriberId)
        {
            Subscriber subscriber = await subscriberRepository.GetById(subscriberId);

            if (subscriber == null)
            {
                throw new Exception("Subscriber not found.");
            }

            await subscriberRepository.DeleteById(subscriberId);
        }

        // Method to update preferences by user id (for logged in users)
        public async Task UpdatePreferencesByUserId(int userId, UpdatePreferencesDTO dto)
        {
            Subscriber subscriber = await subscriberRepository.GetByUserId(userId);

            if (subscriber == null)
            {
                throw new Exception("No subscription found for this user.");
            }

            subscriber.Preferences = dto.Preferences;
            await subscriberRepository.Update(subscriber);
        }


        // Method to send confirmation email with token link
        private async Task SendConfirmationEmail(Subscriber subscriber)
        {
            string appBaseUrl = configuration["App:BaseUrl"];
            string confirmLink = appBaseUrl + "/api/newsletter/confirm/" + subscriber.Token;

            string subject = "Confirm your InkWell subscription";
            string body = "<h2>Welcome to InkWell!</h2>"
                + "<p>Please confirm your subscription by clicking the link below.</p>"
                + "<a href='" + confirmLink + "'>Confirm Subscription</a>"
                + "<p>If you did not subscribe just ignore this email.</p>";

            await SendEmail(subscriber.Email, subscriber.FullName, subject, body);
        }

        //Method to send welcome email after confirmation
        private async Task SendWelcomeEmail(Subscriber subscriber)
        {
            string subject = "Welcome to InkWell!";
            string body = "<h2>You are now subscribed!</h2>"
                + "<p>Thanks for confirming your subscription to InkWell.</p>"
                + "<p>You will receive emails when new posts are published.</p>"
                + "<a href='http://localhost:4200'>Visit InkWell</a>";

            await SendEmail(subscriber.Email, subscriber.FullName, subject, body);
        }

        // Method to send actual email using MailKit
        private async Task SendEmail(string toEmail, string toName, string subject, string body)
        {
            try
            {
                // create email message
                MimeMessage message = new MimeMessage();

                // set from address using config
                message.From.Add(new MailboxAddress(
                    configuration["Email:FromName"],
                    configuration["Email:Username"]
                ));

                // set to address
                message.To.Add(new MailboxAddress(toName, toEmail));

                message.Subject = subject;

                // set body as HTML
                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = body;
                message.Body = bodyBuilder.ToMessageBody();

                // connect to smtp server and send
                using (SmtpClient client = new SmtpClient())
                {
                    await client.ConnectAsync(
                        configuration["Email:Host"],
                        int.Parse(configuration["Email:Port"]),
                        SecureSocketOptions.StartTls
                    );

                    await client.AuthenticateAsync(
                        configuration["Email:Username"],
                        configuration["Email:Password"]
                    );

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception)
            {
                // swallow email errors silently
                // subscription should still work even if email fails
                // you can add logging here later
            }
        }

        // Method to convert Subscriber model to SubscriberResponseDTO
        private SubscriberResponseDTO MapToDTO(Subscriber subscriber)
        {
            SubscriberResponseDTO dto = new SubscriberResponseDTO();
            dto.SubscriberId = subscriber.SubscriberId;
            dto.Email = subscriber.Email;
            dto.FullName = subscriber.FullName;
            dto.UserId = subscriber.UserId;
            dto.Status = subscriber.Status;
            dto.SubscribedAt = subscriber.SubscribedAt;
            dto.UnsubscribedAt = subscriber.UnsubscribedAt;
            dto.Preferences = subscriber.Preferences;
       
            return dto;
        }
    }
}