using System.Text.Json;
using Offers.CleanArchitecture.Api.Middlewares;
using Offers.CleanArchitecture.Application.Utilities;
using Offers.CleanArchitecture.Infrastructure.BackGroundServices;
using Offers.CleanArchitecture.Infrastructure.Data;
using Offers.CleanArchitecture.Infrastructure.Hubs;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("~\\Logging\\Log.json",rollingInterval : RollingInterval.Day)
    .MinimumLevel.Warning()
    .WriteTo.Console()
    .MinimumLevel.Warning()
    .CreateLogger();

//var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://192.168.1.110:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});



builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddWebServices();



//builder.Services.AddControllers();
builder.Services.AddControllers()// we add this to convert request and response to Json camelCase
    .AddJsonOptions(option => 
    {
        option.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        option.JsonSerializerOptions.Converters.Add(new GlossaryConverter());// not used but we will keep it in Application
    }
    );

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();

}
else
{
    await app.InitialiseDatabaseAsync();

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
await app.InitializeQuartz();

app.UseCors("AllowSpecificOrigin");
app.UseRouting();
app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseAuthorization();

app.UseMiddleware<TokenVersionMiddleware>();

app.MapControllers();

app.MapHub<NotificationHub>("hubs/notificationHub");
// this hup may used to know the total user connected and total views, but we don't use it right now
app.MapHub<TestHub>("hubs/testHub");

app.Run();
