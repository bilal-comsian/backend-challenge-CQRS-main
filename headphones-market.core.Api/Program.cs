
using headphones_market.core.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register repository (single registration)
builder.Services.AddSingleton<IHeadphoneRepository, JsonHeadphoneRepository>();

// If you still use HeadphoneService elsewhere, keep this registration; otherwise you can remove it.
builder.Services.AddScoped<headphones_market.core.Api.Services.IHeadphoneService, headphones_market.core.Api.Services.HeadphoneService>();

// Correct MediatR registration for recent versions
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
