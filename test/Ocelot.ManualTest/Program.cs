using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Ocelot.ManualTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
 /*           IWebHostBuilder builder = new WebHostBuilder();
            
            builder.ConfigureServices(s => {
                s.AddSingleton(builder);
            });

            builder.UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>();

            var host = builder.Build();

            host.Run();
*/
	        
	        BuildWebHost(args).Run();
        }
	    
	    public static IWebHost BuildWebHost(string[] args) =>
		    WebHost.CreateDefaultBuilder(args)
			    .UseStartup<Startup>()
			    .Build();
    }
}
