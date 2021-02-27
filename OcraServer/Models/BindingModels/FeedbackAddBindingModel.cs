using System.ComponentModel.DataAnnotations;

namespace OcraServer.Models.BindingModels
{
    public class FeedbackAddBindingModel
    {
        [Required]
        [StringLength(500)]
        public string Text { get; set; }

        [Required]
        public int RestaurantID { get; set; }

        [Range(1, 5)]
		[Required]
		public double Mark { get; set; }
    }
}
