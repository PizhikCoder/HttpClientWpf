using Gma.System.MouseKeyHook;
using KeyEventArgs =  System.Windows.Forms.KeyEventArgs;
using NNMethods;
using NNDataFunctions;
using System.Windows;
using System.Threading;
using System.IO;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.Text;
using ConnectionCommands;

namespace HTTP_WPF_Client_Project
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string ServerAddress = "http://amwe-server.glitch.me/"; // Адрес сервера
        public static string pathOfJournalFile = "";//Путь к файлу журнала программы
        public static IKeyboardMouseEvents _globalHook;
        public static Cookie UserCookie; // куки файл, который используется для подключения к хабам сервера
        public static Client clientData;
        public static bool workingDayHasBegun;
        private void Application_StartUp(object sender, StartupEventArgs e)
        {
            try
            {
                clientData = AuthUser(new string[] { Environment.UserName, "user" }, out UserCookie); // вызов метода авторизации
                Commands.createConnection();//Создание соединения

                pathOfJournalFile = Environment.CurrentDirectory + @"\Reports\" + DateTime.Today.ToShortDateString().ToString() + ".txt";

                if (!File.Exists(pathOfJournalFile))//Проверка, существует ли файл журнала
                {
                    File.Create(pathOfJournalFile).Close();//Созздается файл журнала, если он еще не был создан
                    CreateJournalLines("*Файл журнала создан, работа начата*");
                }
                else
                {
                    CreateJournalLines("\n");
                    CreateJournalLines("########################");
                    CreateJournalLines("*Программа перезапущена*");
                }

                NN.StartNNTraining();

                while (!workingDayHasBegun)//Ожидание начала рабочего дня
                {
                }
                CreateJournalLines("*Рабочий день начался*");
                while (workingDayHasBegun)
                {
                    if (_globalHook == null)//Оформляем событие по вытягиванию нажатых клавиш
                    {
                        _globalHook = Hook.GlobalEvents();
                        _globalHook.KeyDown += GlobalHookKeyPress;
                    }

                    Thread nnThread = new Thread(new ThreadStart(NNDataGettingControl.Start));
                    CreateJournalLines("*Запускается поток для обработки информации*");
                    nnThread.Start();//Создаем и запускаем поток для анализа данных
                    CreateJournalLines("*Поток для обработки информации запущен*");
                }
                CreateJournalLines("*Рабочий день завершен* + \n" +
                                    "##############################");
            }
            catch (Exception ex)
            {
                CreateJournalLines("*Ошибка, получено исключение:*" + ex.ToString());
            }
        }
        private static void GlobalHookKeyPress(object sender, KeyEventArgs e)//Обработчик собыьтия по вытягиванию нажатых клавиш
        {
            NNDataReceiving.keys += e.KeyData.ToString();
        }
        public static void CreateJournalLines(string lines)//Добавляет в файл журнала заданную строку
        {
            File.AppendAllText(pathOfJournalFile, DateTime.Now.ToLongTimeString() + $"   {lines}\n");
        }

        public static Client AuthUser(string[] authdata, out Cookie cookie) // метод для авторизации на сервере
        {
            Client returnproduct = default;
            try
            {
                CookieContainer cookies = new CookieContainer(); // авторизуем, отправляя Post запрос с нужными параметрами
                HttpClientHandler handler = new HttpClientHandler
                {
                    CookieContainer = cookies
                };
                HttpClient client = new HttpClient(handler)
                {
                    BaseAddress = new Uri(App.ServerAddress)
                };
                client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36");
                string json = new JavaScriptSerializer().Serialize(authdata);
                HttpResponseMessage response = client.PostAsync($"auth", new StringContent(json, Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();
                returnproduct = response.Content.ReadAsAsync<Client>().GetAwaiter().GetResult();
                try // Получаем куки
                {
                    Uri uri = new Uri($"{App.ServerAddress}auth");
                    var collection = cookies.GetCookies(uri);
                    cookie = collection[".AspNetCore.Cookies"];
                }
                catch (Exception) // если что-то пойдет не так во время получения куки
                {
                    cookie = default;
                }
            }
            catch (Exception ex)
            {
                // обработай исключение
                cookie = default;
            }
            return returnproduct;
        }
    }
}

