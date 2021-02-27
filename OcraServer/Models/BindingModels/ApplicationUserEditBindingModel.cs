using System;
using System.ComponentModel.DataAnnotations;

namespace OcraServer.Models.BindingModels
{
    public class ApplicationUserEditBindingModel
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        public bool? Sex { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [Required]
        [StringLength(20)]
        public string City { get; set; }
    }
}
