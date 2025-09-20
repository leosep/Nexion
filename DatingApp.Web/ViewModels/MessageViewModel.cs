// DatingApp.Web/ViewModels/MessageViewModel.cs
using System;

namespace DatingApp.Web.ViewModels
{
    public class MessageViewModel
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}