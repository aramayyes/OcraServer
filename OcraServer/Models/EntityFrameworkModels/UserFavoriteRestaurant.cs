using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OcraServer.Models.EntityFrameworkModels
{
    [Table("UserFavoriteRestaurant")]
    public class UserFavoriteRestaurant
    {
        //[Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int ID { get; set; }

        [Key, Column(Order = 0), Required]
        public string UserID { get; set; }

        [Required]
        [Key, Column(Order = 0)]
        public int RestaurantID { get; set; }

        [Required]
        public DateTime AddedDate { get; set; }

        [ForeignKey(nameof(UserID))]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey(nameof(RestaurantID))]
        public virtual Restaurant Restaurant { get; set; }
    }

    public class UserFavoriteRestaurantComparer : IEqualityComparer<UserFavoriteRestaurant>
    {
        public bool Equals(UserFavoriteRestaurant x, UserFavoriteRestaurant y)
        {
            return x.UserID == y.UserID && x.RestaurantID == y.RestaurantID;
        }

        public int GetHashCode(UserFavoriteRestaurant obj)
        {
			int hash = 17;
			hash = hash * 31 + obj.UserID.GetHashCode();
			hash = hash * 31 + obj.RestaurantID.GetHashCode();
			return hash;
        }
    }
}
