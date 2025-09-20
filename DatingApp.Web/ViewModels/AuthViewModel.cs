// DatingApp.Web/ViewModels/AuthViewModel.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.Web.ViewModels
{
    public class AuthViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
