using CvProject.Models;

namespace CvProject.View.Models.ViewModels
{
    public class MessageViewModel
    {
        public List<Message> ReceivedMessages { get; set; } = new();
        public List<Message> SentMessages { get; set; } = new();
    }
}