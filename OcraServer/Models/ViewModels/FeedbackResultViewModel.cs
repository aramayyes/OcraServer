using System;
using System.ComponentModel.DataAnnotations;

namespace OcraServer.Models.ViewModels
{
    public class FeedbackResultViewModel
    {
		[Required]
		public int ID { get; set; }

		[Required]
		public string UserID { get; set; }

		[Required]
		[StringLength(500)]
		public string Text { get; set; }

		[Required]
		public double Mark { get; set; }

		[Required]
		public DateTime Created { get; set; }

		[Required]
		public bool HasBeenEdited { get; set; }
    }
}
