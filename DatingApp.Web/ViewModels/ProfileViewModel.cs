// DatingApp.Web/ViewModels/ProfileViewModel.cs
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.Web.ViewModels
{
    public class ProfileViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        public string Username { get; set; }

        public string Bio { get; set; }

        public string Interests { get; set; }

        public List<IFormFile> ProfilePhotos { get; set; }

        public List<string> ProfilePhotoUrls { get; set; } = new List<string>();

        public Dictionary<string, string> PromptAnswers { get; set; } = new Dictionary<string, string>();

        public string Gender { get; set; }
        public string LookingFor { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        [DataType(DataType.Date)] // Agrega esta anotación para el tipo de control
        public DateTime? DateOfBirth { get; set; } // Agrega esta propiedad

    }
}
