using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net;
using System.Threading.Tasks;
using HTTP_WPF_Client_Project;

namespace ConnectionCommands
{
    class isWorkCommand
    {
        public int id { get; set; }
        public bool isWork { get; set; }
    }
    class Commands
    {
        public static NNDataFunctions.Report Data = null;
        public static HubConnection Connection;
        public async static void createConnection()
        {
            var connection = new HubConnectionBuilder();
            Connection = connection
                .WithUrl("http://amwe-server.glitch.me/report" /* "http://localhost:59885/report" */, options =>
                {
                    options.UseDefaultCredentials = true;
                    options.Cookies.Add(HTTP_WPF_Client_Project.App.UserCookie);
                })
                .Build();

            Connection.ServerTimeout = TimeSpan.FromDays(2);
            Connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await Connection.StartAsync();
            };

            Connection.On<bool>("SetWorkday", new Action<bool>((isWorkdayStarted) => { App.workingDayHasBegun = isWorkdayStarted; }));
            Task.Run(async ()=> { await Connection.StartAsync(); }).Wait();
        }
    }
}
