using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ste4lthPr0xy.Applications.Blackbox.Web
{
    public class Program
    {
        //public static DirectoryInfo DumpDirectory;

        public static void Main(string[] args)
        {
            //var dumpDirectoryName = DateTime.Now.ToString()
            //    .Replace(".", "-")
            //    .Replace(":", "-");

            //if (!Directory.Exists(dumpDirectoryName))
            //    DumpDirectory = Directory.CreateDirectory(dumpDirectoryName);
            //else
            //    DumpDirectory = new DirectoryInfo(dumpDirectoryName);

            CreateHostBuilder(args).Build().Run();
            //Directory.Delete(dumpDirectoryName);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel((context, options) => options.Configure(context.Configuration.GetSection("Kestrel"), reloadOnChange: true));
                    webBuilder.UseIIS();
                    webBuilder.UseIISIntegration();
                    webBuilder.UseStartup<Startup>();
                });
                //.ConfigureAppConfiguration(config => config.AddJsonFile("appsettings.json"));
    }
}
