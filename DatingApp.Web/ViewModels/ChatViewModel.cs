using DatingApp.Core.Models;
using System.Collections.Generic;

namespace DatingApp.Web.ViewModels
{
    public class ChatViewModel
    {
        public Match Match { get; set; }
        public User OtherUser { get; set; }
        public IEnumerable<Message> Messages { get; set; }
    }
}