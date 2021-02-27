using System.ComponentModel.DataAnnotations;

namespace OcraServer.Models.ViewModels
{
    public class ErrorResponse
    {
        [Required]
        public string Message { get; set; }
    }
}
