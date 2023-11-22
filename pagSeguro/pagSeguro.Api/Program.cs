using Microsoft.EntityFrameworkCore;
using pagSeguro.Api.Helpers;
using pagSeguro.Api.Services;
using pagSeguro.Api.Integrations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DI
builder.Services.AddScoped<ISettingService, SettingService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPagSeguroService, PagSeguroService>();

// Memory Cache
builder.Services.AddMemoryCache();

string conn = builder.Configuration.GetConnectionString("pagSeguroApiDb");

builder.Services.AddDbContext<DataContext>(
    options => options.UseSqlServer(conn));

builder.Services.AddCors();

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
