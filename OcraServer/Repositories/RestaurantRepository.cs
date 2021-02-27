using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OcraServer.EntityFramework;
using OcraServer.Enums;
using OcraServer.Models.EntityFrameworkModels;
using OcraServer.Models.ViewModels;

namespace OcraServer.Repositories
{
    public class RestaurantRepository : BaseRepository<Restaurant>, IRepository<Restaurant>
    {
        // Constructors
        public RestaurantRepository(ApplicationDbContext context) : base(context)
        {
            Table = Context.Restaurants;
        }

        // Get All
        public List<RestaurantViewModel> GetViewAll(APIDataLanguage lang)
        {
            var restaurants = GetIQueryable(lang, r => r.IsActive && r.IsMain).ToList();
            return restaurants;
        }

        public Task<List<RestaurantViewModel>> GetViewAllAsync(APIDataLanguage lang)
        {
            var restaurants = GetIQueryable(lang, r => r.IsActive && r.IsMain).ToListAsync();
            return restaurants;
        }

        // Get Count
        public new int GetCount() => Table.Count(r => r.IsActive && r.IsMain);
        public new Task<int> GetCountAsync() => Table.CountAsync(r => r.IsActive && r.IsMain);

        // Get Range
        public List<RestaurantViewModel> GetViewRange(APIDataLanguage lang, int startIndex, int count)
        {
            var restaurants = GetIQueryable(lang, r => r.IsActive && r.IsMain).Skip(startIndex - 1).Take(count).ToList();
            return restaurants;
        }

        public Task<List<RestaurantViewModel>> GetViewRangeAsync(APIDataLanguage lang, int startIndex, int count)
        {
            var restaurants = GetIQueryable(lang, r => r.IsActive && r.IsMain).Skip(startIndex - 1).Take(count).ToListAsync();
            return restaurants;
        }

        // Get Count by category
        public int GetCountByCategory(RestaurantCategory category) => Table.Count(r => r.IsActive && r.IsMain && r.Category == category);
        public Task<int> GetCountByCategoryAsync(RestaurantCategory category) => Table.CountAsync(r => r.IsActive && r.IsMain && r.Category == category);

        // Get Range by category
        public List<RestaurantViewModel> GetViewRangeByCategory(APIDataLanguage lang, RestaurantCategory category, int startIndex, int count)
        {
            var restaurants = GetIQueryable(lang, r => r.IsActive && r.IsMain && r.Category == category).Skip(startIndex - 1).Take(count).ToList();
            return restaurants;
        }

        public Task<List<RestaurantViewModel>> GetViewRangeByCategoryAsync(APIDataLanguage lang, RestaurantCategory category, int startIndex, int count)
        {
            var restaurants = GetIQueryable(lang, r => r.IsActive && r.IsMain && r.Category == category).Skip(startIndex - 1).Take(count).ToListAsync();
            return restaurants;
        }

        // Get All for map
        public List<RestaurantForMapViewModel> GetAllForMap(APIDataLanguage lang)
        {
            var restaurants = GetIQueryableForMap(lang, r => r.IsActive && r.IsMain).ToList();
            return restaurants;

        }

        public Task<List<RestaurantForMapViewModel>> GetAllForMapAsync(APIDataLanguage lang)
        {
            var restaurants = GetIQueryableForMap(lang, r => r.IsActive && r.IsMain).ToListAsync();
            return restaurants;

        }

        // Get for map by category
        public List<RestaurantForMapViewModel> GetForMapByCategories(APIDataLanguage lang, bool Restaurant, bool Tavern, bool FastFood, bool Cafe, bool Pub)
        {

            var restaurants = GetIQueryableForMap(lang, r => r.IsActive && ((
                                                            (Restaurant == true && r.Category == RestaurantCategory.Restaurant)
                                                         || (Tavern == true && r.Category == RestaurantCategory.Tavern)
                                                         || (FastFood == true && r.Category == RestaurantCategory.FastFood)
                                                         || (Cafe == true && r.Category == RestaurantCategory.Cafe)
                                                         || (Pub == true && r.Category == RestaurantCategory.Pub)))).ToList();

            return restaurants;
        }

