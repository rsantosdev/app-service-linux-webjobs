# Azure App Service Linux + WebJobs

This repo is a simple demonstration on how to run worker services / webjobs on Azure
App service for Linux.

There's an youtube video where I explain in detatils, portuguese only
https://www.youtube.com/watch?v=557kiOUffdw

### How to run
Build the WorkerServiceWithHttp image and push to your own Container Registry and 
create a new app service with that container

### How it works
The app service cluster will send a health check request on the configured http port (defaults to 80)
if it returns a 200 status that means the container is healthy if other status it will try to restart
the container. So all you need to do is start a simple kestrel service listen for those requests.

```csharp
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
```