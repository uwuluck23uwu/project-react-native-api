global using ClassLibrary.Models.Dto;
global using ClassLibrary.Models.Data;
global using ClassLibrary.Models.Response;
global using ClassLibrary.Models.Settings;
global using ProjectReactNative.Data;
global using ProjectReactNative.Hubs;
global using ProjectReactNative.Helpers;
global using ProjectReactNative.Services.IServices;

using System.Text;
using System.Reflection;
using Stripe;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ProjectReactNative.Helpers.Configures;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Settings
var key = builder.Configuration.GetValue<string>("Settings:SecretProgram");
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// ---- added: Stripe binding + API key ----
builder.Services.Configure<StripeSettings>(
    builder.Configuration.GetSection("Stripe")
);
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
// -----------------------------------------

// Controllers + JSON
builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    opt.JsonSerializerOptions.WriteIndented = true;
});

// อัปโหลดไฟล์ใหญ่
builder.Services.Configure<FormOptions>(opts =>
{
    opts.MultipartBodyLengthLimit = 100 * 1024 * 1024;
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", cors =>
    {
        cors.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Auth (JWT)
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
        ValidAudience = jwtSettings.GetValue<string>("Audience"),
        ClockSkew = TimeSpan.Zero,
    };
});

// AutoMapper/Swagger/SignalR
builder.Services.AddAutoMapper(typeof(MappingConfigure));
builder.Services.AddResponseCaching();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

// Autofac: auto-register *Service
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(containerBuilder =>
{
    containerBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
        .Where(t => t.Name.EndsWith("Service"))
        .AsImplementedInterfaces()
        .InstancePerLifetimeScope();
}));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseResponseCaching();

// Map Controllers & Hub
app.MapControllers();
app.MapHub<SignalHub>("/hubs/signal");

app.Run();