        public Task<List<RestaurantForMapViewModel>> GetForMapByCategoriesAsync(APIDataLanguage lang, bool Restaurant, bool Tavern, bool FastFood, bool Cafe, bool Pub)
        {
            var restaurants = GetIQueryableForMap(lang, r => r.IsActive && ((
                                                           (Restaurant == true && r.Category == RestaurantCategory.Restaurant)
                                                        || (Tavern == true && r.Category == RestaurantCategory.Tavern)
                                                        || (FastFood == true && r.Category == RestaurantCategory.FastFood)
                                                        || (Cafe == true && r.Category == RestaurantCategory.Cafe)
                                                        || (Pub == true && r.Category == RestaurantCategory.Pub)))).ToListAsync();

            return restaurants;
        }

        // Get by ID
        public RestaurantDetailsViewModel GetViewOne(APIDataLanguage lang, int id)
        {
            RestaurantDetailsViewModel restaurant = GetIQueryableWithDetails(lang, id).ToList().FirstOrDefault();
            return restaurant;
        }

        public Task<List<RestaurantDetailsViewModel>> GetViewOneAsync(APIDataLanguage lang, int id)
        {
            var restaurant = GetIQueryableWithDetails(lang, id).ToListAsync();
            return restaurant;
        }


        // Get By Brand
        public List<RestaurantViewModel> GetViewByBrand(APIDataLanguage lang, string brand, int restID)
        {
            var restaurants = GetIQueryable(lang, r => r.IsActive && r.Brand == brand && r.ID != restID).ToList();
            return restaurants;
        }

        public Task<List<RestaurantViewModel>> GetViewByBrandAsync(APIDataLanguage lang, string brand, int restID)
        {
            var restaurants = GetIQueryable(lang, r => r.IsActive && r.Brand == brand && r.ID != restID).ToListAsync();
            return restaurants;
        }

        // Get by Agent
        public RestaurantForAgentViewModel GetViewByAgent(string userID)
        {
            RestaurantForAgentViewModel restaurant = GetIQueryableForAgent(r => r.IsActive && r.AgentID == userID).FirstOrDefault();
            return restaurant;
        }

        public Task<RestaurantForAgentViewModel> GetViewByAgentAsync(string userID)
        {
            Task<RestaurantForAgentViewModel> restaurant = GetIQueryableForAgent(r => r.IsActive && r.AgentID == userID).FirstOrDefaultAsync();
            return restaurant;
        }

        // Search
        public List<RestaurantSearchResultViewModel> Search(APIDataLanguage lang, string searchTerm)
        {
            var restaurants = GetIQueryableForSearch(lang, r => r.IsActive &&
                                                           (r.NameEng.Contains(searchTerm)
                                                         || r.NameRus.Contains(searchTerm)
                                                         || r.NameArm.Contains(searchTerm))
                                                      ).ToList();

            return restaurants;
        }

        public Task<List<RestaurantSearchResultViewModel>> SearchAsync(APIDataLanguage lang, string searchTerm)
        {
            var restaurants = GetIQueryableForSearch(lang, r => r.IsActive &&
                                                           (r.NameEng.Contains(searchTerm)
                                                         || r.NameRus.Contains(searchTerm)
                                                         || r.NameArm.Contains(searchTerm))
                                                    ).ToListAsync();

            return restaurants;
        }

        public IQueryable<NameForSuggestionModel> GetNames()
        {
            return (Table as IQueryable<Restaurant>).Where(r => r.IsActive && r.IsMain).Select(r => new NameForSuggestionModel { Name_Arm = r.NameArm, Name_Rus = r.NameRus, Name_Eng = r.NameEng });
        }

        // Get categories
        public int ValidateCategory(int restID, int id)
        {
            return Context.RestaurantsCategories.Count(rpc => rpc.RestaurantID == restID && rpc.ID == id);
        }

        public Task<int> ValidateCategoryAsync(int restID, int id)
        {
            return Context.RestaurantsCategories.CountAsync(rpc => rpc.RestaurantID == restID && rpc.ID == id);
        }

