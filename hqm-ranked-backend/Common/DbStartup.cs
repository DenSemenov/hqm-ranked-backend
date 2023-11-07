namespace hqm_ranked_backend.Common
{
    public static class DbStartup
    {
        public static async Task InitializeDatabasesAsync(IServiceProvider services)
        {
            // Create a new scope to retrieve scoped services
            using var scope = services.CreateScope();

            await scope.ServiceProvider.GetRequiredService<ApplicationDbInitializer>()
                .Initialize();
        }
    }
}
