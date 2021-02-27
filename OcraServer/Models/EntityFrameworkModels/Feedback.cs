using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OcraServer.Models.EntityFrameworkModels
{
    [Table("Feedback")]
    public class Feedback
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(500)]
        public string Text { get; set; }

		[Required]
		public double Mark { get; set; }

        [Required]
        public string UserID { get; set; }

        [Required]
        public int RestaurantID { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public bool HasBeenEdited { get; set; } = false;

        [JsonIgnore]
        [ForeignKey(nameof(UserID))]
        public virtual ApplicationUser User { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(RestaurantID))]
        public virtual Restaurant Restaurant { get; set; }

        public virtual ICollection<FeedbackImg> Images { get; set; } = new HashSet<FeedbackImg>();
    }
}
