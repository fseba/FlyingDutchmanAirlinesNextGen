using Microsoft.EntityFrameworkCore;

using FlyingDutchmanAirlines.InfrastuctureLayer;
using FlyingDutchmanAirlines.BusinessLogicLayer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
  .AddDbContext<IFlyingDutchmanAirlinesContext, FlyingDutchmanAirlinesContext>()
  .AddTransient<IFlightService, FlightService>()
  .AddTransient<IBookingService, BookingService>()
  .AddTransient<IFlightRepository, FlightRepository>()
  .AddTransient<IBookingRepository, BookingRepository>()
  .AddTransient<ICustomerRepository, CustomerRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
      options.SwaggerEndpoint("/swagger/v1/swagger.json", "Flying Dutchman Airlines");
      options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
