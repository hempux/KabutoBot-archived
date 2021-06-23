// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using net.hempux.Utilities;
using Serilog;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using net.hempux.kabuto.database;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder;
using System.Collections.Concurrent;

namespace net.hempux.kabuto
{
    public class Program
    {

        public static async Task Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();


            var host = CreateHostBuilder(args).Build();
            var config = host.Services.GetRequiredService<IConfiguration>();

            if (!(File.Exists(AppSettings.Current.SqliteDatabase)))
            {
                Log.Information("Creating sqlite file {filename}", AppSettings.Current.SqliteDatabase);
                SqliteEngine sqliteEngine = new SqliteEngine();
            }
            else
            {
                Log.Information("Using sqlite file {filename}", AppSettings.Current.SqliteDatabase);
            }

            Log.Information("Starting server listening on {AppUrl}", AppSettings.Current.ListenAddress);


            await host.RunAsync();


        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {

                    webBuilder.ConfigureLogging((logging) =>
                    {

                    });
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(AppSettings.Current.ListenAddress);

                });
    }

}
