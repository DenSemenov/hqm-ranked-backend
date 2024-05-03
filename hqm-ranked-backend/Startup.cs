using Hangfire;
using Hangfire.PostgreSql;
using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Services;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Data;

namespace hqm_ranked_backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var db = Configuration.GetSection("Database:Connection").Value;
            services.AddCors();
            services.AddDbContextPool<RankedDb>(options => options.UseNpgsql(db));
            services.AddScoped<ApplicationDbInitializer>();

            services.AddScoped<IPlayerService, PlayerService>();
            services.AddScoped<ISeasonService, SeasonService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IServerService, ServerService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IImageGeneratorService, ImageGeneratorService>();
            services.AddScoped<IReplayService, ReplayService>();

            services.AddHangfire(x => x.UsePostgreSqlStorage(db));

            services.AddSignalR();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = AuthOptions.ISSUER,
                            ValidateAudience = true,
                            ValidAudience = AuthOptions.AUDIENCE,
                            ValidateLifetime = true,
                            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                            ValidateIssuerSigningKey = true,
                        };
                    });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HQM Ranked API", Version = "v1" });
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            DbStartup.InitializeDatabasesAsync(app.ApplicationServices).Wait();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tutorial API V1");
                c.RoutePrefix = "";
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(x => x
                 .AllowAnyMethod()
                 .AllowAnyHeader()
                 .SetIsOriginAllowed(origin => true)
                 .AllowCredentials());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHangfireDashboard("/hangfire");
            app.UseHangfireServer();

            var scope = app.ApplicationServices.CreateScope();

            var eventService = scope.ServiceProvider.GetRequiredService<IEventService>() as EventService;
            RecurringJob.AddOrUpdate("CreateNewDailyEvent", () => eventService.CreateNewEvent(), Cron.Daily);
            var replayService = scope.ServiceProvider.GetRequiredService<IReplayService>() as ReplayService;
            RecurringJob.AddOrUpdate("RemoveOldReplays", () => replayService.RemoveOldReplays(), Cron.Daily);
        }
    }
}