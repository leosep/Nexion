// DatingApp.Core/Models/Match.cs
using Dapper.Contrib.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.Core.Models
{
    [Table("Matches")]
    public class Match
    {
        [Dapper.Contrib.Extensions.Key]
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid User1 ID")]
        public int User1Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid User2 ID")]
        public int User2Id { get; set; }

        [Required]
        public DateTime MatchedAt { get; set; } = DateTime.UtcNow;

        [Write(false)]
        public User User1 { get; set; }

        [Write(false)]
        public User User2 { get; set; }
    }
}