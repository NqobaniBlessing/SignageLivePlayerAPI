using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SignageLivePlayerAPI.Configurations;
using SignageLivePlayerAPI.Endpoints;
using SignageLivePlayerAPI.Services;
using SignageLivePlayerAPI.Services.Interfaces;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<ISecurityContextService, SecurityContextService>();
builder.Services.AddAutoMapper(typeof(MapperConfig));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "SL_Auth";
    options.DefaultChallengeScheme = "SL_Auth";
    options.DefaultScheme = "SL_Auth";

}).AddJwtBearer("SL_Auth", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ViewOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role);
    });

    options.AddPolicy("ViewAndEdit", policy =>
    {
        policy.RequireClaim("admin", "true");
        policy.RequireClaim(ClaimTypes.Role, "Software Developer", "Content Manager");
    });
});

// To authenticate using the JWT on the Swagger UI
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "SignageLivePlayerAPI", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[]{}
        }
    });
});

// Register custom logging
builder.Logging.ClearProviders();

var logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("Logs/signagelive_player_api.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Logging.AddSerilog(logger);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseStatusCodePages(async statusCodeContext
    => await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
                 .ExecuteAsync(statusCodeContext.HttpContext));
}

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler(exceptionHandlerApp
    => exceptionHandlerApp.Run(async context
        => await Results.Problem()
                     .ExecuteAsync(context)));

app.UseHttpsRedirection();

app.MapSignageLiveAuthEndpoints();
app.MapSignageLiveUserEndpoints();
app.MapSignageLivePLayerEndpoints();

app.Run();