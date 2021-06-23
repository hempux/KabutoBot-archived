using InfluxDB.Client;
using InfluxDB.Client.Writes;
using net.hempux.ninjawebhook.Models;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;

namespace net.hempux.Utilities.Influx
{

    public class InfluxAPI
    {
        // https://github.com/influxdata/influxdb-client-csharp
        

        private string influxserver;
        private string influxdatabase;
        private string influxretentionpolicy;
        private string influxuser;
        private string influxpassword;

        private InfluxDBClient client;
        public InfluxAPI(string influx_server,
            string influx_database,
            string influx_retentionpolicy,
            string influx_user,
            string influx_password,
            string influx_authtoken = "placeholder_if_blank")
        {

            influxserver = influx_server;
            influxdatabase = influx_database;
            influxretentionpolicy = influx_retentionpolicy;
            influxuser = influx_user;
            influxpassword = influx_password;
            string authtoken = influx_authtoken;
            string bucket = influx_database;
            string influxversion = GetInfluxdbVersion();
            var options = new InfluxDBClientOptions.Builder()
                .Url(influx_server)
                .AuthenticateToken(authtoken)
                .ReadWriteTimeOut(TimeSpan.FromSeconds(1))
                .TimeOut(TimeSpan.FromSeconds(1))
                .Bucket(bucket)
                .Org("Bluescreen")
                .Build();


            // Test connection to server after initalizing the class
           // Testconnection();

            double version = double.Parse(influxversion.Substring(0, 3), CultureInfo.InvariantCulture);
            if (version < 1.8)
            {
                Log.Error
                (
                @"Infludb is running an unsupported version ({version})
                  Influxdb 1.8.0+ or later required.", influxversion
                );
            }


            if (version < 2.0)
            {
                Log.Information("InfluxDB version {version} discovered", influxversion);
                client = InfluxDBClientFactory.Create(options);
            }
            else
            {
                Log.Information("InfluxDB version {version} discovered", influxversion);
                client = InfluxDBClientFactory.CreateV1(influxserver, influxuser, influxpassword.ToCharArray(), influxdatabase, influxretentionpolicy);
            }

            using (var writeApi = client.GetWriteApi())
            {

                writeApi.EventHandler += (sender, eventArgs) =>
                {
                    if (eventArgs is WriteErrorEvent @event)
                    {
                        var exception = @event.Exception;

                        Console.WriteLine(exception.Message);
                    }
                };

                var point = PointData.Measurement("ncentral")
                    .Tag("host", "ConnectiontestHost")
                    .Tag("Problemtype", "ConnectiontestProblem")
                    .Field("Computer", "ConnectiontestComputer")
                    .Field("notifstate", "ConnectiontestState");

                writeApi.WritePoint(point);


            }

        }



        /// <summary>
        /// Gets the version of influxDB running on the target database server
        /// </summary>
        /// <returns>InfluxDB Version (for example 2.0.1)</returns>
        private string GetInfluxdbVersion()
        {
            string version = new string("Unknown");
            HttpClient httpclient = new HttpClient();
            httpclient.Timeout = TimeSpan.FromSeconds(5);

            try
            {
                var response = httpclient.GetStringAsync(influxserver + "/health").Result;
                if (response.StartsWith("{\"checks"))
                {
                    var parsed = JObject.Parse(response);
                    version = parsed.GetValue("version").ToString();

                }
            }
            catch (Exception faultEx)
            {
                Handleerrors(faultEx);

            }

            return version;
        }

        /// <summary>
        /// Sends a single ActiveIssue item to influxDB,
        /// Useful for testing or if the loop for sending multiple
        /// data needs to be outside the influxdb method
        /// </summary>
        /// <param name="exportdata"></param>
        public void Pushdata(DetailedActivity exportdata)
        {


            using (var writeApi = client.GetWriteApi())
            {
                var point = PointData.Measurement("ninjaRMM")
                    .Tag("host", exportdata.Device.ToString())
                    .Tag("Problemtype", exportdata.Type)
                    .Field("Computer", exportdata.DeviceId.ToString())
                    .Field("notifstate", exportdata.StatusCode.ToString());


                try
                {

                    writeApi.WritePoint(point);
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }
            }



            client.Dispose();
            Log.Information("Pushed " + exportdata.DeviceId);

        }

        /// <summary>
        /// Sends an array of ActiveIssue items to InfluxDB.
        /// 
        /// </summary>
        /// <param name="exportdata"></param>
        public void PushdataList(DetailedActivity[] exportdata)
        {
            List<PointData> data = new List<PointData>();

            using (var writeApi = client.GetWriteApi())
            {

                foreach (DetailedActivity issue in exportdata)
                {

                    var point = PointData.Measurement("ninjaRMM")
                        .Tag("host", issue.Device.ToString())
                        .Tag("Problemtype", issue.Type)
                        .Field("Computer", issue.DeviceId.ToString())
                        .Field("notifstate", issue.StatusCode.ToString())
                        .Field("Problemtype", issue.Status);


                    data.Add(point);
                }

                try
                {
                    writeApi.WritePoints(data);
                    Log.Information("Wrote " + exportdata.Length + " issues to influxDB");
                }
                catch (Exception faultex)
                {
                    Handleerrors(faultex);
        
                        

                }
                writeApi.Flush();
            }

            client.Dispose();

        }



        /// <summary>
        /// Test the connection to the influxdb server and outputs results to console.
        /// </summary>
        public void Testconnection()
        {
            Log.Information("Testing influx connection..");
            HttpClient httpclient = new HttpClient();
            httpclient.Timeout = TimeSpan.FromSeconds(5);

            try
            {
                var response = httpclient.GetStringAsync(influxserver + "/health").Result;
            }
            catch (Exception faultEx)
            {
                Handleerrors(faultEx);
            }



        }

        /// <summary>
        /// Helper method for cleaner code to avoid repetitive clutter, takes in exception thrown from some methods and outputs the error to console.
        /// </summary>
        /// <param name="faultEx"></param>
        private void Handleerrors(Exception faultEx)
        {
            // Checks if there is a nested InnerException and grabs its message.
            // This makes the errormessage the users sees in case the test fails more useful
            // Enabling Nullable to make the compiler happier and the #pragma tag supresses code warnings in Visual studio.
#nullable enable
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string? errormessage = faultEx.InnerException?.InnerException?.Message;

            if (errormessage == null)
                errormessage = faultEx.InnerException?.Message;

            if (errormessage == null)
                errormessage = faultEx.Message;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#nullable disable

            // Clarify "A task was canceled" error message that is thrown if the GetStringAsync request times out.
            if (errormessage.EndsWith("canceled."))
                errormessage = "connection attempt timeout";


            Log.Error(
                @"Unable to write to influx,check configuration, and that influxdb is running and is reachable
                Error: {exception}"
                ,errormessage
                    );
        }
    }

}