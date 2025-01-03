using Autofac.Core;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using SocialService.Configurations;
using SocialService.Data;
using SocialService.Interfaces;
using SocialService.Models.InstagramModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<XApiSettings>(builder.Configuration.GetSection("XApiSettings"));
builder.Services.AddHttpClient<XApiService>();
builder.Services.Configure<InstagramApiSettings>(builder.Configuration.GetSection("InstagramApiSettings"));
builder.Services.Configure<SocialService.Models.InstagramModels.User>(builder.Configuration.GetSection("InstagramMockUser"));
builder.Services.AddSingleton<InstagramService>();
builder.Services.AddSingleton<IRestClientWrapper>(provider => new RestClientWrapper("https://graph.instagram.com/v21.0"));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
builder.Services.AddDbContext<DbContextApplication>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


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
