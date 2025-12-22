using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using WebApplication1.Models;

namespace ClassLibrary
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Subject { get; set; }

        [Required]
        [StringLength(2000)]
        public string Body { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        public int SenderId { get; set; }
        [ForeignKey(nameof(SenderId))]
        public virtual User Sender { get; set; }

        public int ReceiverId { get; set; }

        [ForeignKey(nameof(ReceiverId))]
        public virtual User Receiver { get; set; }
    }
}
