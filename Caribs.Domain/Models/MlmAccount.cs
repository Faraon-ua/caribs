using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Caribs.Domain.DbContext;

namespace Caribs.Domain.Models
{
    public enum MlmAccountType
    {
        CaribbeanBridge
    }
    public class MlmAccount
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        public string LastAwardedOn { get; set; }
        [Required]
        public MlmAccountType MlmAccountType { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
