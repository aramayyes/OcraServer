using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using OcraServer.Enums;
using OcraServer.Models.EntityFrameworkModels;

namespace OcraServer.Models.ViewModels
{
    public class RestaurantForAgentViewModel
    {
        [Required]
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

        //
        /* </Address> */
        //


        [Required]
        public int RatedCount { get; set; }

        [Required]
        public double Rating { get; set; }

        public string ServePrice { get; set; }

        [Required]
        public bool IsOpen24 { get; set; }

		public TimeSpan? OpeningTime { get; set; }

		public TimeSpan? ClosingTime { get; set; }

		public TimeSpan? AdditionalOpeningTime { get; set; }

		public TimeSpan? AdditionalClosingTime { get; set; }

        [Required]
        public bool IsMain { get; set; } = true;

        [StringLength(100)]
        public string Brand { get; set; }

        [Required]
        public int TableCount { get; set; }

        public virtual RestaurantFeatures Features { get; set; }

        public virtual ICollection<RestaurantImg> Images { get; set; } = new HashSet<RestaurantImg>();

        public virtual ICollection<RestaurantPanorama> Panoramas { get; set; } = new HashSet<RestaurantPanorama>();

    }
}
