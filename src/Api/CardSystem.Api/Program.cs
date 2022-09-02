using System.Text;
using CardSystem.Api.Options;
using CardSystem.Api.Services;
using CardSystem.Communication.Abstract;
using CardSystem.Communication.Concrete;
using CardSystem.Communication.Options;
using CardSystem.DataAccess.Abstract;
using CardSystem.DataAccess.Concrete;
using CardSystem.Domain.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseLazyLoadingProxies();
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AuthOptions:Secret"])),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });
builder.Services.AddControllers();
builder.Services.AddScoped(typeof(IAsyncEntityRepository<,>), typeof(AsyncEntityRepository<,>));
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IEmailSender, MockEmailSender>();
builder.Services.Configure<AuthOptions>(builder.Configuration.GetRequiredSection(AuthOptions.ConfigSection));
builder.Services.Configure<EmailOptions>(builder.Configuration.GetRequiredSection(EmailOptions.ConfigSection));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();