        // Get table count and panorama link
        public RestaurantDataForReservation GetRestTableCountNPanoramas(APIDataLanguage lang, int restID)
        {
            switch (lang)
            {
                case APIDataLanguage.EN:
                    return Table.GroupJoin(Context.RestaurantsPanoramas, r => r.ID, rp => rp.RestaurantID, (r, rp) => new RestaurantDataForReservation
                    {
                        ID = r.ID,
                        TableCount = r.TableCount,
                        IsOpen24 = r.IsOpen24,
                        RestaurantName = r.NameEng,
                        AdditionalClosingTime = r.AdditionalClosingTime,
                        AdditionalOpeningTime = r.AdditionalOpeningTime,
                        ClosingTime = r.ClosingTime,
                        OpeningTime = r.OpeningTime,
                        Panoramas = rp
                    }).Where(r => r.ID == restID).ToList().FirstOrDefault();
                case APIDataLanguage.RU:
                    return Table.GroupJoin(Context.RestaurantsPanoramas, r => r.ID, rp => rp.RestaurantID, (r, rp) => new RestaurantDataForReservation
                    {
                        ID = r.ID,
                        TableCount = r.TableCount,
                        IsOpen24 = r.IsOpen24,
                        RestaurantName = r.NameRus,
                        AdditionalClosingTime = r.AdditionalClosingTime,
                        AdditionalOpeningTime = r.AdditionalOpeningTime,
                        ClosingTime = r.ClosingTime,
                        OpeningTime = r.OpeningTime,
                        Panoramas = rp
                    }).Where(r => r.ID == restID).ToList().FirstOrDefault();
                case APIDataLanguage.AM:
                default:
                    return Table.GroupJoin(Context.RestaurantsPanoramas, r => r.ID, rp => rp.RestaurantID, (r, rp) => new RestaurantDataForReservation
                    {
                        ID = r.ID,
                        TableCount = r.TableCount,
                        IsOpen24 = r.IsOpen24,
                        RestaurantName = r.NameArm,
                        AdditionalClosingTime = r.AdditionalClosingTime,
                        AdditionalOpeningTime = r.AdditionalOpeningTime,
                        ClosingTime = r.ClosingTime,
                        OpeningTime = r.OpeningTime,
                        Panoramas = rp
                    }).Where(r => r.ID == restID).ToList().FirstOrDefault();
            }
        }

        public Task<List<RestaurantDataForReservation>> GetRestTableCountNPanoramasAsync(APIDataLanguage lang, int restID)
        {
            switch (lang)
            {
                case APIDataLanguage.EN:
                    return Table.GroupJoin(Context.RestaurantsPanoramas, r => r.ID, rp => rp.RestaurantID, (r, rp) => new RestaurantDataForReservation
                    {
                        ID = r.ID,
                        TableCount = r.TableCount,
                        IsOpen24 = r.IsOpen24,
                        RestaurantName = r.NameEng,
                        AdditionalClosingTime = r.AdditionalClosingTime,
                        AdditionalOpeningTime = r.AdditionalOpeningTime,
                        ClosingTime = r.ClosingTime,
                        OpeningTime = r.OpeningTime,
                        Panoramas = rp
                    }).Where(r => r.ID == restID).ToListAsync();
                case APIDataLanguage.RU:
                    return Table.GroupJoin(Context.RestaurantsPanoramas, r => r.ID, rp => rp.RestaurantID, (r, rp) => new RestaurantDataForReservation
                    {
                        ID = r.ID,
                        TableCount = r.TableCount,
                        IsOpen24 = r.IsOpen24,
                        RestaurantName = r.NameRus,
                        AdditionalClosingTime = r.AdditionalClosingTime,
                        AdditionalOpeningTime = r.AdditionalOpeningTime,
                        ClosingTime = r.ClosingTime,
                        OpeningTime = r.OpeningTime,
                        Panoramas = rp
                    }).Where(r => r.ID == restID).ToListAsync();
                case APIDataLanguage.AM:
                default:
                    return Table.GroupJoin(Context.RestaurantsPanoramas, r => r.ID, rp => rp.RestaurantID, (r, rp) => new RestaurantDataForReservation
                    {
                        ID = r.ID,
                        TableCount = r.TableCount,
                        IsOpen24 = r.IsOpen24,
                        RestaurantName = r.NameArm,
                        AdditionalClosingTime = r.AdditionalClosingTime,
                        AdditionalOpeningTime = r.AdditionalOpeningTime,
                        ClosingTime = r.ClosingTime,
                        OpeningTime = r.OpeningTime,
                        Panoramas = rp
                    }).Where(r => r.ID == restID).ToListAsync();
            }
        }

