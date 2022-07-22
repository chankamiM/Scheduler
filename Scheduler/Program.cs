using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Helper.App_PreInit();

            CreateHostBuilder(args).Build().Run();

            Helper.App_Closed();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(/*args*/)
            .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<SchedulerWorker>();
                });
    }
}
