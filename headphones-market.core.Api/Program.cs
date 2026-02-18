using headphones_market.core.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Read connection string and fail fast if missing
var connectionString = "Server=DESKTOP-EBQC7D8;Database=TestDb;Trusted_Connection=True;TrustServerCertificate=True;"; //builder.Configuration.GetConnectionString("Server=DESKTOP-EBQC7D8;Database=TestDb;Trusted_Connection=True;TrustServerCertificate=True;");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured. Add it to appsettings.json, environment variables, or User Secrets.");
}

// Configure EF Core with SQL Server
builder.Services.AddDbContext<HeadphonesDbContext>(options =>
    options.UseSqlServer(connectionString)
           .EnableSensitiveDataLogging()   // shows parameter values; remove in production
           .LogTo(Console.WriteLine));     // writes SQL to console during dev

// Register EF-based repositories and existing service
builder.Services.AddScoped<IHeadphoneRepository, EfHeadphoneRepository>();
builder.Services.AddScoped<IKeyboardRepository, EfKeyboardRepository>();
builder.Services.AddScoped<headphones_market.core.Api.Services.IHeadphoneService, headphones_market.core.Api.Services.HeadphoneService>();

// MediatR registration
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
