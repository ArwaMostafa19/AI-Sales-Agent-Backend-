
using System.Security.Claims;
using System.Text;
using AI_Sales_Agent.Abstractions;
using AI_Sales_Agent.Data;
using AI_Sales_Agent.Domain;
using AI_Sales_Agent.Infrastructure.Audit;
using AI_Sales_Agent.Infrastructure.Auth;
using AI_Sales_Agent.Infrastructure.Email;
using AI_Sales_Agent.Infrastructure.Errors;
using AI_Sales_Agent.Infrastructure.Mongo;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

namespace AI_Sales_Agent
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var jwtOptions = builder.Configuration
                .GetSection(JwtOptions.SectionName)
                .Get<JwtOptions>() ?? new JwtOptions();

            builder.Services.Configure<JwtOptions>(
                builder.Configuration.GetSection(JwtOptions.SectionName));
            builder.Services.Configure<EmailOptions>(
                builder.Configuration.GetSection(EmailOptions.SectionName));

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure()));

            builder.Services.Configure<MongoSettings>(
                builder.Configuration.GetSection(MongoSettings.SectionName));
            builder.Services.PostConfigure<MongoSettings>(options =>
            {
                options.KnowledgeDocumentsCollection = builder.Configuration["KNOWLEDGE_DOCUMENTS_COLLECTION"]
                    ?? options.KnowledgeDocumentsCollection;
                options.KnowledgeChunksCollection = builder.Configuration["KNOWLEDGE_CHUNKS_COLLECTION"]
                    ?? options.KnowledgeChunksCollection;
                options.KnowledgeSummariesCollection = builder.Configuration["KNOWLEDGE_SUMMARIES_COLLECTION"]
                    ?? options.KnowledgeSummariesCollection;
                options.KnowledgeUploadsCollection = builder.Configuration["KNOWLEDGE_UPLOADS_COLLECTION"]
                    ?? options.KnowledgeUploadsCollection;
            });

            builder.Services.AddSingleton<IMongoClient>(_ =>
            {
                var mongoSettings = builder.Configuration
                    .GetSection(MongoSettings.SectionName)
                    .Get<MongoSettings>() ?? new MongoSettings();
                var clientSettings = MongoClientSettings.FromConnectionString(mongoSettings.ConnectionString);
                clientSettings.MinConnectionPoolSize = mongoSettings.MinConnectionPoolSize;
                clientSettings.MaxConnectionPoolSize = mongoSettings.MaxConnectionPoolSize;

                return new MongoClient(clientSettings);
            });
            builder.Services.AddScoped<IMongoDbContext, MongoDbContext>();
            builder.Services.AddScoped<IMongoDatabase>(sp =>
                sp.GetRequiredService<IMongoDbContext>().Database);

            builder.Services
                .AddIdentity<User, IdentityRole<Guid>>(options =>
                {
                    options.Password.RequiredLength = 8;
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                            var tokenSecurityStamp = context.Principal?.FindFirstValue("security_stamp");

                            if (!Guid.TryParse(userId, out _) || string.IsNullOrWhiteSpace(tokenSecurityStamp))
                            {
                                context.Fail("Invalid token.");
                                return;
                            }

                            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
                            var user = await userManager.FindByIdAsync(userId);
                            if (user is null)
                            {
                                context.Fail("Invalid token.");
                                return;
                            }

                            var currentSecurityStamp = await userManager.GetSecurityStampAsync(user);
                            if (currentSecurityStamp != tokenSecurityStamp)
                            {
                                context.Fail("Token is no longer valid.");
                            }
                        }
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                foreach (var permission in Permissions.All)
                {
                    options.AddPolicy(permission, policy =>
                        policy.Requirements.Add(new PermissionRequirement(permission)));
                }
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token."
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
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            builder.Services.AddScoped<IAuditLogger, AuditLogger>();
            builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
            builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
            builder.Services.AddMediatR(configuration =>
                configuration.RegisterServicesFromAssembly(typeof(Program).Assembly));
            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            builder.Services.AddSingleton<MongoDbContext>();

            var app = builder.Build();

            // Seed Database Roles and Admin User
            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    await DbInitializer.SeedAsync(scope.ServiceProvider);
                }
                catch (Exception)
                {
                    // Log or handle as needed
                }
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
