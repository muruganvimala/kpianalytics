using BusinessIntelligence_API.Model;
using BusinessIntelligence_API.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using Microsoft.EntityFrameworkCore;
using System.Text;
using BusinessIntelligence_API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Serilog.Configuration;
using BusinessIntelligence_API.Repository;
using BusinessIntelligence_API.Controllers;
using BusinessIntelligence_API.Hubs;
using BusinessIntelligence_API.Mapping;
using AutoMapper;
using BusinessIntelligence_API.Middleware;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting.Internal;
using System.Diagnostics;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);
JTSContext _jTSContext = new JTSContext();
// Configure logging

//Log.Logger = new LoggerConfiguration()
//	.Enrich.FromLogContext()
//	.WriteTo.File("Logs/apilog-.txt",
//		rollingInterval: RollingInterval.Day,
//		outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}API Details: {Properties:j}{NewLine}{Exception}",
//		restrictedToMinimumLevel: LogEventLevel.Error) // Log only events that are Error or higher
//	.CreateLogger();


//Log.Logger = new LoggerConfiguration()
//		   .MinimumLevel.Information()
//		   .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
//		   .WriteTo.MSSqlServer(connectionString,
//			   tableName: "Bi_ApiCallLogs",
//			   autoCreateSqlTable: false,
//			   restrictedToMinimumLevel: LogEventLevel.Information)
//		   .CreateLogger();

//builder.Logging.ClearProviders();
//builder.Logging.AddSerilog();
//builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<JTSContext>();
builder.Services.AddScoped<IBiPerformanceMetricRepository, BiPerformanceMetricRepository>();
builder.Services.AddScoped<ISettingRepository, SettingRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserMasterRepository, UserMasterRepository>();
builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
builder.Services.AddScoped<ILogServiceRepository, LogService>();
builder.Services.AddScoped<RealTimeController>();
builder.Services.AddScoped<IForexRepository, ForexRepository>();
builder.Services.AddScoped<ICustomerDatumRepository, CustomerDatumRepository>(); // test commit
builder.Services.AddScoped<IDirectCostRepository, DirectCostRepository>();
builder.Services.AddScoped<IIndirectLabourCostRepository, IndirectLabourCostRepository>();
builder.Services.AddScoped<IOtherCostRepository, OtherCostRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IQMSFeedbackRepository, QMSFeedBackRepository>();
builder.Services.AddScoped<IQMSDataRepository, QMSDataRepository>();
builder.Services.AddScoped<IProcessMasterRepository, ProcessMasterRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IBiFinCusDefSheetRepository, BiFinCusDefSheetRepository>();
// Add AutoMapper
//builder.Services.AddAutoMapper(typeof(MappingProfile));




//builder.Services.AddCors(options =>
//{
//	options.AddPolicy("CorsPolicy",
//		builder => builder
//			.WithOrigins("http://13.127.126.231:2344") // Allow requests from the Angular app's origin
//			.AllowAnyMethod()
//			.AllowAnyHeader()
//	);
//});

//developer
//builder.Services.AddCors(policy =>
//{
//	policy.AddPolicy("CorsPolicy", opt => opt
//		.AllowAnyOrigin()
//		.AllowAnyMethod()
//		.AllowAnyHeader());
//});

//live
//builder.Services.AddCors(options =>
//{
//	options.AddPolicy("CorsPolicy", builder =>
//	{
//		builder
//			.WithOrigins("https://nsight.novatechset.com") // Allow requests from the web app's origin
//			.AllowAnyMethod()
//			.AllowAnyHeader()
//			.AllowCredentials()
//			.WithHeaders("Authorization"); // Allow the Authorization header
//	});
//});

//UAT
builder.Services.AddCors(options =>
{
	options.AddPolicy("CorsPolicy", builder =>
	{
		builder
			.WithOrigins("https://devnsight.novatechset.com") // Allow requests from the web app's origin
			.AllowAnyMethod()
			.AllowAnyHeader()
			.AllowCredentials()
			.WithHeaders("Authorization"); // Allow the Authorization header
	});
});


