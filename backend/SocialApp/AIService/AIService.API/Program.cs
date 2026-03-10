using AIService.Application.Interfaces;
using AIService.Infrastructure.Plugins;
using AIService.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SemanticKernel;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── Semantic Kernel with Ollama ───────────────────
#pragma warning disable SKEXP0070
builder.Services.AddOllamaTextGeneration(
	modelId: builder.Configuration["Ollama:ModelId"]!,
	endpoint: new Uri(builder.Configuration["Ollama:Endpoint"]!));
#pragma warning restore SKEXP0070

builder.Services.AddKernel()
	.Plugins.AddFromType<PostPlugin>("PostPlugin");

// ── Services ──────────────────────────────────────
builder.Services.AddScoped<IPostAssistantService, PostAssistantService>();

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

app.UseCors("AllowGateway");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();