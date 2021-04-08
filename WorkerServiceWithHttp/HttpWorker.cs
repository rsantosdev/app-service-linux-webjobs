using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace WorkerServiceWithHttp
{
    public class HttpWorker : BackgroundService
    {
        private IHost _webHost;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            /*
             * In order to run on AppService for Containers we need to have a http port exposed
             * so the cluster can verify if the container is running
             */
            _webHost = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.Configure(c =>
                        c.Run(async s => await s.Response.WriteAsync("Hello App Service Containers", stoppingToken))
                    );
                })
                .Build();

            return _webHost.StartAsync(stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _webHost?.StopAsync(cancellationToken);
            return base.StopAsync(cancellationToken);
        }
    }
}