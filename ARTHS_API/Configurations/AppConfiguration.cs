using ARTHS_API.Configurations.Middleware;
using ARTHS_Data;
using ARTHS_Data.Repositories.Implementations;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Implementations;
using ARTHS_Service.Interfaces;
using Hangfire;
using Hangfire.SqlServer;
using HangfireBasicAuthenticationFilter;
using Microsoft.OpenApi.Models;

namespace ARTHS_API.Configurations
{
    public static class AppConfiguration
    {
        public static void AddDependenceInjection(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<ITellerService, TellerService>();
            services.AddScoped<IOwnerService, OwnerService>();
            services.AddScoped<ICloudStorageService, CloudStorageService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IRepairServiceService, RepairServiceService>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<IMotobikeProductService, MotobikeProductService>();

            services.AddScoped<ICartService, CartService>();

            services.AddScoped<IWarrantyService, WarrantyService>();
            //services.AddScoped<IVNPayService, PaymentService>();
            services.AddScoped<IRevenueStoreService, RevenueStoreService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IDeviceTokenService, DeviceTokenService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IConfigurationService, ConfigurationService>();
            services.AddScoped<IWarrantyHistoryService, WarrantyHistoryService>();

            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddTransient<IGhnService, GhnService>();
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ARTHS Service Interface",
                    Description = @"APIs for Application to manage motorbikes accessories and repair business of Thanh Huy store in Ho Chi Minh City.
                        <br/>
                        <br/>
                        <strong>WebApp:</strong> <a href='https://webapp.thanhhuystore.com' target='_blank'>https://webapp.thanhhuystore.com</a>",
                    Version = "v1"
                });
                c.DescribeAllParametersInCamelCase();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Use the JWT Authorization header with the Bearer scheme. Enter 'Bearer' followed by a space, then your token.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,
                        },
                        new List<string>()
                      }
                 });

                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);
                c.EnableAnnotations();
            });
        }
        public static void UseJwt(this IApplicationBuilder app)
        {
            app.UseMiddleware<JwtMiddleware>();
        }

        public static void UseExceptionHandling(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
        }

        public static void AddHangfireDashboard(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/hangfire/job-dashboard", new DashboardOptions
            {
                DashboardTitle = "Hangfire Job of Thanh Huy motorbike server",
                DarkModeEnabled = true,
                Authorization = new[]
                {
                    new HangfireCustomBasicAuthenticationFilter
                    {
                        User = "admin.arths",
                        Pass = "admin.arths.@.@"
                    }
                }
            });
        }

        public static void AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    });
            });

            services.AddHangfireServer();
        }
        public static void AddHangfireJobs(this IServiceProvider serviceProvider, IRecurringJobManager recurringJobManager)
        {
            // Đăng ký công việc định kỳ với Hangfire sử dụng factory delegate
            recurringJobManager.AddOrUpdate(
                "SendMaintenanceReminders",
                () => serviceProvider.CreateScope().ServiceProvider.GetRequiredService<INotificationService>().CheckAndSendMaintenanceReminders(),
                "30 5 * * *"
            );
            recurringJobManager.AddOrUpdate(
                "CheckDiscontinuedDiscount",
                () => serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IDiscountService>().CheckDicounts(),
                "0 17 * * *"
            );
            
        }
    }
}
