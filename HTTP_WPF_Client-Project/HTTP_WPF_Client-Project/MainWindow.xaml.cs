using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HTTP_WPF_Client_Project
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Пример подключения к хабу
        // каждое соединение должно создаваться отдельными экземплярами. это важно
        private async Task ConfigueAndStartHubConnectionsAsync()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl($"") // сюда url
                .Build(); // создать экземпляр соединения

            /* var connection1 = new HubConnectionBuilder()
                .WithUrl($"") // сюда еще url
                .Build(); // создать еще один экземпляр соединения
            //... */

            // район установки таймаутов
            connection.KeepAliveInterval = TimeSpan.FromDays(1);
            connection.HandshakeTimeout = TimeSpan.FromDays(1);
            connection.ServerTimeout = TimeSpan.FromDays(2);

            connection.Closed += async (error) => // если соединение закрылось...
            {
                // при желании обработай Exception error
                await connection.StartAsync(); // запустить соединение заново
            };

            connection.On("SomeMethod", SomeMethod); // если произошел вызов метода "SomeMethod" на сервере к этому клиенту, вот его обработчик

            await connection.StartAsync(); // Запустить настроенное соединение

            // после того как соединение запустилось, можно вызывать методы на сервере

            await connection.InvokeAsync("DoSome"/*, после запятой можешь приложить какие-то объекты, если это требуется методу*/);
        }

        private void SomeMethod()
        {
            // сервер сообщил вам выполнить этот метод через HubConnection connection
        }
        #endregion
    }
}
