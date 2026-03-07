using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserService.Application;
using UserService.Application.Interfaces;
using UserService.Infrastructure.Consumers;
using UserService.Infrastructure.Context;
using UserService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────
builder.Services.AddDbContext<UserDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Cloudinary ────────────────────────────────────
builder.Services.Configure<CloudinarySettings>(
	builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddScoped<IPhotoService, PhotoService>();

// ── Services ──────────────────────────────────────
builder.Services.AddScoped<IUserService, UserService.Infrastructure.Services.UserService>();

// ── JWT ───────────────────────────────────────────
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
		};
	});

builder.Services.AddMassTransit(x =>
{
	// ── Register Consumers ────────────────────────
	x.AddConsumer<UserRegisteredConsumer>();
	x.AddConsumer<UserDeletedConsumer>();

	x.UsingRabbitMq((ctx, cfg) =>
	{
		cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", h =>
		{
			h.Username(builder.Configuration["RabbitMQ:Username"]!);
			h.Password(builder.Configuration["RabbitMQ:Password"]!);
		});

		// ── Receive Endpoints ─────────────────────
		cfg.ReceiveEndpoint("user-registered-queue", e =>
		{
			e.ConfigureConsumer<UserRegisteredConsumer>(ctx);
		});

		cfg.ReceiveEndpoint("user-deleted-queue", e =>
		{
			e.ConfigureConsumer<UserDeletedConsumer>(ctx);
		});

		cfg.ConfigureEndpoints(ctx);
	});
});


builder.Services.AddAuthorization();

// ── CORS ──────────────────────────────────────────
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowGateway", policy =>
		policy.WithOrigins("https://localhost:5000")
			  .AllowAnyHeader()
			  .AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/openapi/v1.json", "v1");
	});
}

app.UseSwaggerUI();
app.UseCors("AllowGateway");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();