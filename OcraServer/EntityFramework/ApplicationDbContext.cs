using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using OcraServer.Models.EntityFrameworkModels;
using System.Linq;

namespace OcraServer.EntityFramework
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        #region Constructors

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        #endregion Constructors


        #region Methods

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            // TODO: OnDelete noaction
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            builder.Entity<Feedback>()
                .HasMany(f => f.Images)
                .WithOne(fi => fi.Feedback)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserFavoriteRestaurant>()
                   .HasKey(ufr => new { ufr.UserID, ufr.RestaurantID});

            builder.Entity<ReservationProduct>()
                   .HasKey(ufr => new { ufr.ReservationID ,ufr.ProductID});

            builder.Entity<RefreshToken>()
                   .HasIndex(rt => rt.Token)
                   .IsUnique(true);
        }

        #endregion Methods

        #region Properties

        public virtual DbSet<Discount> Discounts { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<FeedbackImg> FeedbacksImgs { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<ReservationProduct> ReservationProducts { get; set; }
        public virtual DbSet<ExternalReservation> ReservedTables { get; set; }
        public virtual DbSet<Restaurant> Restaurants { get; set; }
        public virtual DbSet<RestaurantFeatures> RestaurantFeatures { get; set; }
        public virtual DbSet<RestaurantImg> RestaurantsImgs { get; set; }
        public virtual DbSet<RestaurantPanorama> RestaurantsPanoramas { get; set; }
        public virtual DbSet<RestaurantMenuCategory> RestaurantsCategories { get; set; }
        public virtual DbSet<UserFavoriteRestaurant> UsersFavoriteRestaurants { get; set; }

        #endregion Properties

    }
}
