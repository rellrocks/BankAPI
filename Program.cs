using BankAPI.Models;
using BankAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var MyAllowAnyOrigins = "_myAllowAnyOrigins";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 

builder.Services.AddDbContext<BankDBContext>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("BankDBConnection"), sqlServerOptions => sqlServerOptions.CommandTimeout(180))
);

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AccountService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowAnyOrigins,
                          builder =>
                          {
                              builder.AllowAnyOrigin()
                                     .AllowAnyHeader()
                                     .AllowAnyMethod();
                          });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseRouting();
app.UseCors(MyAllowAnyOrigins);
app.UseAuthorization();
app.MapControllers(); 

app.Run();
