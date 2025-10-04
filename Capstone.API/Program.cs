using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "Capstone API", Version = "v1" });

	// Add JWT Bearer support
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			Array.Empty<string>()
		}
	});
});

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
		policy.AllowAnyOrigin()
			  .AllowAnyMethod()
			  .AllowAnyHeader());
});

builder.Services.AddSingleton<IMongoClient>(sp =>
{
	var connectionString = builder.Configuration.GetConnectionString("MongoDb")
						  ?? "mongodb://localhost:27017"; // fallback if not set
	return new MongoClient(connectionString);
});

builder.Services.AddAuthentication("Bearer")
	.AddJwtBearer("Bearer", options =>
	{
		var jwtSettings = builder.Configuration.GetSection("Jwt");
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = jwtSettings["Issuer"],
			ValidAudience = jwtSettings["Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
		};
	});

builder.Services.AddHttpClient();

builder.Services.AddHostedService<NytTravelStoryService>();

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

app.UseCors("AllowAll");

// allows serving of static files
app.UseStaticFiles(new StaticFileOptions
{
	// API URL can't seem to communicate .avif files, trying this...
	ContentTypeProvider = new FileExtensionContentTypeProvider
	{
		Mappings =
		{
			[".avif"]="image/avif"
		}
	}
});

app.Run();
