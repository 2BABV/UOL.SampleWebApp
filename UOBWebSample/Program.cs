using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace UOBWebSample
{
    /// <summary>
    /// Provides starting point of the content service.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The start procedure.
        /// </summary>
        /// <param name="args">The start arguments of the content service.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates a web host builder.
        /// </summary>
        /// <param name="args">The start arguments of the content service.</param>
        /// <returns>The used <see cref="IWebHostBuilder"/>.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
        }
    }
}
