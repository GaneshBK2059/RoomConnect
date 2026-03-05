using RoomConnect.Infrastructure.Database;
using RoomConnect.Infrastructure.Repositories;
using RoomConnect.Application.Services;
using RoomConnect.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register database factory
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

// Register repositories
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<RoomRepository>();
builder.Services.AddScoped<BookingRepository>();
builder.Services.AddScoped<ReviewRepository>();
builder.Services.AddScoped<RoomImagesRepository>();
builder.Services.AddScoped<RefreshTokenRepository>();

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<DashboardService>();

// Add JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "THIS_IS_SUPER_SECRET_KEY_123456";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "RoomConnect";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "RoomConnectUsers";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero // Tokens expire exactly at token expiration time (instead of 5 minutes later)
        };
    });

builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Use CORS
app.UseCors("AllowAll");

// Use Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
