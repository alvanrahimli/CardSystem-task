using CardSystem.Api.Options;
using CardSystem.Communication.Options;
using CardSystem.DataAccess.Abstract;
using CardSystem.DataAccess.Concrete;
using CardSystem.Domain.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseLazyLoadingProxies();
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddControllers();
builder.Services.AddScoped(typeof(IAsyncEntityRepository<,>), typeof(AsyncEntityRepository<,>));
builder.Services.Configure<AuthOptions>(builder.Configuration.GetRequiredSection(AuthOptions.ConfigSection));
builder.Services.Configure<EmailOptions>(builder.Configuration.GetRequiredSection(EmailOptions.ConfigSection));

var app = builder.Build();


app.Run();