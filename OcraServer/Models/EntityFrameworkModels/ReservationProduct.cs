using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OcraServer.Models.EntityFrameworkModels
{
    [Table("ReservationProduct")]
    public class ReservationProduct
    {
        [Key, Column(Order = 0), Required]
        public int ReservationID { get; set; }

        [JsonIgnore]
        [Key, Column(Order = 1), Required]
        public int ProductID { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        public int Price { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(ReservationID))]
        public virtual Reservation Reservation { get; set; }

        [Required]
        [ForeignKey(nameof(ProductID))]
        public virtual Product Product { get; set; }
    }
}
