using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OcraServer.Extra;
using OcraServer.Models.EntityFrameworkModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OcraServer.Enums;

namespace OcraServer.EntityFramework
{
    public class DbSeeder
    {
        #region Private Members

        private ApplicationDbContext _dbContext;
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<ApplicationUser> _userManager;

        private int featuresID = 0;

        private List<ApplicationUser> admins = new List<ApplicationUser>();
        private List<ApplicationUser> users = new List<ApplicationUser>();
        private List<Restaurant> restaurants = new List<Restaurant>();
        private List<Discount> discounts = new List<Discount>();
        private List<Event> events = new List<Event>();

        #endregion Private Members

        #region Constructor

        public DbSeeder(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        #endregion Constructor

        #region Public Methods

        public async Task SeedAsync()
        {
            //// Create the Db if it doesn't exist
            //_dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            /* Temporarily */

            //Create default Users
            if (await _dbContext.Users.CountAsync() == 0)
            {
                await CreateUsersAsync();
            }

            //if (await _dbContext.RestaurantFeatures.CountAsync() == 0)
            //{
            //    await CreateResturantsFeaturesAsync();
            //}

            //if (await _dbContext.Restaurants.CountAsync() == 0)
            //{
            //    await CreateRestaurantsAsync();
            //}

            //if (await _dbContext.Discounts.CountAsync() == 0)
            //{
            //    await CreateDiscountsAsync();
            //}

            //if (await _dbContext.Events.CountAsync() == 0)
            //{
            //    await CreateEventsAsync();
            //}

            //if (await _dbContext.UsersFavoriteRestaurants.CountAsync() == 0)
            //{
            //    await CreateUserFavoriteRestaurants();
            //}


        }

        private async Task CreateResturantsFeaturesAsync()
        {
            RestaurantFeatures features = new RestaurantFeatures
            {
                HasCreditCard = true,
                HasKidsZone = true,
                HasNoSmokingZone = true,
                HasOutsideZone = true,
                HasTakeAway = true,
                HasWiFi = true
            };

            await _dbContext.RestaurantFeatures.AddAsync(features);
            await _dbContext.SaveChangesAsync();

            featuresID = features.ID;
        }

        private async Task CreateUserFavoriteRestaurants()
        {
            List<UserFavoriteRestaurant> ufavRestaurants = new List<UserFavoriteRestaurant>();

            Random rand = new Random();
            for (int i = 1; i < 20; i++)
            {
                int r = rand.Next(0, 20);
                var ufavRest = new UserFavoriteRestaurant
                {
                    RestaurantID = restaurants[r].ID,
                    UserID = users[r % 4].Id
                };

                if (!ufavRestaurants.Exists(uf => uf.RestaurantID == ufavRest.RestaurantID && uf.UserID == ufavRest.UserID))
                {
                    ufavRestaurants.Add(ufavRest);
                }
            }

            await _dbContext.UsersFavoriteRestaurants.AddRangeAsync(ufavRestaurants);
            await _dbContext.SaveChangesAsync();
        }

        private async Task CreateEventsAsync()
        {
            Random rand = new Random();
            for (int i = 0; i < 100; ++i)
            {
                events.Add(new Event
                {
                    DescriptionEng = "Description",
                    DescriptionRus = "Описание",
                    DescriptionArm = "Նկարագրություն",

                    ImgLink = "http://mediabitch.ru/wp-content/uploads/2015/04/events-heavenly-header.jpg",

                    NameArm = "Իվենթի անուն",

                    EventDateTime = DateTime.Now,

                    Restaurant = restaurants[rand.Next(0, 20)]
                });
            }

            await _dbContext.Events.AddRangeAsync(events);
            await _dbContext.SaveChangesAsync();
        }

        private async Task CreateDiscountsAsync()
        {
            Random rand = new Random();
            for (int i = 0; i < 100; ++i)
            {
                discounts.Add(new Discount
                {
                    Deadline = DateTime.Now,
                    DescriptionEng = "Description",
                    DescriptionRus = "Описание",
                    DescriptionArm = "Նկարագրություն",

                    ImgLink = @"https://www.mssdefence.com/wp-content/uploads/2016/11/Discount-Action-Mss-Defence.png",

                    NewPrice = 2500,

                    NameArm = "Զեղչի անուն",

                    Restaurant = restaurants[rand.Next(0, 20)]
                });
            }

            await _dbContext.Discounts.AddRangeAsync(discounts);
            await _dbContext.SaveChangesAsync();
        }

        private async Task CreateRestaurantsAsync()
        {
            #region Restaurants

            List<RestaurantImg> BeijImgs = new List<RestaurantImg>();
            BeijImgs.Add(new RestaurantImg
            {
                ImgLink = @"http://oldbeijing.am/wp-content/uploads/2014/06/Panorama1.jpg",
               
            });
            BeijImgs.Add(new RestaurantImg
            {
                ImgLink = @"http://oldbeijing.am/wp-content/uploads/2014/06/IMG_97461.jpg",
            });
            BeijImgs.Add(new RestaurantImg
            {
                ImgLink = @"http://oldbeijing.am/wp-content/uploads/2014/06/IMG_97641.jpg",
            });
            BeijImgs.Add(new RestaurantImg
            {
                ImgLink = @"http://oldbeijing.am/wp-content/uploads/2014/06/IMG_98301.jpg",

            });

            List<RestaurantImg> CactusImgs = new List<RestaurantImg>();
            CactusImgs.Add(new RestaurantImg
            {
                ImgLink = @"http://oldbeijing.am/wp-content/uploads/2014/06/Panorama1.jpg",

            });
            CactusImgs.Add(new RestaurantImg
            {
                ImgLink = @"http://oldbeijing.am/wp-content/uploads/2014/06/IMG_97461.jpg",

            });
            CactusImgs.Add(new RestaurantImg
            {
                ImgLink = @"http://oldbeijing.am/wp-content/uploads/2014/06/IMG_97641.jpg",
               
            });
            CactusImgs.Add(new RestaurantImg
            {
                ImgLink = @"http://oldbeijing.am/wp-content/uploads/2014/06/IMG_98301.jpg",

            });

            List<RestaurantImg> BelImgs = new List<RestaurantImg>();
            BelImgs.Add(new RestaurantImg
            {
                ImgLink = @"http://www.bellagio.am/am/img/gallery/47.jpg",

            });
            BelImgs.Add(new RestaurantImg
            {
                ImgLink = @"http://www.bellagio.am/am/img/gallery/51.jpg",
               
            });
            BelImgs.Add(new RestaurantImg
            {
                ImgLink = @"http://www.bellagio.am/am/img/gallery/49.jpg",
               
            });
            BelImgs.Add(new RestaurantImg
            {
                ImgLink = @"http://www.bellagio.am/am/img/gallery/58.jpg",
            });

            Random rand = new Random();

            for (int i = 1; i <= 20; ++i)
            {
                int r = rand.Next(1, 4);

                switch (r)
                {
                    case 1:
                        restaurants.Add(new Restaurant
                        {
                            ID = i,

                            NameEng = "Old Beijing: CHINESE CUISINE",
                            NameArm = "Հին Պեկին: ՉԻՆԱԿԱՆ ԽՈՀԱՆՈՑ",
                            NameRus = "Старый Пекин: КИТАЙСКАЯ КУХНЯ",

                            AddressEng = "Yerevan, Tumanyan Str. 9-58",
                            AddressRus = "Ереван ул. Туманяна 9-58",
                            AddressArm = "Երևան, Թումանյան փող. 9-58",

                            CityEng = "Yerevan",
                            CityRus = "Ереван",
                            CityArm = "Երևան",


                            Latitude = 44.520831,
                            Longitude = 40.179770,

                            PhoneNumber = "37491527822",



                            DescriptionEng = "“Old Beijing: CHINESE CUISINE” is one of the best Chinese restaurants in Yerevan. Since 2009, IMG_9807 from the very moment of its establishment our restaurant has become one of the loveliest places for many people. We suggest more than two hundred various dishes of Chinese Cuisine wide selection of beverages, refreshmentsIMG_9805 and exotic cocktails. The comfortable halls of the “Old Beijing: CHINESE CUISINE” decorated in Chinese style and best choice of music create a friendly environment for family events, dinners, business lunches or celebrations. Highly qualified staff of the “Old Beijing: CHINESE CUISINE” is always ready to provide you necessary support and assistance. Welcome to the Old Beijing.",
                            DescriptionRus = "“Старый Пекин: КИТАЙСКАЯ КУХНЯ” один из лучших китайских ресторанов Еревана. Начиная с момента своего создания в 2009 IMG_9807 году ресторан стал одним из наиболее любимых заведений для многих людей. Мы предлогам более чем двести наименований различных блюд китайской кухни, а также большой выбор напитков и экзотических коктейлей. Комфортабельные залы IMG_9805 Старого Пекина, декорированные в китайском стиле и лучшая музыкальная подборка создают дружественную обстановку для семейных мероприятий, ужинов, деловых обедов и празднеств. Высококвалифицированный персонал ресторана “Старый Пекин: КИТАЙСКАЯ КУХНЯ” всегда готов предложить Вам необходимую помощь и содействие. Добро пожаловать в Старый Пекин.",
                            DescriptionArm = "«Հին Պեկին: ՉԻՆԱԿԱՆ ԽՈՀԱՆՈՑ»-ը Երևանի լավագույն չինական ռեստորաններից մեկն է:IMG_9807 Սկսած իր ստեծման պահից 2009 թվականին ռեստորանը շատերի համար դարձել է ամենասիրելի վայրերից մեկը: Մենք առաջարկում ենք չինական խոհանոցի առավել քան երկու հարյուր անվանում ճաշատեսակներ, ինչպես նաև ըմպելիքների և էկզոտիկ կոկտեյլների մեծ ընտրանի: IMG_9805 Չինական ոճով ձևավորված՝ Հին Պեկինի հարմարավետ սրահները և լավագույն երաժշտության ընտրանին ստեղծում են հյուրընկալ միջավայր ընտանեկան միջոցառումների, ընթրիկների, գործնական հանդիպումների և հանդիսությունների համար: «Հին Պեկին: ՉԻՆԱԿԱՆ ԽՈՀԱՆՈՑ»-ի բարձրակարգ անձնակազմը մշտապես պատրաստ է առաջարկելու Ձեզ անհրաժեշտ օգնությունն ու աջակցությունը: Բարի գալուստ Հին Պեկին:",

                            MainImgLink = "http://oldbeijing.am/wp-content/themes/pekin/images/slide1.jpg",


                            Category = RestaurantCategory.Restaurant,

                            ClosingTime = new TimeSpan(23, 00, 00),
                            OpeningTime = new TimeSpan(9, 00, 00),
                            AdditionalClosingTime = new TimeSpan(23, 00, 00),
                            AdditionalOpeningTime = new TimeSpan(9, 00, 00),

                            FeaturesID = featuresID,

                            CuisineEng = "Chinese",
                            CuisineRus = "Китайская",
                            CuisineArm = "Չինական",

                            Cost = "$$",

                            ServePrice = null,

                            IsMain = true,
                            Brand = "OldBeijing",

                            Images = new List<RestaurantImg>(BeijImgs),

                            Agent = admins[i - 1],
                        });
                        admins[i - 1].RestaurantID = restaurants[i - 1].ID;
                        break;

                    case 2:
                        restaurants.Add(new Restaurant
                        {
                            ID = i,

                            NameEng = "Cactus",
                            NameArm = "Կակտուս",
                            NameRus = "Кактус",


                            AddressEng = "Yerevan, 42 Mashtots ave.",
                            AddressRus = "Ереван, Пр. Маштоца 42",
                            AddressArm = "Երևան, Մաշտոցի Պող. 42",

                            CityEng = "Yerevan",
                            CityRus = "Ереван",
                            CityArm = "Երևան",

                            Longitude = 40.184931,
                            Latitude = 44.512891,

                            PhoneNumber = "37410539939",


                            DescriptionEng = "First Mexican restaurant in Yerevan. Having been once to Mexico, one wants to be back there again. The fierce heat of the sun, desert and prickly cactus are not the only highlights of the country; it has many wonders to offer, like the mariachi playing guitars, gorgeous dark beauties who entertain the local macho with their flamenco dances. Mexico is the country of tequila, most exotic cocktails, and inexperienced emotions. Having opened in May 2000, the first Mexican restaurant in Yerevan soon became one of the most frequented spots in the city. Over the years, the restaurant has hosted a variety of festivals, has won at several contests, thus earning a great reputation among the city residents, mass media, politicians, show businessmen and tourists.",
                            DescriptionRus = "Первый мексиканскый ресторан в Ереване. За почти десятилетнюю историю ресторан стал одним из самых посещаемых ресторанов в стране. За все эти годы наш ресторан пережил множество разных фестивалей,таких как Бразильский фестиваль, фестиваль Маргариты, Супный фестиваль, Коктейльный и Футбольный фестивали, День текилы, День независимости Мексики (”Синко де Майо), Кубинский фестиваль коктейлей, Сигарный фестиваль, Пивной фестиваль ”Октоберфест”и много много других. Также ”Кактус” принимал участие и побеждал во многих конкурсах, удостаивался только самых лучших и теплых отзывов, как со стороны жителей города, средств массовой информации, политиков, звезд шоу-бизнеса, так и со стороны туристов.",
                            DescriptionArm = "Առաջին մեքսիկական ռեստորանը Երևանում: Բավական է մեկ անգամ այցելեք Մեքսիկա, և դուք անպայման կցանկանաք կրկին վերադառնալ այնտեղ: Արևի տապը, անապատը և փշոտ կակտուսներն այս երկրի միակ գրավչությունը չեն: Այն շատ ու շատ հրաշքներ կարող է առաջարկել: Կարող եք վայելել մարիաչիների կիթառի սիրառատ ելևէջները, ինչպես նաև թխամորթ գեղեցկուհիների ֆլամենկո պարը: Մեքսիկան տեկիլայի, էկզոտիկ կոկտեյլների և աննկարագրելի հույզերի երկիր է: 2000 թվականի մայիսին Երևանում մեքսիկական առաջին ռեստորանը, իր դռները բացելով հանրության առաջ, շատ արագ դարձավ քաղաքի ամենասիրված անկյուններից մեկը: Տարիների ընթացքում ռեստորանն իր հարկի տակ անց է կացրել բազմաթիվ և բազմաժանր միջոցառումներ, հաղթել է մի շարք մրցույթներում`իր արժանապատիվ տեղը գրավելով և մեծ համբավ վայելելով քաղաքի բնակիչների, զանգվածային լրատվամիջոցների, շոու բիզնեսի ներկայացուցիչների և զբոսաշրջիկների շրջանում:",

                            MainImgLink = "http://cactus.am/cabare/wp-content/uploads/2014/06/about-cactus-restaurant-550x590.jpg",

                            Category = RestaurantCategory.Restaurant,

							ClosingTime = new TimeSpan(23, 00, 00),
							OpeningTime = new TimeSpan(9, 00, 00),
							AdditionalClosingTime = new TimeSpan(23, 00, 00),
							AdditionalOpeningTime = new TimeSpan(9, 00, 00),

                            CuisineEng = "Mexican",
                            CuisineRus = "Мексиканская",
                            CuisineArm = "Մեքսիկական",

                            FeaturesID = featuresID,

                            Cost = "$$",

                            ServePrice = null,

                            Brand = "Cactus",

                            Agent = admins[i - 1],

                            IsMain = true,

                            Images = new List<RestaurantImg>(CactusImgs),
                        });
                        admins[i - 1].RestaurantID = restaurants[i - 1].ID;
                        break;

                    case 3:
                    default:
                        restaurants.Add(new Restaurant
                        {
                            ID = i,

                            NameEng = "Bellagio",
                            NameArm = "Bellagio",
                            NameRus = "Bellagio",

                            AddressEng = "Yerevan, 42 Mashtots ave.",
                            AddressRus = "Ереван, Пр. Маштоца 42",
                            AddressArm = "Երևան, Մաշտոցի Պող. 42",

                            CityEng = "Yerevan",
                            CityRus = "Ереван",
                            CityArm = "Երևան",

                            Longitude = 40.192239,
                            Latitude = 44.533055,

                            PhoneNumber = "37410551212",



                            DescriptionEng = "The hotel-restaurant complex Bellagio has a 8-year history. Recently we've inaugurated the second complex of Bellagio in village Ptghni, nearly fifteen minutes journey from the center of Yerevan. Here we'll help you organise ceremonies of various types - from magnificent weddings, birthdays to corporate parties, business lunch, etc. We are open for 24 hours/7 days a week. Besides the hotel and restaurant, here we have also splendid saunas.",
                            DescriptionRus = "Гостинично-ресторанный комплекс Белладжио действует вот уже 8 лет. Недавно открылся второй комплекс Белладжио в пятнадцати минутах езды от центра Еревана, в деревне Птхни. У нас организовывают мероприятия самых разных типов, начиная от самых роскошных свадеб, дней рождения, крещений, до больших и малых корпоративных вечеринок, деловых ужинов и т.д. Наши двери открыты для наших гостей 24 часа в сутки, 7 дней в неделю. Кроме гостиницы и ресторана действуют также роскошные сауны.",
                            DescriptionArm = "Հյուրանոցա-ռեստորանային համալիր «Բելլաջիո»-ն գործում է արդեն 9 տարի: Վերջերս բացվել է «Բելլաջիո»-ի երկրորդ համալիրը , որը գտնվում է Երևանի կենտրոնից տասնհինգ րոպե հեռավորության վրա` գյուղ Պտղնիում: Մենք կազմակերպում ենք ամենատարբեր տեսակի միջոցառումներ՝ սկսած շքեղ հարսանեկան արարողություններից, ծննդյան տոներից, կնունքներից, մինչև մեծ ու փոքր կորպորատիվ երեկույթներ, գործնական ընթրիքներ և այլն: Մեր դռները բաց են մեր հյուրերի համար օրը 24 ժամ, շաբաթը 7 օր: Բացի հյուրանոցից և ռեստորանից` գործում են նաև շքեղ սաունաներ:",

                            MainImgLink = "http://www.bellagio.am/am/img/gallery/gallery1.jpg",


                            Category = RestaurantCategory.Restaurant,


							ClosingTime = new TimeSpan(23, 00, 00),
							OpeningTime = new TimeSpan(9, 00, 00),
							AdditionalClosingTime = new TimeSpan(23, 00, 00),
							AdditionalOpeningTime = new TimeSpan(9, 00, 00),

                            CuisineEng = "Armenian",
                            CuisineRus = "Армянская",
                            CuisineArm = "Հայկական",

                            Cost = "$$$",

                            FeaturesID = featuresID,

                            ServePrice = null,



                            Brand = "Bellagio",

                            Agent = admins[i - 1],

                            IsMain = true,
                            Images = new List<RestaurantImg>(BelImgs),
                        });
                        admins[i - 1].RestaurantID = restaurants[i - 1].ID;
                        break;
                }
            }

            await _dbContext.Restaurants.AddRangeAsync(restaurants);
            await _dbContext.SaveChangesAsync();

            #endregion Restaurants           
        }

        #endregion Public Methods

        #region Seed Methods

        private async Task CreateUsersAsync()
        {
            string role_Agent = Constants.ApplicationRoles.AGENT_ROLE;
            string role_Client = Constants.ApplicationRoles.CLIENT_ROLE;

            //Create Roles (if they doesn't exist yet)
            if (!await _roleManager.RoleExistsAsync(role_Agent))
            {
                await _roleManager.CreateAsync(new IdentityRole(role_Agent));
            }
            if (!await _roleManager.RoleExistsAsync(role_Client))
            {
                await _roleManager.CreateAsync(new IdentityRole(role_Client));
            }


            #region Agents                      

            Random rand = new Random();

            for (int i = 1; i <= 100; ++i)
            {
                int r = rand.Next(1, 4);
                string j = i.ToString();

                ApplicationUser adminToAdd;

                switch (r)
                {
                    case 1:
                        adminToAdd = new ApplicationUser
                        {
                            UserName = "Admin" + j,
                            Email = "Admin" + j + "@gmail.com",
                            EmailConfirmed = true,
                            FirstName = "Susanna",
                            LastName = "Sargsyan",
                            City = "Erevan",
                            Sex = false,
                        };
                        break;

                    case 2:
                        adminToAdd = new ApplicationUser
                        {
                            UserName = "Admin" + j,
                            Email = "Admin" + j + "@gmail.com",
                            EmailConfirmed = true,
                            FirstName = "Armine",
                            LastName = "Martirosyan",
                            City = "Erevan",
                            Sex = false,
                        };
                        break;

                    case 3:
                    default:
                        adminToAdd = new ApplicationUser
                        {
                            UserName = "Admin" + j,
                            Email = "Admin" + j + "@gmail.com",
                            EmailConfirmed = true,
                            FirstName = "Ani",
                            LastName = "Boolyan",
                            City = "Erevan",
                            Sex = false,
                        };
                        break;
                }

                if (!(await _dbContext.Users.AnyAsync(u => u.UserName == adminToAdd.UserName)))
                {
                    await _userManager.CreateAsync(adminToAdd, "Pass4Admin");
                    await _userManager.AddToRoleAsync(adminToAdd, role_Agent);
                    // Remove Lockout and E-Mail confirmation.
                    adminToAdd.EmailConfirmed = true;
                    adminToAdd.LockoutEnabled = false;
                }

                admins.Add(adminToAdd);
            }

            #endregion Agents

            #region Users


            for (int i = 1; i < 5; ++i)
            {
                var userToAdd = new ApplicationUser
                {
                    UserName = "User" + (8 + i).ToString(),
                    Email = "email" + (8 + i).ToString() + "@gmail.com",
                    FirstName = "User " + (8 + i).ToString(),
                    LastName = "User " + i.ToString()
                };

                if (!(await _dbContext.Users.AnyAsync(u => u.UserName == userToAdd.UserName)))
                {
                    await _userManager.CreateAsync(userToAdd, "MyP@assword1");
                    await _userManager.AddToRoleAsync(userToAdd, role_Client);
                }

                users.Add(userToAdd);
            };

            #endregion Users


            await _dbContext.SaveChangesAsync();
        }

        #endregion Seed Methods
    }
}

