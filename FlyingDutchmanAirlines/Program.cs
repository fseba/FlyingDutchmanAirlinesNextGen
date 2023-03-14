using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.ServiceLayer;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("FlyingDutchmanDatabase");

builder.Services
  .AddDbContext<FlyingDutchmanAirlinesContext>(options => options.UseSqlServer(connectionString))
  //.AddTransient<FlyingDutchmanAirlinesContext, FlyingDutchmanAirlinesContext>()
  .AddTransient<FlightService, FlightService>()
  .AddTransient<BookingService, BookingService>()
  .AddTransient<FlightRepository, FlightRepository>()
  .AddTransient<BookingRepository, BookingRepository>()
  .AddTransient<CustomerRepository, CustomerRepository>()
  .AddTransient<AirportRepository, AirportRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(swagger =>
    {
      swagger.SwaggerEndpoint("/swagger/v1/swagger.json", "Flying Dutchman Airlines");
      swagger.SupportedSubmitMethods(new[] { SubmitMethod.Get, SubmitMethod.Post });
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
