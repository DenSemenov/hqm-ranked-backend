using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

IConfigurationRoot configuration = builder.Build();

var connection = configuration.GetSection("Database:Connection").Value;

var contextOptions = new DbContextOptionsBuilder<RankedDb>()
   .UseNpgsql(connection)
   .Options;
var _dbContext = new RankedDb(contextOptions);
var _storageService = new StorageService(_dbContext);
var _spotifyService = new SpotifyService(_dbContext);
var _replayCalcService = new ReplayCalcService(_dbContext, _storageService, _spotifyService);

await Main();

async Task Main()
{
   
    while (true)
    {
        await ParseLastReplay();

        Thread.Sleep(10000);
    }
}

async Task ParseLastReplay()
{
    var replayId = _dbContext.ReplayData.Include(x => x.ReplayFragments).Include(x => x.Game).Where(x => x.ReplayFragments.Count == 0).Select(x => x.Game.Id).FirstOrDefault();

    if (replayId != null)
    {
        await _replayCalcService.ParseReplay(new ReplayRequest
        {
            Id = replayId
        });
    }
}