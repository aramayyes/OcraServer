using System.ComponentModel.DataAnnotations;

namespace OcraServer.Models.ViewModels
{
    public class CountViewModel
    {
        [Required]
        public int Count { get; set; }
    }
}
