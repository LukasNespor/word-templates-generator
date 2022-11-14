using Api.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Api.Startup))]
namespace Api
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string storage = Environment.GetEnvironmentVariable("Storage");
            BlobService blobs = new(storage);
            TableService tables = new(storage);

            builder.Services.AddTransient(x => blobs);
            builder.Services.AddTransient(x => tables);

            blobs.EnsureContainerAsync(Constants.TemplatesContainerName).ConfigureAwait(false);
            tables.EnsureTableAsync(Constants.ListsTableName).ConfigureAwait(false);
            tables.EnsureTableAsync(Constants.TemplatesContainerName).ConfigureAwait(false);
        }
    }
}
