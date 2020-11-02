using Gma.System.MouseKeyHook;
using KeyEventArgs =  System.Windows.Forms.KeyEventArgs;
using NNMethods;
using NNDataFunctions;
using System.Windows;
using System.Threading;
using System.IO;
using System;

namespace HTTP_WPF_Client_Project
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string pathOfJournalFile = "";//Путь к файлу журнала программы
        public static IKeyboardMouseEvents _globalHook;
        private void Application_StartUp(object sender, StartupEventArgs e)
        {
            try
            {
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

                if (_globalHook == null)//Оформляем событие по вытягиванию нажатых клавиш
                {
                    _globalHook = Hook.GlobalEvents();
                    _globalHook.KeyDown += GlobalHookKeyPress;
                }

                Thread nnThread = new Thread(new ThreadStart(NNDataGettingControl.Start));
                CreateJournalLines("*Запускается поток для обработки информации*");
                nnThread.Start();//Создам и запускаем поток для анализа данных
                CreateJournalLines("*Поток для обработки информации запущен*");
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
    }
}

