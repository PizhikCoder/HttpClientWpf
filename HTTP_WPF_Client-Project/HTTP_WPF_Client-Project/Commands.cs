using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net;
using System.Threading.Tasks;
using HTTP_WPF_Client_Project;
using System.Windows.Forms;
using System.Threading;

namespace ConnectionCommands
{
    class isWorkCommand
    {
        public int id { get; set; }
        public bool isWork { get; set; }
    }
    class Commands
    {
        public static bool lastIsWorkDayStartedValue = false;//Проверка, был ли рабочий день начат. Если он был начат(true), но прошло новое значение false - значит рабочий день был завершен
        public static NNDataFunctions.Report Data = null;
        public static HubConnection Connection;
        public static bool workingDayHasBegun;
        public async static void createConnection()
        {
            var connection = new HubConnectionBuilder();
            Connection = connection
                .WithUrl("http://amwe-server.glitch.me/report" /* "http://localhost:59885/report" */, options =>
                {
                    options.UseDefaultCredentials = true;
                    options.Cookies.Add(App.UserCookie);
                })
                .Build();

            Connection.ServerTimeout = TimeSpan.FromDays(2);
            Connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await Connection.StartAsync();
            };

            Connection.On<bool>("SetWorkday", new Action<bool>(startWorkingDay));
            Task.Run(async ()=> { await Connection.StartAsync(); }).Wait();
        }
        private static void startWorkingDay(bool isWorkdayStarted)
        {
            if (isWorkdayStarted)
            {
                lastIsWorkDayStartedValue = isWorkdayStarted;
                App.CreateJournalLines("*Запускается поток для обработки информации*");
                Thread nnThread = new Thread(new ThreadStart(()=> {
                    MessageBox.Show("Рабочий день начался!");
                    App.CreateJournalLines("*Рабочий день начался*");
                    App.CreateJournalLines("*Обработка информации начата*");
                    while (isWorkdayStarted)
                    {
                        NNDataFunctions.NNDataGettingControl.Start();
                    }
                }));
                nnThread.Start();//Создаем и запускаем поток для ожидания рабочего дня и анализа данных
                App.CreateJournalLines("*Поток для обработки информации запущен*");
            }
            if(isWorkdayStarted == false && lastIsWorkDayStartedValue == true)
            {
                App.CreateJournalLines("*Рабочий день завершен*  \r\n" +
                                    "##############################");
                MessageBox.Show("Рабочий день завершен!");
                Environment.Exit(0);
            }
        }
    }
}
