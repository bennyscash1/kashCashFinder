using KashCashFinderBe.Data;
using KashCashFinderBe.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// CORS – allow the Vue dev server to call this API
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendCorsPolicy", policy =>
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173",
                "http://localhost:5174",
                "https://localhost:5174")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Database
var connectionString = builder.Configuration.GetConnectionString("KashCashFinderDb");
builder.Services.AddDbContext<WhereCanBuyDbContext>(options =>
    options.UseSqlServer(connectionString));

// Application services
builder.Services.AddScoped<ISearchService, SearchService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("FrontendCorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
