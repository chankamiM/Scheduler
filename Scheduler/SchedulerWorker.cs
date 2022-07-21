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
