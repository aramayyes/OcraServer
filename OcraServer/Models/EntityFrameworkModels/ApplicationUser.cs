using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json;

namespace OcraServer.Models.EntityFrameworkModels
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public bool IsActive { get; set; } = true;

        [JsonIgnore]
        [Required]
        public bool HasFilled { get; set; } = false;

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        public string ImgUrl { get; set; } = "https://image.flaticon.com/icons/png/128/149/149452.png";
        public string BackgroundImgUrl { get; set; }

        public string FirebaseUID { get; set; }

        [JsonIgnore]
        public string FirebaseNotificationTokenAndroid { get; set; }

		[JsonIgnore]
		public string FirebaseNotificationTokeniOS { get; set; }

		[JsonIgnore]
		public string FirebaseNotificationTokenWeb { get; set; }

        [Required]
        [StringLength(20)]
        public string City { get; set; } = "Yerevan";

        public bool? Sex { get; set; } = null;

        public DateTime? DateOfBirth { get; set; }

        [Required]
        public int Points { get; set; } = 0;

        [JsonIgnore]
        public int? RestaurantID { get; set; }

        [JsonIgnore]
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }

        [JsonIgnore]
        public virtual ICollection<Feedback> Feedbacks { get; set; } = new HashSet<Feedback>();

        public virtual ICollection<UserFavoriteRestaurant> FavoriteRestaurants { get; set; } = new HashSet<UserFavoriteRestaurant>();

        public virtual ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();
    }
}
