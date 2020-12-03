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
            var connection = new HubConnectionBuilder()
                .WithUrl("http://amwe-server.glitch.me/report" /* "http://localhost:59885/report" */, options =>
                {
                    options.UseDefaultCredentials = true;
                    options.Cookies.Add(HTTP_WPF_Client_Project.App.UserCookie);
                })
                .Build();

            connection.KeepAliveInterval = TimeSpan.FromDays(1);
            connection.HandshakeTimeout = TimeSpan.FromDays(1);
            connection.ServerTimeout = TimeSpan.FromDays(2);
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            connection.On<bool>("SetWorkday", new Action<bool>((isWorkdayStarted) => { App.workingDayHasBegun = isWorkdayStarted; }));
            await connection.StartAsync();
            Connection = connection;
        }
    }
}
