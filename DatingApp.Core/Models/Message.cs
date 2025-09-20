// DatingApp.Core/Models/Message.cs
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.Core.Models
{
    [Table("Messages")]
    public class Message
    {
        [Dapper.Contrib.Extensions.Key]
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid sender ID")]
        public int SenderId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid receiver ID")]
        public int ReceiverId { get; set; }

        [Required(ErrorMessage = "Message content is required")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Message must be between 1 and 1000 characters")]
        public string Content { get; set; }

        [Required]
        public DateTime SentAt { get; set; }
    }
}