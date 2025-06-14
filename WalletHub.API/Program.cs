using WalletHub.API.BackgroundServices;
using WalletHub.API.Caching;
using WalletHub.API.Data;
using WalletHub.API.Extensions;
using WalletHub.API.Interfaces;
using WalletHub.API.Middlewares;
using WalletHub.API.Models;
using WalletHub.API.Repository;
using WalletHub.API.Service;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using WalletHub.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddControllers().AddNewtonsoftJson(options => {
    options.SerializerSettings.ReferenceLoopHandling = 
    Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

builder.Services.AddDbContext<ApplicationDBContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddCache(builder.Configuration);

builder.Services.AddSwaggerSetup();

builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddRepositories();

builder.Services.AddServices();
//builder.Services.AddHttpClient();

builder.Services.AddHttpClient<ExportDataService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5018/");
    client.Timeout = TimeSpan.FromSeconds(30);
});


builder.Services.AddDecorators();


builder.Services.AddFluentValidation();

builder.Services.AddHangfire(builder.Configuration);

builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration.GetSection("AuthMessageSenderOptions"));
builder.Services.AddTransient<IEmailSenderService, EmailSenderService>();


var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHangfireDashboard(); 

builder.Services.RegisterRecurringJobs();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
} 

app.UseHttpsRedirection();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
    .SetIsOriginAllowed(origin => true));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();   

app.Run();

