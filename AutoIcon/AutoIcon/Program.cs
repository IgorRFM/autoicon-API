using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using CloudinaryDotNet;


var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

var cloudinarySettings = builder.Configuration.GetSection("Cloudinary");
var cloudName = cloudinarySettings["CloudName"];
var apiKey = cloudinarySettings["ApiKey"];
var apiSecret = cloudinarySettings["ApiSecret"];

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddSingleton(sp =>
{
    var config = builder.Configuration.GetSection("Cloudinary").Get<CloudinarySettings>();
    var account = new Account(config.CloudName, config.ApiKey, config.ApiSecret);
    return new Cloudinary(account);
});


// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5, // até 5 requisições
            Window = TimeSpan.FromMinutes(1440),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 2 // fila de espera (opcional)
        });
    });

    options.RejectionStatusCode = 429; // Too Many Requests
});



builder.Services.AddControllers();

var app = builder.Build();

app.UseCors();
app.UseRateLimiter(); // Habilita o rate limiting
app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public class CloudinarySettings
{
    public string CloudName { get; set; }
    public string ApiKey { get; set; }
    public string ApiSecret { get; set; }
}

