using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace DatingApp.Core.Models
{
    [Table("Users")]
    public class User
    {
        [Dapper.Contrib.Extensions.Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores")]
        public string Username { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; }

        [Required]
        public byte[] PasswordSalt { get; set; }

        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
        public string Bio { get; set; }

        [StringLength(200, ErrorMessage = "Interests cannot exceed 200 characters")]
        public string Interests { get; set; }

        public string ProfilePhotoUrlsJson { get; set; }
        public string PromptAnswersJson { get; set; }

        public List<string> ProfilePhotoUrls { get; set; }
        public Dictionary<string, string> PromptAnswers { get; set; }

        [RegularExpression("^(Hombre|Mujer|No Binario)$", ErrorMessage = "Género inválido")]
        public string Gender { get; set; }

        [RegularExpression("^(Hombres|Mujeres|Ambos)$", ErrorMessage = "Preferencia de búsqueda inválida")]
        public string LookingFor { get; set; }

        [Range(18, 99, ErrorMessage = "Edad mínima debe estar entre 18 y 99")]
        public int MinAge { get; set; }

        [Range(18, 99, ErrorMessage = "Edad máxima debe estar entre 18 y 99")]
        public int MaxAge { get; set; }

        [StringLength(100, ErrorMessage = "País no puede exceder 100 caracteres")]
        public string Country { get; set; }

        [StringLength(100, ErrorMessage = "Ciudad no puede exceder 100 caracteres")]
        public string City { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}