// DatingApp.Web/ViewModels/SearchViewModel.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.Web.ViewModels
{
    public class SearchViewModel
    {
        [Display(Name = "Intereses")]
        public string Interests { get; set; }
    }
}
