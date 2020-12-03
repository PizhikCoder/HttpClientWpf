using System;
using NNMethods;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using HTTP_WPF_Client_Project;
using Microsoft.AspNetCore.SignalR.Client;
using ConnectionCommands;

namespace NNDataFunctions
{
    class KeyPressedInfo
    {
        public string Key { get; set; }
        public int PressedCount { get; set; }
    }
    class Report
    {
        public Client Client { get; set; }
        public float OverallRating { get; set; }
        public float KeyBoardRating { get; set; }
        public int MouseRating { get; set; }
        public int ProcessRating { get; set; }

        public ICollection<KeyPressedInfo> KeyPressedInfo { get; set; }
        public int pressingCount { get; set; }
        public bool isMouseCoordChanged { get; set; }
        public int ProcessChangedCount { get; set; }
        public ICollection<string> OldProcesses { get; set; }
        public ICollection<string> LastProcesses { get; set; }

    }
    class Values//Класс с текущими собранными данными
    {
        public static float OverallRating;

        public static float MouseInfo = -1;
        public static float KeyBoardInfo = -1;
        public static float ProcessesInfo = -1;

        public static int ProcessesChangedCount = 0;
        public static int MouseCoordinatesChangedCount = 0;
        public static bool isMouseCoordChanged = false;
        public static List<KeyPressedInfo> keyPressedInfos = new List<KeyPressedInfo> { };
        public static int pressingCount { get; set; }
        public static ICollection<string> OldProcesses { get; set; }
        public static ICollection<string> LastProcesses { get; set; }
    }

    class NNDataReceiving
    {
        public static Stopwatch watch = new Stopwatch();
        public static List<string> keys = new List<string>();

        public static string keyspressed = "";//Строка нажатых клавиш
        public static int waitingTime = 30000;
        public static int keyboardCoefficient = 30;

        public static void ValuesChecker() //Проверка, заполнены ли все значения
        {
            bool key = false;
            while (!key)
            {
                if (Values.MouseInfo != -1 && Values.KeyBoardInfo != -1 && Values.ProcessesInfo != -1)
                {
                    key = true;
                    watch.Reset();
                }
            }
        }
        public static void MouseInfoReceiving()
        {
            int counter = 15;//Счетчик того, сколько раз изменилась позиция курсора мыши
            Point pos1;
            Point pos2;
            while (watch.ElapsedMilliseconds <= waitingTime)
            {
                pos1 = Cursor.Position;//Получаем начальные координаты курсора
                Thread.Sleep(2000);
                pos2 = Cursor.Position;//Получаем конечные координаты курсора
                if (pos1 != pos2)//Если координаты мыши изменились, уменьшаем значение счетчика на 1
                {
                    counter--;
                }
            }
            if (counter <= 0)//Если значение счетчика меньше либо равно нулю, то работа мыши в текущей итерации засчитывается
            {
                Values.MouseInfo = 1;
                Values.isMouseCoordChanged = true;
            }
            else
            {
                Values.MouseInfo = 0;
            }
            Values.MouseCoordinatesChangedCount = Math.Abs(counter - 15);
        }
        public static void KeyBoardInfoReceiving()
        {
            while (watch.ElapsedMilliseconds <= waitingTime)
            {
            }
            List<string> keysCopy = keys;
            int allowableKeyRepeatCount = keys.Count / 2;//Количество допускаемых повторений символа
            float rating;
            int counterOfGameControlKeys = keys.Count / 2;//Счетчик, который равен половине от всех нажатых клавиш, он отсчитывает количество нажатых клавиш, которые отвечают за управление игровым процессом
            keysCopy = keysCopy.Distinct().ToList();
            foreach (string ch in keysCopy)//Перебирает символы в массиве 
            {
                KeyPressedInfo keyPressed = new KeyPressedInfo
                {
                    Key = ch,//Записывает значение текущего символа
                    PressedCount = keys.Count(c => c == ch)//Записывает сколько символов равных текущему ch существует в массиве
                };
                Values.keyPressedInfos.Add(keyPressed);//Добавляет экземпляр класса KeyPressedInfo  в коллекцию 
                if (keysCopy.Count(x => x == ch) >= allowableKeyRepeatCount)//Если число нажатий 1 симпола выше допустимого значения
                {
                    counterOfGameControlKeys = 0;//Создаем ситуацию, в которой рейтинг клавиатуры будет 0
                }
            }
            counterOfGameControlKeys = counterOfGameControlKeys - keys.Count(c => c == "W")
                - keys.Count(c => c == "A") - keys.Count(c => c == "S")
                - keys.Count(c => c == "D"); //Вычетаем из счетчика нажатий игровых клавишь(которых допускается не более половины от всех клавиш) количество нажатых игровых клавиш
            if (counterOfGameControlKeys <= 0)
            {
                Values.KeyBoardInfo = 0;
            }
            else
            {
                rating = (float)keys.Count / keyboardCoefficient;//Получение отношения числа нажатых клавиш к заданному коэффициенту
                if (rating >= 1)
                {
                    Values.KeyBoardInfo = 1;
                }
                else
                {
                    Values.KeyBoardInfo = (float)Math.Round(rating, 1);
                }
            }
            Values.pressingCount = keys.Count;
            keys = new List<string>();//Обнуляем список нажатых символов для следующей итерации
        }
        
