using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OcraServer.Models.EntityFrameworkModels
{
    [Table("FeedbackImg")]
    public class FeedbackImg
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int FeedbackID { get; set; }

        [Required]
        [StringLength(445)]
        public string ImgLink { get; set; }

        [JsonIgnore]
        [ForeignKey("FeedbackID")]
        public virtual Feedback Feedback { get; set; }
    }
}
