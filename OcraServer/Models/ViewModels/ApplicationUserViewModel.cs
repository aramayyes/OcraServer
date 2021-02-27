using System;
using System.ComponentModel.DataAnnotations;

namespace OcraServer.Models.ViewModels
{
    public class ApplicationUserViewModel
    {
        [Required]
        public string ID { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string ImgUrl { get; set; }

        public bool? Sex { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public int Points { get; set; }
    }
}
