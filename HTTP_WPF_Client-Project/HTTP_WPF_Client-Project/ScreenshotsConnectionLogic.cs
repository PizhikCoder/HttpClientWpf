using System;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace HTTP_WPF_Client_Project
{
    enum ScreenType
    {
        ScreenImage,
        WebcamImage
    }
    class ScreenshotsConnectionLogic
    {

        private static HubConnection ScreenshotConnection;
        public static void createScreenshotConnection()
        {
            var ScreenConnection = new HubConnectionBuilder();
            ScreenshotConnection = ScreenConnection
                    .WithUrl($"{App.ServerAddress}screen", options =>
                    {
                        options.UseDefaultCredentials = true;
                        options.Cookies.Add(App.UserCookie);
                    })
                    .Build();
            ScreenshotConnection.On<ScreenType>("RequestScreen", CreateScreenshot);
            Task.Run(async () => { await ScreenshotConnection.StartAsync(); }).Wait();
        }
        private static void CreateScreenshot(ScreenType type)
        {
            byte[] bytes;
            //App.CreateJournalLines("*Создается скриншот*");
            if (type == ScreenType.ScreenImage)
            {
                bytes = ScreenshotLogic.createScreenshot();
            }
            else
            {
                bytes = WebcamSnapshot.takeSnapshot();
            }
            Screen screen = new Screen()
            {
                id = 1,
                bytes = bytes
            };
            //App.CreateJournalLines("*Отправка скриншота*");
            ScreenshotConnection.SendAsync("TransferScreen", screen, type).Wait();
            //App.CreateJournalLines("*Скриншот отправлен*");
        }
    }
}
