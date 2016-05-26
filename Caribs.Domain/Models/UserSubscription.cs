using System;
using System.ComponentModel.DataAnnotations;
using Caribs.Domain.DbContext;

namespace Caribs.Domain.Models
{
    public class Tool
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
