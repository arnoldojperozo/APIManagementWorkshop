using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NETConfAPI.Models
{
    public class Talk
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        public virtual Speaker Speaker { get; set; }
    }
}