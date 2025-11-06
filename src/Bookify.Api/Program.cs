using Bookify.Api.Controllers.Bookings;
using Bookify.Api.Extensions;
using Bookify.Application;
using Bookify.Infrastructure;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
        options.ConfigObject.AdditionalItems["persistAuthorization"] = true;
    });

    app.ApplyMigrations();

    app.SeedData();
}

app.UseHttpsRedirection();

app.UseCustomExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBookingEndpoints();

app.Run();
