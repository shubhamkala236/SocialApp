using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PostService.Application.Interfaces;
using PostService.Application.Settings;
using PostService.Infrastructure.Consumers;
using PostService.Infrastructure.Context;
using PostService.Infrastructure.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<PostDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPostService, PostServices>();
builder.Services.AddScoped<IPhotoService, PhotoService>();

builder.Services.Configure<CloudinarySettings>(
	builder.Configuration.GetSection("Cloudinary"));

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

//builder.Services.AddMassTransit(x =>
//{
//	x.UsingRabbitMq((ctx, cfg) =>
//	{
//		cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", h =>
//		{
//			h.Username(builder.Configuration["RabbitMQ:Username"]!);
//			h.Password(builder.Configuration["RabbitMQ:Password"]!);
//		});

//		cfg.ConfigureEndpoints(ctx);
//	});
//});

builder.Services.AddMassTransit(x =>
{
	x.AddConsumer<UserAvatarUpdatedConsumer>();

	x.UsingRabbitMq((ctx, cfg) =>
	{
		cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", h =>
		{
			h.Username(builder.Configuration["RabbitMQ:Username"]!);
			h.Password(builder.Configuration["RabbitMQ:Password"]!);
		});

		cfg.ReceiveEndpoint("post-avatar-updated-queue", e =>
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
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider
		.GetRequiredService<PostDbContext>(); // change per service
	await db.Database.MigrateAsync(); // ✅ auto migrate on startup
}


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
