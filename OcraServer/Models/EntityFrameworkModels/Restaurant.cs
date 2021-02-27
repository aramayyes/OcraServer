using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OcraServer.Enums;
using System;

namespace OcraServer.Models.EntityFrameworkModels
{
    [Table("Restaurant")]
    public class Restaurant
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string NameEng { get; set; }
        [Required]
        [StringLength(100)]
        public string NameRus { get; set; }
        [Required]
        [StringLength(100)]
        public string NameArm { get; set; }

        [Required]
        [StringLength(2000)]
        public string DescriptionEng { get; set; }

        [Required]
        [StringLength(2000)]
        public string DescriptionRus { get; set; }

        [Required]
        [StringLength(2000)]
        public string DescriptionArm { get; set; }

        [StringLength(445)]
        public string LogoLink { get; set; }

        [Required]
        [StringLength(445)] 
        public string MainImgLink { get; set; }

        [Required]
        public RestaurantCategory Category { get; set; }

        [Required]
        [StringLength(100)]
        public string CuisineEng { get; set; }

        [Required]
        [StringLength(100)]
        public string CuisineRus { get; set; }

        [Required]
        [StringLength(100)]
        public string CuisineArm { get; set; }

        [Required]
        [StringLength(100)]
        public string Cost { get; set; }

		//
		/* <Address> */
		//

		[Required]
		[StringLength(100)]
		public string CityEng { get; set; }

		[Required]
		[StringLength(100)]
		public string CityRus { get; set; }

		[Required]
		[StringLength(100)]
		public string CityArm { get; set; }

		[Required]
		[StringLength(100)]
		public string AddressEng { get; set; }

		[Required]
		[StringLength(100)]
		public string AddressRus { get; set; }

		[Required]
		[StringLength(100)]
		public string AddressArm { get; set; }

		[Required]
		[StringLength(20)]
		public string PhoneNumber { get; set; }

		[StringLength(20)]
		public string AdditionalPhoneNumber { get; set; }

		[Required]
		public double Latitude { get; set; }

		[Required]
		public double Longitude { get; set; }

		

		[StringLength(50)]
		public string WebSite { get; set; }

		[StringLength(20)]
		public string Email { get; set; }

		[StringLength(80)]
		public string Facebook { get; set; }

        [Required]
        public int RatedCount { get; set; } = 1;

        [Required]
        public double Rating { get; set; } = 5;

        public string ServePrice { get; set; }

        [Required]
        public bool IsOpen24 { get; set; } = false;

		public TimeSpan? OpeningTime { get; set; }

		public TimeSpan? ClosingTime { get; set; }

		public TimeSpan? AdditionalOpeningTime { get; set; }

		public TimeSpan? AdditionalClosingTime { get; set; }

        [Required]
		public string AgentID { get; set; }

        [Required]
        public int FeaturesID { get; set; }

        [Required]
        public bool IsMain { get; set; } = true;

        [StringLength(100)]
        public string Brand { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public int TableCount { get; set; }

		[JsonIgnore]
        [ForeignKey(nameof(AgentID))]
        public virtual ApplicationUser Agent { get; set; }

        [ForeignKey(nameof(FeaturesID))]
        public virtual RestaurantFeatures Features { get; set; }

        public virtual ICollection<Feedback> Feedbacks { get; set; } = new HashSet<Feedback>();
       
        [JsonIgnore]
        public virtual ICollection<UserFavoriteRestaurant> FavoriteUsers { get; set; } = new HashSet<UserFavoriteRestaurant>();

        public virtual ICollection<RestaurantImg> Images { get; set; } = new HashSet<RestaurantImg>();

        public virtual ICollection<RestaurantPanorama> Panoramas { get; set; } = new HashSet<RestaurantPanorama>();

        public virtual ICollection<RestaurantMenuCategory> MenuCategories { get; set; } = new HashSet<RestaurantMenuCategory>();

        [JsonIgnore]
        public virtual ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();

        public virtual ICollection<ExternalReservation> ReservedTables { get; set; } = new HashSet<ExternalReservation>();

        public virtual ICollection<Event> Events { get; set; } = new HashSet<Event>();

        public virtual ICollection<Discount> Discounts { get; set; } = new HashSet<Discount>();
	}
}
