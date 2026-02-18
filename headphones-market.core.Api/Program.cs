using headphones_market.core.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure EF Core with SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<HeadphonesDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register EF-based repositories and existing service
builder.Services.AddScoped<IHeadphoneRepository, EfHeadphoneRepository>();
builder.Services.AddScoped<IKeyboardRepository, EfKeyboardRepository>();
builder.Services.AddScoped<headphones_market.core.Api.Services.IHeadphoneService, headphones_market.core.Api.Services.HeadphoneService>();

// MediatR registration (unchanged)
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
