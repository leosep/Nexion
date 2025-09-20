// DatingApp.Web/ViewModels/UserViewModel.cs
using System.Collections.Generic;

namespace DatingApp.Web.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Bio { get; set; }
        public string Interests { get; set; }
        public List<string> ProfilePhotoUrls { get; set; } = new List<string>();
        public Dictionary<string, string> PromptAnswers { get; set; } = new Dictionary<string, string>();
    }
}
