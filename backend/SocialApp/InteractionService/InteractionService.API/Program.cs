using System.Text;
using InteractionService.Application.Interfaces;
using InteractionService.Infrastructure.Consumers;
using InteractionService.Infrastructure.Context;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// ── Database ──────────────────────────────────────
builder.Services.AddDbContext<InteractionDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Services ──────────────────────────────────────
builder.Services.AddScoped<IInteractionService,
	InteractionService.Infrastructure.Services.InteractionService>();

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
	x.AddConsumer<PostDeletedConsumer>();
	x.AddConsumer<UserDeletedConsumer>();

	x.UsingRabbitMq((ctx, cfg) =>
	{
		cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", h =>
		{
			h.Username(builder.Configuration["RabbitMQ:Username"]!);
			h.Password(builder.Configuration["RabbitMQ:Password"]!);
		});

		// ── Receive Endpoints ─────────────────────
		cfg.ReceiveEndpoint("interaction-post-deleted-queue", e =>
		{
			e.ConfigureConsumer<PostDeletedConsumer>(ctx);
		});

		cfg.ReceiveEndpoint("interaction-user-deleted-queue", e =>
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

app.UseCors("AllowGateway");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();