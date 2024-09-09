using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Akka.Actor;
using Akka.Configuration;
using Akka.DependencyInjection;
using btm.paas.Actors;
using btm.paas.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace btm.paas
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 0;
            if (args.Length != 0)
            {
                _ = int.TryParse(args[0], out port);
            }

            //HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            //IServiceCollection services = builder.Services;
            IServiceCollection services = new ServiceCollection();

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var configuration = new ConfigurationBuilder()
               .AddJsonFile("./appsettings.json", false, true)
               .AddJsonFile($"./appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true)
               .Build();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            string root = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
            Config cfg = ConfigurationFactory.ParseString(File.ReadAllText(Path.Combine(root, "Config.properties")));
            Config finalConfig = port > 0
                ? ConfigurationFactory.ParseString($"akka.remote.dot-netty.tcp.port = {port}").WithFallback(cfg)
                : cfg;

            Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                Console.WriteLine("TEST");
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            });
            Microsoft.Extensions.DependencyInjection.ServiceProvider serviceProvider = services.BuildServiceProvider();

            DatabaseUpdate(serviceProvider, configuration);

            ActorSystem _paasActorSystem = ActorSystem.Create("paassystem", finalConfig);
            _paasActorSystem.ActorOf(Props.Create(() => new PaasActor(serviceProvider)), "paas");
            _paasActorSystem.WhenTerminated.Wait();

            //using IHost host = builder.Build();
            //host.Run();
        }

        private static void DatabaseUpdate(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            Log.Information("Seeding database...");

            string connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                Log.Debug("Connection string is not provided");
                return;
            }

            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
            context!.Database.Migrate();
        }
    }
}
