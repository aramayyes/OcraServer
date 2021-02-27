using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using OcraServer.EntityFramework;
using OcraServer.Enums;

namespace OcraServer.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("BackgroundImgUrl");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<DateTime?>("DateOfBirth");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirebaseNotificationTokenAndroid");

                    b.Property<string>("FirebaseNotificationTokenWeb");

                    b.Property<string>("FirebaseNotificationTokeniOS");

                    b.Property<string>("FirebaseUID");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<bool>("HasFilled");

                    b.Property<string>("ImgUrl")
                        .IsRequired();

                    b.Property<bool>("IsActive");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<int>("Points");

                    b.Property<int?>("RestaurantID");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool?>("Sex");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.Discount", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("Deadline");

                    b.Property<string>("DescriptionArm")
                        .IsRequired()
                        .HasMaxLength(2000);

                    b.Property<string>("DescriptionEng")
                        .HasMaxLength(2000);

                    b.Property<string>("DescriptionRus")
                        .HasMaxLength(2000);

                    b.Property<int?>("DiscountSize");

                    b.Property<string>("ImgLink")
                        .IsRequired()
                        .HasMaxLength(445);

                    b.Property<bool>("IsActive");

                    b.Property<string>("NameArm")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("NameEng")
                        .HasMaxLength(100);

                    b.Property<string>("NameRus")
                        .HasMaxLength(100);

                    b.Property<int?>("NewPrice");

                    b.Property<int>("RestaurantID");

                    b.HasKey("ID");

                    b.HasIndex("RestaurantID");

                    b.ToTable("Discount");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.Event", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AdditionalPrice");

                    b.Property<string>("DescriptionArm")
                        .IsRequired()
                        .HasMaxLength(2000);

                    b.Property<string>("DescriptionEng")
                        .HasMaxLength(2000);

                    b.Property<string>("DescriptionRus")
                        .HasMaxLength(2000);

                    b.Property<DateTime>("EventDateTime");

                    b.Property<string>("ImgLink")
                        .IsRequired()
                        .HasMaxLength(445);

                    b.Property<bool>("IsActive");

                    b.Property<string>("NameArm")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("NameEng")
                        .HasMaxLength(100);

                    b.Property<string>("NameRus")
                        .HasMaxLength(100);

                    b.Property<int>("RestaurantID");

                    b.HasKey("ID");

                    b.HasIndex("RestaurantID");

                    b.ToTable("Event");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.ExternalReservation", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("PeopleCount");

                    b.Property<DateTime>("ReservationDateTime");

                    b.Property<int>("RestaurantID");

                    b.Property<int?>("TableNumber");

                    b.HasKey("ID");

                    b.HasIndex("RestaurantID");

                    b.ToTable("ExternalReservation");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.Feedback", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<bool>("HasBeenEdited");

                    b.Property<double>("Mark");

                    b.Property<int>("RestaurantID");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<string>("UserID")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("RestaurantID");

                    b.HasIndex("UserID");

                    b.ToTable("Feedback");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.FeedbackImg", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("FeedbackID");

                    b.Property<string>("ImgLink")
                        .IsRequired()
                        .HasMaxLength(445);

                    b.HasKey("ID");

                    b.HasIndex("FeedbackID");

                    b.ToTable("FeedbackImg");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.Product", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CategoryID");

                    b.Property<string>("DescriptionArm")
                        .HasMaxLength(2000);

                    b.Property<string>("DescriptionEng")
                        .HasMaxLength(2000);

                    b.Property<string>("DescriptionRus")
                        .HasMaxLength(2000);

                    b.Property<string>("ImgLink")
                        .IsRequired()
                        .HasMaxLength(445);

                    b.Property<bool>("IsActive");

                    b.Property<string>("NameArm")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("NameEng")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("NameRus")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int>("Price");

                    b.Property<int>("RestaurantID");

                    b.HasKey("ID");

                    b.HasIndex("CategoryID");

                    b.HasIndex("RestaurantID");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.RefreshToken", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ExpirationDate");

                    b.Property<bool>("IsActive");

                    b.Property<string>("Token")
                        .IsRequired();

                    b.Property<string>("UserID")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("Token")
                        .IsUnique();

                    b.HasIndex("UserID");

                    b.ToTable("RefreshToken");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.Reservation", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<bool>("IsActive");

                    b.Property<string>("Note")
                        .HasMaxLength(500);

                    b.Property<int?>("PeopleCount");

                    b.Property<DateTime>("ReservationDateTime");

                    b.Property<int>("RestaurantID");

                    b.Property<int>("Status");

                    b.Property<int?>("SumPrice");

                    b.Property<int?>("TableNumber");

                    b.Property<string>("UserID")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("RestaurantID");

                    b.HasIndex("UserID");

                    b.ToTable("Reservation");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.ReservationProduct", b =>
                {
                    b.Property<int>("ReservationID");

                    b.Property<int>("ProductID");

                    b.Property<int>("Count");

                    b.Property<int>("Price");

                    b.HasKey("ReservationID", "ProductID");

                    b.HasAlternateKey("ProductID", "ReservationID");

                    b.ToTable("ReservationProduct");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.Restaurant", b =>
                {
                    b.Property<int>("ID");

                    b.Property<TimeSpan?>("AdditionalClosingTime");

                    b.Property<TimeSpan?>("AdditionalOpeningTime");

                    b.Property<string>("AdditionalPhoneNumber")
                        .HasMaxLength(20);

                    b.Property<string>("AddressArm")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("AddressEng")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("AddressRus")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("AgentID")
                        .IsRequired();

                    b.Property<string>("Brand")
                        .HasMaxLength(100);

                    b.Property<int>("Category");

                    b.Property<string>("CityArm")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("CityEng")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("CityRus")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<TimeSpan?>("ClosingTime");

                    b.Property<string>("Cost")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("CuisineArm")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("CuisineEng")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("CuisineRus")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("DescriptionArm")
                        .IsRequired()
                        .HasMaxLength(2000);

                    b.Property<string>("DescriptionEng")
                        .IsRequired()
                        .HasMaxLength(2000);

                    b.Property<string>("DescriptionRus")
                        .IsRequired()
                        .HasMaxLength(2000);

                    b.Property<string>("Email")
                        .HasMaxLength(20);

                    b.Property<string>("Facebook")
                        .HasMaxLength(80);

                    b.Property<int>("FeaturesID");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsMain");

                    b.Property<bool>("IsOpen24");

                    b.Property<double>("Latitude");

                    b.Property<string>("LogoLink")
                        .HasMaxLength(445);

                    b.Property<double>("Longitude");

                    b.Property<string>("MainImgLink")
                        .IsRequired()
                        .HasMaxLength(445);

                    b.Property<string>("NameArm")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("NameEng")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("NameRus")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<TimeSpan?>("OpeningTime");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<int>("RatedCount");

                    b.Property<double>("Rating");

                    b.Property<string>("ServePrice");

                    b.Property<int>("TableCount");

                    b.Property<string>("WebSite")
                        .HasMaxLength(50);

                    b.HasKey("ID");

                    b.HasIndex("AgentID");

                    b.HasIndex("FeaturesID");

                    b.ToTable("Restaurant");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.RestaurantFeatures", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("HasCreditCard");

                    b.Property<bool>("HasKidsZone");

                    b.Property<bool>("HasNoSmokingZone");

                    b.Property<bool>("HasOutsideZone");

                    b.Property<bool>("HasTakeAway");

                    b.Property<bool>("HasWiFi");

                    b.HasKey("ID");

                    b.ToTable("RestaurantFeatures");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.RestaurantImg", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ImgLink")
                        .IsRequired()
                        .HasMaxLength(445);

                    b.Property<int>("RestaurantID");

                    b.HasKey("ID");

                    b.HasIndex("RestaurantID");

                    b.ToTable("RestaurantImg");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.RestaurantMenuCategory", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("NameArm")
                        .IsRequired();

                    b.Property<string>("NameEng")
                        .IsRequired();

                    b.Property<string>("NameRus")
                        .IsRequired();

                    b.Property<int>("RestaurantID");

                    b.HasKey("ID");

                    b.HasIndex("RestaurantID");

                    b.ToTable("RestaurantProductCategory");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.RestaurantPanorama", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("PanoramaLink")
                        .IsRequired();

                    b.Property<int>("RestaurantID");

                    b.HasKey("ID");

                    b.HasIndex("RestaurantID");

                    b.ToTable("RestaurantPanorama");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.UserFavoriteRestaurant", b =>
                {
                    b.Property<string>("UserID");

                    b.Property<int>("RestaurantID");

                    b.Property<DateTime>("AddedDate");

                    b.HasKey("UserID", "RestaurantID");

                    b.HasAlternateKey("RestaurantID", "UserID");

                    b.ToTable("UserFavoriteRestaurant");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("OcraServer.Models.EntityFrameworkModels.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("OcraServer.Models.EntityFrameworkModels.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId");

                    b.HasOne("OcraServer.Models.EntityFrameworkModels.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.Discount", b =>
                {
                    b.HasOne("OcraServer.Models.EntityFrameworkModels.Restaurant", "Restaurant")
                        .WithMany("Discounts")
                        .HasForeignKey("RestaurantID");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.Event", b =>
                {
                    b.HasOne("OcraServer.Models.EntityFrameworkModels.Restaurant", "Restaurant")
                        .WithMany("Events")
                        .HasForeignKey("RestaurantID");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.ExternalReservation", b =>
                {
                    b.HasOne("OcraServer.Models.EntityFrameworkModels.Restaurant", "Restaurant")
                        .WithMany("ReservedTables")
                        .HasForeignKey("RestaurantID");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.Feedback", b =>
                {
                    b.HasOne("OcraServer.Models.EntityFrameworkModels.Restaurant", "Restaurant")
                        .WithMany("Feedbacks")
                        .HasForeignKey("RestaurantID");

                    b.HasOne("OcraServer.Models.EntityFrameworkModels.ApplicationUser", "User")
                        .WithMany("Feedbacks")
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.FeedbackImg", b =>
                {
                    b.HasOne("OcraServer.Models.EntityFrameworkModels.Feedback", "Feedback")
                        .WithMany("Images")
                        .HasForeignKey("FeedbackID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.Product", b =>
                {
                    b.HasOne("OcraServer.Models.EntityFrameworkModels.RestaurantMenuCategory", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryID");

                    b.HasOne("OcraServer.Models.EntityFrameworkModels.Restaurant", "Restaurant")
                        .WithMany()
                        .HasForeignKey("RestaurantID");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.RefreshToken", b =>
                {
                    b.HasOne("OcraServer.Models.EntityFrameworkModels.ApplicationUser", "User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.Reservation", b =>
                {
                    b.HasOne("OcraServer.Models.EntityFrameworkModels.Restaurant", "Restaurant")
                        .WithMany("Reservations")
                        .HasForeignKey("RestaurantID");

                    b.HasOne("OcraServer.Models.EntityFrameworkModels.ApplicationUser", "User")
                        .WithMany("Reservations")
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.ReservationProduct", b =>
                {
                    b.HasOne("OcraServer.Models.EntityFrameworkModels.Product", "Product")
                        .WithMany("ReservationProducts")
                        .HasForeignKey("ProductID");

                    b.HasOne("OcraServer.Models.EntityFrameworkModels.Reservation", "Reservation")
                        .WithMany("OrderedProducts")
                        .HasForeignKey("ReservationID");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.Restaurant", b =>
                {
                    b.HasOne("OcraServer.Models.EntityFrameworkModels.ApplicationUser", "Agent")
                        .WithMany()
                        .HasForeignKey("AgentID");

                    b.HasOne("OcraServer.Models.EntityFrameworkModels.RestaurantFeatures", "Features")
                        .WithMany("Restaurants")
                        .HasForeignKey("FeaturesID");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.RestaurantImg", b =>
                {
                    b.HasOne("OcraServer.Models.EntityFrameworkModels.Restaurant", "Restaurant")
                        .WithMany("Images")
                        .HasForeignKey("RestaurantID");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.RestaurantMenuCategory", b =>
                {
                    b.HasOne("OcraServer.Models.EntityFrameworkModels.Restaurant", "Restaurant")
                        .WithMany("MenuCategories")
                        .HasForeignKey("RestaurantID");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.RestaurantPanorama", b =>
                {
                    b.HasOne("OcraServer.Models.EntityFrameworkModels.Restaurant", "Restaurant")
                        .WithMany("Panoramas")
                        .HasForeignKey("RestaurantID");
                });

            modelBuilder.Entity("OcraServer.Models.EntityFrameworkModels.UserFavoriteRestaurant", b =>
                {
                    b.HasOne("OcraServer.Models.EntityFrameworkModels.Restaurant", "Restaurant")
                        .WithMany("FavoriteUsers")
                        .HasForeignKey("RestaurantID");

                    b.HasOne("OcraServer.Models.EntityFrameworkModels.ApplicationUser", "User")
                        .WithMany("FavoriteRestaurants")
                        .HasForeignKey("UserID");
                });
        }
    }
}
