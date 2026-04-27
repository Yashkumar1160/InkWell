namespace InkWell.Newsletter.DTOs
{
    // data angular sends when sending a newsletter campaign
    public class SendNewsletterDTO
    {
        public string Subject { get; set; }
        public string Body { get; set; }

        // if set only subscribers with this preference get the email
        // if empty all active subscribers get it
        public string PreferenceFilter { get; set; }
    }

}