        // Get One
        public new Restaurant GetOne(int? id) => Table.Include(r => r.Agent).FirstOrDefault(r => r.ID == id);
        public new async Task<Restaurant> GetOneAsync(int? id)
        {
            return await Table.Include(r => r.Agent).FirstOrDefaultAsync(r => r.ID == id);
        }

        // Delete By Id
        public int Delete(int id)
        {
            Context.Entry(new Restaurant()
            {
                ID = id
            }).State = EntityState.Deleted;
            return SaveChanges();
        }

        public Task<int> DeleteAsync(int id)
        {
            Context.Entry(new Restaurant()
            {
                ID = id
            }).State = EntityState.Deleted;
            return SaveChangesAsync();
        }

        // Private helpers
        private IQueryable<RestaurantViewModel> GetIQueryable(APIDataLanguage lang, Expression<Func<Restaurant, bool>> predicate)
        {
            var nowTime = DateTime.UtcNow.TimeOfDay;
            var isWeekend = ((int)DateTime.UtcNow.DayOfWeek == 6 || (int)DateTime.UtcNow.DayOfWeek == 6) ? true : false;

            switch (lang)
            {
                case APIDataLanguage.EN:
                    return (Table as IQueryable<Restaurant>).Where(predicate).OrderByDescending(r => r.ID).Select(r => new RestaurantViewModel
                    {
                        ID = r.ID,
                        Address = r.AddressEng,
                        City = r.CityEng,
                        Cost = r.Cost,
                        LogoLink = r.LogoLink,
                        MainImgLink = r.MainImgLink,
                        Name = r.NameEng,
                        IsOpen24 = r.IsOpen24,
                        OpeningTime = r.OpeningTime,
                        ClosingTime = r.ClosingTime,
                        AdditionalOpeningTime = r.AdditionalOpeningTime,
                        AdditionalClosingTime = r.AdditionalClosingTime,
                        RatedCount = r.RatedCount,
                        Rating = r.Rating,
                        IsOpen = r.IsOpen24 || (!isWeekend && nowTime > r.OpeningTime && nowTime < r.ClosingTime) || (isWeekend && nowTime > r.AdditionalOpeningTime && nowTime < r.AdditionalClosingTime)
                    });
                case APIDataLanguage.RU:
                    return ((Table as IQueryable<Restaurant>)
                             .Where(predicate)
                             .OrderByDescending(r => r.ID)
                             .Select(r => new RestaurantViewModel
                             {
                                 ID = r.ID,
                                 Address = r.AddressRus,
                                 City = r.CityRus,
                                 Cost = r.Cost,
                                 LogoLink = r.LogoLink,
                                 MainImgLink = r.MainImgLink,
                                 Name = r.NameRus,
                                 IsOpen24 = r.IsOpen24,
                                 OpeningTime = r.OpeningTime,
                                 ClosingTime = r.ClosingTime,
                                 AdditionalOpeningTime = r.AdditionalOpeningTime,
                                 AdditionalClosingTime = r.AdditionalClosingTime,
                                 RatedCount = r.RatedCount,
                                 Rating = r.Rating,
                                 IsOpen = r.IsOpen24 || (!isWeekend && nowTime > r.OpeningTime && nowTime < r.ClosingTime) || (isWeekend && nowTime > r.AdditionalOpeningTime && nowTime < r.AdditionalClosingTime)
                             }));
                case APIDataLanguage.AM:
                default:
                    return ((Table as IQueryable<Restaurant>)
                             .Where(predicate)
                             .OrderByDescending(r => r.ID)
                             .Select(r => new RestaurantViewModel
                             {
                                 ID = r.ID,
                                 Address = r.AddressArm,
                                 City = r.CityArm,
                                 Cost = r.Cost,
                                 LogoLink = r.LogoLink,
                                 MainImgLink = r.MainImgLink,
                                 Name = r.NameArm,
                                 IsOpen24 = r.IsOpen24,
                                 OpeningTime = r.OpeningTime,
                                 ClosingTime = r.ClosingTime,
                                 AdditionalOpeningTime = r.AdditionalOpeningTime,
                                 AdditionalClosingTime = r.AdditionalClosingTime,
                                 RatedCount = r.RatedCount,
                                 Rating = r.Rating,
                                 IsOpen = r.IsOpen24 || (!isWeekend && nowTime > r.OpeningTime && nowTime < r.ClosingTime) || (isWeekend && nowTime > r.AdditionalOpeningTime && nowTime < r.AdditionalClosingTime)
                             }));
            }
        }

