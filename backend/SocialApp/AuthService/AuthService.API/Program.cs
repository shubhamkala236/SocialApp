using System.Text;
using AuthService.Application.Interface;
using AuthService.Infrastructure.Consumers;
using AuthService.Infrastructure.Context;
using AuthService.Infrastructure.Service;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

//Services
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AuthDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IAuthService, AuthServices>();

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
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
		};
	});

builder.Services.AddMassTransit(x =>
{
	x.AddConsumer<UserAvatarUpdatedConsumer>();  // ✅ add this

	x.UsingRabbitMq((ctx, cfg) =>
	{
		cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", h =>
		{
			h.Username(builder.Configuration["RabbitMQ:Username"]!);
			h.Password(builder.Configuration["RabbitMQ:Password"]!);
		});

		cfg.ReceiveEndpoint("auth-avatar-updated-queue", e =>
		{
			e.ConfigureConsumer<UserAvatarUpdatedConsumer>(ctx);
		});

		cfg.ConfigureEndpoints(ctx);
	});
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//Gateway CORS policy
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowGateway", policy =>
		policy.WithOrigins("https://localhost:5000")
			  .AllowAnyHeader()
			  .AllowAnyMethod());
});

var app = builder.Build();

if(app.Environment.IsDevelopment())
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
