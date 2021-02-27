using System.ComponentModel.DataAnnotations;

namespace OcraServer.Models.BindingModels
{
    public class FeedbackEditBindingModel
    {
        [Required]
        public int ID { get; set; }

        [Required]
        [StringLength(500)]
        public string Text { get; set; }

        [Range(1,5)]
		[Required]
		public double Mark { get; set; }
    }
}