        private IQueryable<RestaurantForMapViewModel> GetIQueryableForMap(APIDataLanguage lang, Expression<Func<Restaurant, bool>> predicate)
        {
            switch (lang)
            {
                case APIDataLanguage.EN:
                    return (Table as IQueryable<Restaurant>).Where(predicate).OrderByDescending(r => r.ID).Select(restaurant => new RestaurantForMapViewModel
                    {
                        ID = restaurant.ID,
                        Address = restaurant.AddressEng,
                        Category = restaurant.Category,
                        LogoLink = restaurant.LogoLink,
                        Latitude = restaurant.Latitude,
                        Longitude = restaurant.Longitude,
                        Rating = restaurant.Rating,
                        Name = restaurant.NameEng
                    });
                case APIDataLanguage.RU:
                    return (Table as IQueryable<Restaurant>).Where(predicate).OrderByDescending(r => r.ID).Select(restaurant => new RestaurantForMapViewModel
                    {
                        ID = restaurant.ID,
                        Address = restaurant.AddressRus,
                        Category = restaurant.Category,
                        LogoLink = restaurant.LogoLink,
                        Latitude = restaurant.Latitude,
                        Longitude = restaurant.Longitude,
                        Rating = restaurant.Rating,
                        Name = restaurant.NameRus
                    });
                case APIDataLanguage.AM:
                default:
                    return (Table as IQueryable<Restaurant>).Where(predicate).OrderByDescending(r => r.ID).Select(restaurant => new RestaurantForMapViewModel
                    {
                        ID = restaurant.ID,
                        Address = restaurant.AddressArm,
                        Category = restaurant.Category,
                        LogoLink = restaurant.LogoLink,
                        Latitude = restaurant.Latitude,
                        Longitude = restaurant.Longitude,
                        Rating = restaurant.Rating,
                        Name = restaurant.NameArm
                    });
            }
        }

