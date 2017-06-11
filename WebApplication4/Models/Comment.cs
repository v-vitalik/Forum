using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public class Comment
    {
        [Required]
        public int CommentID { get; set; }
        public string Text { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }
        public int PostID { get; set; }
        public DateTime Created { get; set; }
        public string ApplicationUserName { get; set; }
    }
}