        public static void ProcessesInfoReceiving()
        {
            Process[] processes1 = Process.GetProcesses().Where(c => (int)c.MainWindowHandle != 0).ToArray();
            string[] processesString1 = new string[processes1.Length];
            Process[] processes2;
            for (int i = 0; i < processes1.Length; i++)
            {
                processesString1[i] = processes1[i].ProcessName;//Создает массив строк с именами процессов
            }
            while (watch.ElapsedMilliseconds <= waitingTime)
            {
            }
            processes2 = Process.GetProcesses().Where(c => (int)c.MainWindowHandle != 0).ToArray();
            string[] processesString2 = new string[processes2.Length];
            for (int i = 0; i < processes2.Length; i++)
            {
                processesString2[i] = processes2[i].ProcessName;//Создает массив строк с именами процессов
            }
            if (Enumerable.SequenceEqual(processesString1, processesString2))
            {
                Values.ProcessesInfo = 0;
                Values.OldProcesses = processesString1;//Сохраняем полученные данные
                Values.LastProcesses = processesString2;
                Values.ProcessesChangedCount = Math.Abs(processes1.Length - processes2.Length);
            }
            else
            {
                Values.ProcessesInfo = 1;
                Values.OldProcesses = processesString1;//Сохраняем полученные данные
                Values.LastProcesses = processesString2;
                Values.ProcessesChangedCount = Math.Abs(processes1.Length - processes2.Length);
            }
        }
    }

    class NNDataGettingControl:Values
    {
        public static void Start()//Запускает сбор информации, загружает ее в NN и вызывает метод составления и отправки отчета
        {
            float nnResult = 0;
            NNDataReceiving.watch.Start();//Запускает секундомер для фиксации времени работы классов по сбору информации
            Thread thr1 = new Thread(new ThreadStart(NNDataReceiving.ProcessesInfoReceiving));//Поток сбора информации о процессах
            Thread thr2 = new Thread(new ThreadStart(NNDataReceiving.KeyBoardInfoReceiving));//Поток для сбора информации о нажатых клавишах
            thr1.Start();
            thr2.Start();
            App.CreateJournalLines("*Сбор информации начался*");
            NNDataReceiving.MouseInfoReceiving();//Сбор информации о движении мыши(выполняется в основном потоке)
            NNDataReceiving.ValuesChecker();//Проверяет вся ли информация собрана, если не вся, то ожидает, пока сбор завершится и обнуляет таймер
            App.CreateJournalLines("*Сбор информации завершен*");
            NNDataReceiving.watch.Stop();
            App.CreateJournalLines("*Основная сводка полученных данных:\n" +
                                   $"                                   MouseInfo: {Values.MouseInfo}\n" +
                                   $"                                   KeyBoardInfo: {Values.KeyBoardInfo}\n"+
                                   $"                                   ProcessesInfo: {Values.ProcessesInfo}*");
            nnResult = NN.GetDataFromNN(MouseInfo, KeyBoardInfo, ProcessesInfo);
            Values.OverallRating = nnResult;
            App.CreateJournalLines($"*Оценка нейронной сети: {nnResult}*");
                App.CreateJournalLines("*Начат процесс создания отчета...*");
                ReportCreator();
            App.CreateJournalLines("*Отчет отправлен*");
            App.CreateJournalLines("*Цикл работы завершен*");
            App.isDataThreadWorking = false; //Указываем, что поток сбора и обработки данных завершил свою работу
        }
        private async static void ReportCreator()//Составляет отчет и отправляет его
        {
            App.CreateJournalLines("*Отправка отчета...*");
            Report report = new Report()
            {
                Client = App.clientData,
                OverallRating = Values.OverallRating,
                KeyBoardRating = Values.KeyBoardInfo,
                MouseRating = (int)Values.MouseInfo,
                ProcessRating = (int)Values.ProcessesInfo,
                KeyPressedInfo = Values.keyPressedInfos,
                pressingCount = Values.pressingCount,
                isMouseCoordChanged = Values.isMouseCoordChanged,
                ProcessChangedCount = Values.ProcessesChangedCount,
                OldProcesses = Values.OldProcesses,
                LastProcesses = Values.LastProcesses
            };
            App.CreateJournalLines("*Отчет создан*");
            App.CreateJournalLines("*Отправка*");
            Commands.Connection.InvokeAsync("SendReport", report).Wait();
            App.CreateJournalLines("*Отчет отправлен*");
            App.CreateJournalLines($"*Отчет о работе сотрудника(информация о нажатых клавишах и процессах на ПК скрыта):\n" +
                                   $"                                                                                   OverallRating: {Values.OverallRating}\n" +
                                   $"                                                                                   KeyBoardRating: {(int)Values.KeyBoardInfo}\n" +
                                   $"                                                                                   MouseRating: {(int)Values.MouseInfo}\n" +
                                   $"                                                                                   ProcessRating: {(int)Values.ProcessesInfo}\n" +
                                   $"                                                                                   isMouseCoordChanged: {Values.isMouseCoordChanged}\n" +
                                   $"                                                                                   ProcessChangedCount: {Values.ProcessesChangedCount}\n");

        }
    }
}