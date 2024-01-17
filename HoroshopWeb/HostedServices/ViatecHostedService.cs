using HoroshopWeb.Clients;
using HoroshopWeb.Extensions;
using HoroshopWeb.Infrastructure;

namespace HoroshopWeb.HostedServices;

public class ViatecHostedService(
    FileService fileService,
    ViatecClient viatecClient
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var newDocument = await viatecClient.GetDocumentAsync(stoppingToken);
            var newVersion = newDocument.GetVersion();

            var oldDocument = await fileService.GetViatecDocumentAsync(stoppingToken);
            var oldVersion = oldDocument?.GetVersion();
            
            if (newVersion != oldVersion)
            {
                await fileService.StoreViatecFileAsync(newDocument, stoppingToken);
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
