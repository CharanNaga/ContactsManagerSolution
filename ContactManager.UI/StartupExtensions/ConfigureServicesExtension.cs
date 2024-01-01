using ContactManager.Core.Domain.IdentityEntities;
using CRUDExample.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

namespace CRUDExample
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services,IConfiguration configuration)
        {
            //Use ctrl + H key and replace all "builder.Services" names with "services"

            //add ResponseHeaderActionFilter as a service
            services.AddTransient<ResponseHeaderActionFilter>();

            //adds controllers & views as services
            services.AddControllersWithViews(options =>
            {
                //options.Filters.Add<ResponseHeaderActionFilter>(5); //set as global filter but it won't accept parameters other than order
                var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();
                //options.Filters.Add(new ResponseHeaderActionFilter("CustomKey-FromGlobal","CustomValue-FromGlobal",2));

                options.Filters.Add(new ResponseHeaderActionFilter(logger)
                {
                    Key = "CustomKey-FromGlobal",
                    Value = "CustomValue-FromGlobal",
                    Order = 2
                });
            });

            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<IPersonsRepository, PersonsRepository>();

            services.AddScoped<ICountriesGetterService, CountriesGetterService>();
            services.AddScoped<ICountriesAdderService, CountriesAdderService>();
            services.AddScoped<ICountriesUploaderService, CountriesUploaderService>();

            //services.AddScoped<IPersonsGetterService, PersonsGetterService>();

            //1. Open closed principle using Interfaces.
            services.AddScoped<IPersonsGetterService, PersonsGetterServiceWithFewExcelFields>(); //Added GetterService with the new functionality into IoC container. So in the client classes, FewExcelFields implementation would be available

            //2. Open closed principle using Inheritance.
            //services.AddScoped<IPersonsGetterService, PersonsGetterServiceChild>(); //Added GetterService with the new functionality into IoC container. So in the client classes, PersonsGetterServiceChild implementation would be available

            services.AddScoped<PersonsGetterService, PersonsGetterService>(); //when any class asks object for PersonsGetterService, provide object of PersonsGetterService as an argument.
            services.AddScoped<IPersonsAdderService, PersonsAdderService>();
            services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
            services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
            services.AddScoped<IPersonsSorterService, PersonsSorterService>();

            //adding DbContext as a service
            services.AddDbContext<ApplicationDbContext>( //by default scoped service.
                options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                });

            //adding PersonsListActionFilter as a service
            services.AddTransient<PersonsListActionFilter>();

            //adding Identity as a service to IoC container
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>  //for creating users, roles tables
            {
                options.Password.RequiredLength = 5; //min length of password is 5 chars
                options.Password.RequireNonAlphanumeric = false; //may or mayn't contain atleast one non alphanumeric value
                options.Password.RequireUppercase = false; //may or mayn't contain atleast one uppercase letter
                options.Password.RequireLowercase = true; //must and should contain atleast one lowercase letter
                options.Password.RequireDigit = false; //may or mayn't contain atleast one digit
                options.Password.RequiredUniqueChars = 3; //Contain atleast 3 distinct chars. Eg:- CH123CH contains 5 distinct chars 'C','H','1','2','3'
            }) 
                .AddEntityFrameworkStores<ApplicationDbContext>()  //creating tables using IdentityDbContext overall in entire application
                .AddDefaultTokenProviders() //Generating tokens at runtime randomly while Email & phone number verifications, forgot or resetting passwords
                .AddUserStore<
                    UserStore<ApplicationUser,ApplicationRole,ApplicationDbContext,Guid>
                    >()  //configuring repository layer for users table i.e., users store
                .AddRoleStore<
                    RoleStore<ApplicationRole,ApplicationDbContext,Guid>    
                    >(); //configuring repository layer for roles table i.e., roles store

            //adding HttpLogging as a service
            services.AddHttpLogging(options =>
            {
                options.LoggingFields = HttpLoggingFields.RequestProperties
                | HttpLoggingFields.ResponsePropertiesAndHeaders;
            });
            return services;
        }
    }
}