        public IQueryable<RestaurantDetailsViewModel> GetIQueryableWithDetails(APIDataLanguage lang, int restID)
        {
            switch (lang)
            {
                case APIDataLanguage.EN:
                    return Table
                        .Join(Context.RestaurantFeatures, r => r.FeaturesID, rf => rf.ID, (r, rf) => new { restaurant = r, rf })
                        .GroupJoin(Context.RestaurantsImgs, a => a.restaurant.ID, rimg => rimg.RestaurantID, (r, rimgs) => new { r.restaurant, r.rf, rimgs })
                        .Where(x => x.restaurant.ID == restID && x.restaurant.IsActive)
                        .Select(x => new RestaurantDetailsViewModel
                        {
                            ID = x.restaurant.ID,
                            Address = x.restaurant.AddressEng,
                            Category = x.restaurant.Category,
                            LogoLink = x.restaurant.LogoLink,
                            Latitude = x.restaurant.Latitude,
                            Longitude = x.restaurant.Longitude,
                            Name = x.restaurant.NameEng,
                            AdditionalPhoneNumber = x.restaurant.AdditionalPhoneNumber,
                            Brand = x.restaurant.Brand,
                            City = x.restaurant.CityEng,
                            Cost = x.restaurant.Cost,
                            Cuisine = x.restaurant.CuisineEng,
                            Description = x.restaurant.DescriptionEng,
                            Email = x.restaurant.Email,
                            Facebook = x.restaurant.Facebook,
                            IsMain = x.restaurant.IsMain,
                            MainImgLink = x.restaurant.MainImgLink,
                            PhoneNumber = x.restaurant.PhoneNumber,
                            WebSite = x.restaurant.WebSite,
                            ServePrice = x.restaurant.ServePrice,
                            Images = x.rimgs,
                            IsOpen24 = x.restaurant.IsOpen24,
                            Features = x.rf,
                            OpeningTime = x.restaurant.OpeningTime,
                            ClosingTime = x.restaurant.ClosingTime,
                            AdditionalOpeningTime = x.restaurant.AdditionalOpeningTime,
                            AdditionalClosingTime = x.restaurant.AdditionalClosingTime,
                            RatedCount = x.restaurant.RatedCount,
                            Rating = x.restaurant.Rating
                        });
                case APIDataLanguage.RU:
                    return Table
                        .Join(Context.RestaurantFeatures, r => r.FeaturesID, rf => rf.ID, (r, rf) => new { restaurant = r, rf })
                        .GroupJoin(Context.RestaurantsImgs, a => a.restaurant.ID, rimg => rimg.RestaurantID, (r, rimgs) => new { r.restaurant, r.rf, rimgs })
                        .Where(x => x.restaurant.ID == restID && x.restaurant.IsActive).Select(x => new RestaurantDetailsViewModel
                        {
                            ID = x.restaurant.ID,
                            Address = x.restaurant.AddressRus,
                            Category = x.restaurant.Category,
                            LogoLink = x.restaurant.LogoLink,
                            Latitude = x.restaurant.Latitude,
                            Longitude = x.restaurant.Longitude,
                            Name = x.restaurant.NameRus,
                            AdditionalPhoneNumber = x.restaurant.AdditionalPhoneNumber,
                            Brand = x.restaurant.Brand,
                            City = x.restaurant.CityRus,
                            Cost = x.restaurant.Cost,
                            Cuisine = x.restaurant.CuisineRus,
                            Description = x.restaurant.DescriptionRus,
                            Email = x.restaurant.Email,
                            Facebook = x.restaurant.Facebook,
                            IsMain = x.restaurant.IsMain,
                            MainImgLink = x.restaurant.MainImgLink,
                            PhoneNumber = x.restaurant.PhoneNumber,
                            WebSite = x.restaurant.WebSite,
                            ServePrice = x.restaurant.ServePrice,
                            Images = x.rimgs,
                            IsOpen24 = x.restaurant.IsOpen24,
                            Features = x.rf,
                            OpeningTime = x.restaurant.OpeningTime,
                            ClosingTime = x.restaurant.ClosingTime,
                            AdditionalOpeningTime = x.restaurant.AdditionalOpeningTime,
                            AdditionalClosingTime = x.restaurant.AdditionalClosingTime,
							RatedCount = x.restaurant.RatedCount,
							Rating = x.restaurant.Rating
                        });
                /*return Table
                    .Join(Context.RestaurantFeatures, r => r.FeaturesID, rf => rf.ID, (r, rf) => new { restaurant = r, rf })
                    .GroupJoin(Context.RestaurantsImgs, a => a.restaurant.ID, rimg => rimg.RestaurantID, (r, rimgs) => new { r, rimgs })
                    .Where(x => x.r.restaurant.ID == restID && x.r.restaurant.IsActive).Select(x => new RestaurantDetailsViewModel
                    {
                        ID = x.r.restaurant.ID,
                        Address = x.r.restaurant.AddressRus,
                        Category = x.r.restaurant.Category,
                        LogoLink = x.r.restaurant.LogoLink,
                        Latitude = x.r.restaurant.Latitude,
                        Longitude = x.r.restaurant.Longitude,
                        Name = x.r.restaurant.NameRus,
                        AdditionalPhoneNumber = x.r.restaurant.AdditionalPhoneNumber,
                        Brand = x.r.restaurant.Brand,
                        City = x.r.restaurant.CityRus,
                        Cost = x.r.restaurant.Cost,
                        Cuisine = x.r.restaurant.CuisineRus,
                        Description = x.r.restaurant.DescriptionRus,
                        Email = x.r.restaurant.Email,
                        Facebook = x.r.restaurant.Facebook,
                        IsMain = x.r.restaurant.IsMain,
                        MainImgLink = x.r.restaurant.MainImgLink,
                        PhoneNumber = x.r.restaurant.PhoneNumber,
                        WebSite = x.r.restaurant.WebSite,
                        ServPrice = x.r.restaurant.ServPrice,
                        Images = x.rimgs,
                        IsOpen24 = x.r.restaurant.IsOpen24,
                        Features = x.r.rf,
                        OpeningTime = x.r.restaurant.OpeningTime,
                        ClosingTime = x.r.restaurant.ClosingTime,
                        AdditionalOpeningTime = x.r.restaurant.AdditionalOpeningTime,
                        AdditionalClosingTime = x.r.restaurant.AdditionalClosingTime
                    });*/
                case APIDataLanguage.AM:
                default:
                    return Table
                        .Join(Context.RestaurantFeatures, r => r.FeaturesID, rf => rf.ID, (r, rf) => new { restaurant = r, rf })
                        .GroupJoin(Context.RestaurantsImgs, a => a.restaurant.ID, rimg => rimg.RestaurantID, (r, rimgs) => new { r.restaurant, r.rf, rimgs })
                        .Where(x => x.restaurant.ID == restID && x.restaurant.IsActive).Select(x => new RestaurantDetailsViewModel
                        {
                            ID = x.restaurant.ID,
                            Address = x.restaurant.AddressArm,
                            Category = x.restaurant.Category,
                            LogoLink = x.restaurant.LogoLink,
                            Latitude = x.restaurant.Latitude,
                            Longitude = x.restaurant.Longitude,
                            Name = x.restaurant.NameArm,
                            AdditionalPhoneNumber = x.restaurant.AdditionalPhoneNumber,
                            Brand = x.restaurant.Brand,
                            City = x.restaurant.CityArm,
                            Cost = x.restaurant.Cost,
                            Cuisine = x.restaurant.CuisineArm,
                            Description = x.restaurant.DescriptionArm,
                            Email = x.restaurant.Email,
                            Facebook = x.restaurant.Facebook,
                            IsMain = x.restaurant.IsMain,
                            MainImgLink = x.restaurant.MainImgLink,
                            PhoneNumber = x.restaurant.PhoneNumber,
                            WebSite = x.restaurant.WebSite,
                            ServePrice = x.restaurant.ServePrice,
                            Images = x.rimgs,
                            IsOpen24 = x.restaurant.IsOpen24,
                            Features = x.rf,
                            OpeningTime = x.restaurant.OpeningTime,
                            ClosingTime = x.restaurant.ClosingTime,
                            AdditionalOpeningTime = x.restaurant.AdditionalOpeningTime,
                            AdditionalClosingTime = x.restaurant.AdditionalClosingTime,
							RatedCount = x.restaurant.RatedCount,
							Rating = x.restaurant.Rating
                        });
                    /*return Table
                        .Join(Context.RestaurantFeatures, r => r.FeaturesID, rf => rf.ID, (r, rf) => new { restaurant = r, rf })
                        .GroupJoin(Context.RestaurantsImgs, a => a.restaurant.ID, rimg => rimg.RestaurantID, (r, rimgs) => new { r, rimgs })
                        .Where(x => x.r.restaurant.ID == restID && x.r.restaurant.IsActive).Select(x => new RestaurantDetailsViewModel
                        {
                            ID = x.r.restaurant.ID,
                            Address = x.r.restaurant.AddressArm,
                            Category = x.r.restaurant.Category,
                            LogoLink = x.r.restaurant.LogoLink,
                            Latitude = x.r.restaurant.Latitude,
                            Longitude = x.r.restaurant.Longitude,
                            Name = x.r.restaurant.NameArm,
                            AdditionalPhoneNumber = x.r.restaurant.AdditionalPhoneNumber,
                            Brand = x.r.restaurant.Brand,
                            City = x.r.restaurant.CityArm,
                            Cost = x.r.restaurant.Cost,
                            Cuisine = x.r.restaurant.CuisineArm,
                            Description = x.r.restaurant.DescriptionArm,
                            Email = x.r.restaurant.Email,
                            Facebook = x.r.restaurant.Facebook,
                            IsMain = x.r.restaurant.IsMain,
                            MainImgLink = x.r.restaurant.MainImgLink,
                            PhoneNumber = x.r.restaurant.PhoneNumber,
                            WebSite = x.r.restaurant.WebSite,
                            ServPrice = x.r.restaurant.ServPrice,
                            Images = x.rimgs,
                            IsOpen24 = x.r.restaurant.IsOpen24,
                            Features = x.r.rf,
                            OpeningTime = x.r.restaurant.OpeningTime,
                            ClosingTime = x.r.restaurant.ClosingTime,
                            AdditionalOpeningTime = x.r.restaurant.AdditionalOpeningTime,
                            AdditionalClosingTime = x.r.restaurant.AdditionalClosingTime
                        });
                        */
            }
        }

