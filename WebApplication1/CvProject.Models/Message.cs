using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CvProject.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        public string Subject { get; set; } = "Inget ämne";
        public string Body { get; set; } = "";
        public DateTime DateSent { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;

        public string? AnonymousName { get; set; } 

        
        public string? SenderId { get; set; }
        public virtual User? Sender { get; set; }

        public string? ReceiverId { get; set; }
        public virtual User? Receiver { get; set; }
        public string? SenderName { get; set; }
    }
}