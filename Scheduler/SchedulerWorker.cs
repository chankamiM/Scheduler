using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Scheduler
{
    public class SchedulerWorker : BackgroundService
    {

        public SchedulerWorker(ILogger<SchedulerWorker> logger)
        {

        }

        /*

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            base.Dispose();

        }
        */
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
              Helper.App_PostInit();

            while (!stoppingToken.IsCancellationRequested)
            {

                await Task.Delay(1000, stoppingToken);
            }

            Helper.App_Closing();

        }
    }
}
