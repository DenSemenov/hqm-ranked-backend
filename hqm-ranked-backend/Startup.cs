using Hangfire;
using Hangfire.PostgreSql;
using hqm_ranked_backend.Common;
using hqm_ranked_backend.Helpers;
using hqm_ranked_backend.Hubs;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.DTO;
using hqm_ranked_backend.Services;
using hqm_ranked_backend.Services.Interfaces;
using hqm_ranked_services;
using hqm_ranked_services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using static System.Net.Mime.MediaTypeNames;

namespace hqm_ranked_backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles", "logs", "log.html"), rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            Log.Information(LogHelper.GetInfoLog("Starting the Web Host..."));

            var db = Configuration.GetSection("Database:Connection").Value;
            services.AddCors();
            services.AddDbContextPool<RankedDb>(options => {
                options.UseNpgsql(db);
                options.ConfigureWarnings(warnings =>warnings.Ignore(CoreEventId.NavigationBaseIncludeIgnored));
            });
            services.AddScoped<ApplicationDbInitializer>();

            services.AddScoped<IPlayerService, PlayerService>();
            services.AddScoped<ISeasonService, SeasonService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IServerService, ServerService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IImageGeneratorService, ImageGeneratorService>();
            services.AddScoped<IReplayService, ReplayService>();
            services.AddScoped<IReplayCalcService, ReplayCalcService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<ISpotifyService, SpotifyService>();
            services.AddScoped<ICostService, CostService>();
            services.AddScoped<ITeamsService, TeamsService>();
            services.AddScoped<IAwardsService, AwardsService>();
            services.AddScoped<IContractService, ContractService>();
            services.AddScoped<IShopService, ShopService>();
            services.AddScoped<IWeeklyTourneyService, WeeklyTourneyService>();
            //services.AddHostedService<TelegramService>();

            services.AddScoped<ExceptionMiddleware>();

            services.AddMemoryCache();
            services.AddHangfire(x => x.UsePostgreSqlStorage(db));

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
                options.KeepAliveInterval = TimeSpan.FromSeconds(120);
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(120);
                options.MaximumReceiveMessageSize = null;
            });

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

            Log.Information(LogHelper.GetInfoLog("DB connected"));

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

            app.UseRouting().UseMiddleware<ExceptionMiddleware>();

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
                endpoints.MapHub<ActionHub>("/actionhub");
            });

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new AuthorizationFilter() }
            });
            app.UseHangfireServer();

            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles")),
                RequestPath = "/StaticFiles",
                EnableDefaultFiles = true,
                EnableDirectoryBrowsing = true,
            });

            var scope = app.ApplicationServices.CreateScope();

            var eventService = scope.ServiceProvider.GetRequiredService<IEventService>() as EventService;
            RecurringJob.AddOrUpdate("CreateNewDailyEvent", () => eventService.CreateNewEvent(), Cron.Daily);
            var replayService = scope.ServiceProvider.GetRequiredService<IReplayService>() as ReplayService;
            RecurringJob.AddOrUpdate("RemoveOldReplays", () => replayService.RemoveOldReplays(), Cron.Daily);
            RecurringJob.AddOrUpdate("ParseReplays", () => replayService.ParseAllReplays(), Cron.Daily);
            var spotifyService = scope.ServiceProvider.GetRequiredService<ISpotifyService>() as SpotifyService;
            RecurringJob.AddOrUpdate("GetPlaylist", () => spotifyService.GetPlaylist(), Cron.Daily);
            var costService = scope.ServiceProvider.GetRequiredService<ICostService>() as CostService;
            RecurringJob.AddOrUpdate("CalcCosts", () => costService.CalcCosts(), Cron.Daily);
            var teamsService = scope.ServiceProvider.GetRequiredService<ITeamsService>() as TeamsService;
            RecurringJob.AddOrUpdate("CancelExpiredInvites", () => teamsService.CancelExpiredInvites(), Cron.Hourly);
            var awardsService = scope.ServiceProvider.GetRequiredService<IAwardsService>() as AwardsService;
            RecurringJob.AddOrUpdate("CalcAwards", () => awardsService.CalcAwards(), Cron.Daily);
            var playerService = scope.ServiceProvider.GetRequiredService<IPlayerService>() as PlayerService;
            RecurringJob.AddOrUpdate("CalcPlayersStats", () => playerService.CalcPlayersStats(), Cron.Daily);
            var weeklyTourneyService = scope.ServiceProvider.GetRequiredService<IWeeklyTourneyService>() as WeeklyTourneyService;
            RecurringJob.AddOrUpdate("CreateWeeklyTourney", () => weeklyTourneyService.CreateTourney(), "30 17 * * 5");
        }
    }
}