        private IQueryable<RestaurantForAgentViewModel> GetIQueryableForAgent(Expression<Func<Restaurant, bool>> predicate)
        {

            return (Table as IQueryable<Restaurant>).Where(predicate).Select(restaurant => new RestaurantForAgentViewModel
            {
                ID = restaurant.ID,
                AddressEng = restaurant.AddressEng,
                AddressArm = restaurant.AddressArm,
                AddressRus = restaurant.AddressRus,
                Category = restaurant.Category,
                LogoLink = restaurant.LogoLink,
                Latitude = restaurant.Latitude,
                Longitude = restaurant.Longitude,
                NameEng = restaurant.NameEng,
                NameRus = restaurant.NameRus,
                NameArm = restaurant.NameArm,
                AdditionalPhoneNumber = restaurant.AdditionalPhoneNumber,
                Brand = restaurant.Brand,
                CityEng = restaurant.CityEng,
                CityArm = restaurant.CityArm,
                CityRus = restaurant.CityRus,
                Cost = restaurant.Cost,
                CuisineEng = restaurant.CuisineEng,
                CuisineRus = restaurant.CuisineRus,
                CuisineArm = restaurant.CuisineArm,
                DescriptionEng = restaurant.DescriptionEng,
                DescriptionRus = restaurant.DescriptionRus,
                DescriptionArm = restaurant.DescriptionArm,
                Email = restaurant.Email,
                Facebook = restaurant.Facebook,
                IsMain = restaurant.IsMain,
                MainImgLink = restaurant.MainImgLink,
                PhoneNumber = restaurant.PhoneNumber,
                WebSite = restaurant.WebSite,
                ServePrice = restaurant.ServePrice,
                Images = restaurant.Images,
                IsOpen24 = restaurant.IsOpen24,
                Features = restaurant.Features,
                OpeningTime = restaurant.OpeningTime,
                ClosingTime = restaurant.ClosingTime,
                AdditionalOpeningTime = restaurant.AdditionalOpeningTime,
                AdditionalClosingTime = restaurant.AdditionalClosingTime,
                Panoramas = restaurant.Panoramas,
                TableCount = restaurant.TableCount,
                RatedCount = restaurant.RatedCount,
                Rating = restaurant.Rating
            });
        }

