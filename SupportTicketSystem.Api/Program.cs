using Microsoft.EntityFrameworkCore;
using SupportTicketSystem.Api.ExceptionHandler;
using SupportTicketSystem.Core.Interfaces;
using SupportTicketSystem.Core.Interfaces.IAgent;
using SupportTicketSystem.Core.Interfaces.ICustomer;
using SupportTicketSystem.Core.Interfaces.ITicket;
using SupportTicketSystem.Infrastructure.Context;
using SupportTicketSystem.Infrastructure.Data;
using SupportTicketSystem.Infrastructure.Helper;
using SupportTicketSystem.Infrastructure.Services;
using System.Text.Json.Serialization;
using SupportTicketSystem.Core.Exceptions;

var builder = WebApplication.CreateBuilder(args);

//Googled about this , it is to make sure that the enums are serialized as strings instead of numbers in the JSON response. This is useful for better readability and understanding of the API responses.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });    

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    string conn = !String.IsNullOrEmpty(builder.Configuration.GetConnectionString("DefaultConnection"))
                            ? builder.Configuration.GetConnectionString("DefaultConnection")!
                            : throw new AppException(404, "Failed to connect DB. Connection string missing.");

    options.UseSqlServer(conn);
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<IAdoRepo, AdoRepo>();
builder.Services.AddScoped<IHelperRepo, HelperRepo>();

builder.Services.AddScoped<IEfCoreCustomersRepo, EfCoreCustomersRepo>();
builder.Services.AddScoped<IEfCoreTicketsRepo, EfCoreTicketsRepo>();
builder.Services.AddScoped<IEfCoreAgentsRepo, EfCoreAgentsRepo>();

builder.Services.AddScoped<ITicketsService, TicketsService>();
builder.Services.AddScoped<ICustomersService, CustomersService>();
builder.Services.AddScoped<IAgentsService, AgentsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();
