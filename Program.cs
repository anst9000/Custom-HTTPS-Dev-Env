using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HttpsTest
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

	public static IHostBuilder CreateHostBuilder(string[] args) =>
		Host.CreateDefaultBuilder(args)
			.ConfigureServices((context, services) => {
				HostConfig.CertPath = context.Configuration["CertPath"];
				HostConfig.CertPassword = context.Configuration["CertPassword"];
				HostConfig.CustomDomainName = context.Configuration["CustomDomainName"];
			})
			.ConfigureWebHostDefaults(webBuilder => {
				webBuilder.ConfigureKestrel(opt => {
					var host = Dns.GetHostEntry(HostConfig.CustomDomainName);

					opt.Listen(host.AddressList[0], 5001, listenOpt => {
						listenOpt.UseHttps(HostConfig.CertPath, HostConfig.CertPassword);
					});
					opt.Listen(host.AddressList[0], 5000);
					// opt.ListenAnyIP(5000);
				});

				webBuilder.UseStartup<Startup>();
			});
	}

	public static class HostConfig
	{
		public static string CertPath { get; set; }
		public static string CertPassword { get; set; }
		public static string CustomDomainName { get; set; }
	}
}