        private IQueryable<RestaurantSearchResultViewModel> GetIQueryableForSearch(APIDataLanguage lang, Expression<Func<Restaurant, bool>> predicate)
        {
            switch (lang)
            {
                case APIDataLanguage.EN:
                    return (Table as IQueryable<Restaurant>).Where(predicate).OrderByDescending(r => r.ID).Select(restaurant => new RestaurantSearchResultViewModel
                    {
                        ID = restaurant.ID,
                        Address = restaurant.AddressEng,
                        MainImgLink = restaurant.MainImgLink,
                        Name = restaurant.NameEng,
                        Rating = restaurant.Rating
                    });
                case APIDataLanguage.RU:
                    return (Table as IQueryable<Restaurant>).Where(predicate).OrderByDescending(r => r.ID).Select(restaurant => new RestaurantSearchResultViewModel
                    {
                        ID = restaurant.ID,
                        Address = restaurant.AddressRus,
                        MainImgLink = restaurant.MainImgLink,
                        Name = restaurant.NameRus,
                        Rating = restaurant.Rating
                    });
                case APIDataLanguage.AM:
                default:
                    return (Table as IQueryable<Restaurant>).Where(predicate).OrderByDescending(r => r.ID).Select(restaurant => new RestaurantSearchResultViewModel
                    {
                        ID = restaurant.ID,
                        Address = restaurant.AddressArm,
                        MainImgLink = restaurant.MainImgLink,
                        Name = restaurant.NameArm,
                        Rating = restaurant.Rating,
                    });
            }
        }

        // Get By Agent
        public Restaurant GetByAgent(string agentID) => Table.FirstOrDefault(r => r.AgentID == agentID);
        public Task<Restaurant> GetByAgentAsync(string agentID) => Table.FirstOrDefaultAsync(r => r.AgentID == agentID);
    }
}
