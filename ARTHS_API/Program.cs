using ARTHS_API.Configurations;
using ARTHS_Data.Entities;
using ARTHS_Data.Mapping;
using ARTHS_Utility.Settings;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("AppSetting"));
builder.Services.AddDbContext<ARTHS_DBContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        //options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; // Dòng này sẽ ẩn các trường có giá trị null
    }
);
builder.Services.AddSwaggerGenNewtonsoftSupport();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyHeader();
                          policy.AllowAnyMethod();
                          policy.WithOrigins(
                              "http://127.0.0.1:5173");
                          //    "http://localhost:51509",
                          //    "https://egift-d50fc.web.app",
                          //"https://egiftadmin-add56.web.app");
                          policy.AllowCredentials();
                      });
});

builder.Services.AddHangfireServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddDependenceInjection();
builder.Services.AddAutoMapper(typeof(GeneralProfile));

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

//--------------------
app.UseJwt();

app.AddHangfireDashboard();
// Đăng ký các công việc định kỳ của Hangfire
var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
app.Services.AddHangfireJobs(recurringJobManager);



app.UseExceptionHandling();

//--------------------


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
