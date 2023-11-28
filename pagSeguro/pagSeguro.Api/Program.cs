using Microsoft.EntityFrameworkCore;
using pagSeguro.Api.Helpers;
using pagSeguro.Api.Services;
using pagSeguro.Api.Integrations;
using Microsoft.AspNetCore.Authentication;
using pagSeguro.Api.Authentication;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Basic", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Basic",
        In = ParameterLocation.Header,
        Description = "Basic Authentication Header"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Basic"
                }
            },
            new string[] { "Basic" }
        }
    });
});

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

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                         policy =>
                         {
                             policy.WithOrigins("https://localhost:7204", 
                                 "https://www.editoracontracorrente.com.br",
                                 "https://contracorrente-ecomm.webflow.io");
                             policy.AllowAnyHeader();
                         });
});

builder.Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(
    "Basic", null);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();

app.Run();