builder.Services.AddSignalR();
builder.Services.AddSignalR().AddHubOptions<RealTimeHub>(options =>
{
	// Hub configuration options, if any
});

// Register JwtService with dependency injection
builder.Services.AddSingleton<JwtService>(serviceProvider =>
{
	var configuration = serviceProvider.GetRequiredService<IConfiguration>();
	var jwtService = new JwtService(configuration);
	return jwtService;
});



builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
	options.SaveToken = true;
	options.RequireHttpsMetadata = false;
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateIssuerSigningKey = true,
		ValidAudience = builder.Configuration["JWT:ValidAudience"],
		ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
		ClockSkew = TimeSpan.Zero,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
	};
});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AdminPolicy", policy => policy.RequireRole(Roles.Admin));
	options.AddPolicy("UserPolicy", policy => policy.RequireRole(Roles.User));
	// Add more policies as needed
});

builder.Services.AddHttpContextAccessor();


string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
string connectionString = "";
if (environment == "UAT")
{
	connectionString = "Server=dev-test-editgenie.ctrglcosg13x.ap-south-1.rds.amazonaws.com;User ID=kpidevapp;Password=Ytrew@18Dec23;Database=KPIAnalyticsDev;Trusted_Connection=False;";
}
else
{
	connectionString = "Server=sesame-dblive.ctrglcosg13x.ap-south-1.rds.amazonaws.com;User ID=kpiliveapp;Password=Alyp%2okiFbz4;Database=KPIAnalyticsLive;Trusted_Connection=False;";
}

var app = builder.Build();

app.UseExceptionHandler(options =>
{
	options.Run(async context =>
	{
		context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
		context.Response.ContentType = "application/json";

		var ex = context.Features.Get<IExceptionHandlerFeature>();
		if (ex != null)
		{
			var errorMessage = $"An error occurred: {ex.Error.Message}";
			var errorResponse = new
			{
				type = "https://tools.ietf.org/html/rfc7235#section-3.1",
				title = "Unauthorized",
				status = 401,
				traceId = Activity.Current?.Id ?? context.TraceIdentifier,
				detail = errorMessage
			};

			var jsonResponse = JsonSerializer.Serialize(errorResponse);
			await context.Response.WriteAsync(jsonResponse);
			// Log the exception
			Log.Error(ex.Error, errorMessage);
			// Log the exception in the BiApiCallLog table
			if (context.Items.TryGetValue("ApiCallLog", out var apiCallLogObj) && apiCallLogObj is BiApiCallLog apiCallLog)
			{
				apiCallLog.Exception = ex.Error.Message;
				apiCallLog.Timestamp = DateTime.Now;
				apiCallLog.Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
				apiCallLog.ServerName = Environment.MachineName;
				apiCallLog.Username = Environment.UserName;
				var responseData = await GetResponseDataAsync(context.Response);
				apiCallLog.ResponseData = responseData ?? "";
				apiCallLog.StatusCode = context.Response.StatusCode;
				await _jTSContext.BiApiCallLogs.AddAsync(apiCallLog);
				await _jTSContext.SaveChangesAsync();
			}
		}
	});
});

async Task<string?> GetResponseDataAsync(HttpResponse response)
{
	if (!response.HasStarted)
	{
		// Rewind the response stream so it can be read from the beginning
		response.Body.Seek(0, SeekOrigin.Begin);

		// Read the response body
		using (var reader = new StreamReader(response.Body, Encoding.UTF8))
		{
			return await reader.ReadToEndAsync();
		}
	}

	return null;
}

//app.UseMiddleware<ApiLoggingMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	//app.UseErrorHandlingMiddleware();
	//app.UseSwagger();
	//app.UseSwaggerUI();
}
//else
//{
//	app.UseErrorHandlingMiddleware();
//	app.UseHsts();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();


app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
