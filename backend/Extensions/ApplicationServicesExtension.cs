using backend.Base;
using backend.BussinessLogic;
using backend.Controllers;
using backend.Dao;
using backend.Helper;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using webapi.Dao.Repository;
using webapi.Dao.UnitofWork;
using webapi.Data;

namespace backend.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IUnitofWork, UnitofWork>();
            services.AddSingleton<Hashtable>();
            services.AddTransient<CategoryBusinessLogic>();
            services.AddTransient<RestaurantBusinessLogic>();
            services.AddTransient<HotelBusinessLogic>();
            services.AddTransient<LocationBusinessLogic>();
            services.AddTransient<TransportationBusinessLogic>();
            services.AddTransient<ResortBusinessLogic>();
            services.AddTransient<FeedBackBusinessLogic>();
            services.AddTransient<OrderBusinessLogic>();
            services.AddTransient<TourBusinessLogic>();
            services.AddTransient<ItineraryBusinessLogic>();
            services.AddTransient<OrderDetailBusinessLogic>();
            services.AddTransient<ServiceBusinessLogic>();
            services.AddTransient<StaffBusinessLogic>();
            services.AddTransient<PaymentBussinessLogic>();
            services.AddHttpClient<PaymentPayPalController>();
            services.AddTransient<TourDetailBusinessLogic>();
            services.AddTransient<UserBussinessLogic>();
            services.AddTransient<ImageService>();
            services.AddTransient<RoleBusinessLogic>();
            services.AddTransient<InformationBusinessLogic>();
            services.AddTransient<ImageService>();
            services.AddTransient<Search_TourDetail_Dao>();
            services.AddTransient<Search_Tour_Dao>();
            services.AddTransient<Top_10_Tour_Dao>();
            services.AddAutoMapper(typeof(AutoMapper1));
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = acttionContext =>
                {
                    var errors = acttionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToArray();
                    var errorResponse = new APIValidationError
                    {
                        Errors = errors,
                        Message = "Validation failed"
                    };
                    return new BadRequestObjectResult(errorResponse);
                };
            });

            return services;
        }
    }
}
