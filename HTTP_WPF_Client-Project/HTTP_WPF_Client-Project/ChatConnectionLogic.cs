using System;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace HTTP_WPF_Client_Project
{
    class Chat
    {
        public Window1 window { get; set; }
        public uint ChatId { get; set; }
        public bool isChatWorking { get; set; }
    }
    class ChatConnectionLogic
    {
        private static uint chatId;
        public static List<Chat> Chats = new List<Chat> { };

        private static HubConnection ChatSystemConnection;
        public static void createChatConnection()
        {
            var connection = new HubConnectionBuilder();
            ChatSystemConnection = connection
                .WithUrl($"{App.ServerAddress}chat", options => {
                options.UseDefaultCredentials = true;
                options.Headers.Add("User-Agent", "Mozilla/5.0");
                options.Cookies.Add(App.UserCookie);
            }).Build();
            ChatSystemConnection.ServerTimeout = TimeSpan.FromDays(2);
            ChatSystemConnection.On<uint>("OpenChat", OpenChat);
            ChatSystemConnection.On<uint, string, string, DateTime>("ReceiveMessage", RecieveMessage);
            ChatSystemConnection.On<uint>("AcceptChatID", AcceptChat);
            ChatSystemConnection.On<uint>("CloseDeleteChat", DeleteChat);
            ChatSystemConnection.Closed += async (error) =>
            {
                //
                await ChatSystemConnection.StartAsync();
            };
            Task.Run(async () => { await ChatSystemConnection.StartAsync(); }).Wait();
        }

        private static void OpenChat(uint id)
        {
            MessageBox.Show("Администратор сети хочет начать чат с Вами");
            ChatSystemConnection.SendAsync("AcceptChat", id);
        }

        private static void AcceptChat(uint id)
        {
            App.CreateJournalLines("*Запускается чат...*");
            chatId = id;
            Application.Current.Dispatcher.Invoke(windowOfChatCreate);
            App.CreateJournalLines("*Чат запущен*");
        }
        private static void windowOfChatCreate()
        {
            Window1 window1 = new Window1();
            Chat chat = new Chat()
            {
                window = window1,
                ChatId = chatId,
                isChatWorking = true
            };
            Chats.Add(chat);
            window1.lbChatState.Content = chatId.ToString();
            window1.Visibility = Visibility.Visible;
            window1.Show();
            window1.Activate();
        }

        private static void DeleteChat(uint id)
        {
            MessageBox.Show("Сеанс работы чата завершен");
            Chat chat = Chats.Find(x => x.ChatId == id);
            chat.isChatWorking = false;
            Application.Current.Dispatcher.Invoke(()=>
            {
                chat.window.Close();
            });
            App.CreateJournalLines("*Сеанс работы чата завершен, чат зыкрыт*");
            Chats.Remove(chat);
        }

        private static void RecieveMessage(uint id, string text, string user, DateTime timestamp)
        {
            Chat chat = Chats.Find(x => x.ChatId == id);
            Application.Current.Dispatcher.Invoke(() => { chat.window.ChatBox.Text += timestamp.ToLongTimeString() + $"  {user}:" + text + "\n"; });
        }
        public static void SendMessage(uint ChatId, string message)
        {
            ChatSystemConnection.InvokeAsync("SendMessageToChat", ChatId, message);
        }
        public static async void onChatClosing(uint ChatId)
        {
            await ChatSystemConnection.InvokeAsync("CloseChat", ChatId);
        }
    }
}
