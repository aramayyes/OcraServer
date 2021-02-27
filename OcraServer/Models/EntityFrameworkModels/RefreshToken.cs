using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OcraServer.Models.EntityFrameworkModels
{
    [Table("RefreshToken")]
    public class RefreshToken
    {
		[Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        public string UserID { get; set; }

		[ForeignKey(nameof(UserID))]
		public virtual ApplicationUser User { get; set; }
    }
}
