using System;
using NNMethods;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using HTTP_WPF_Client_Project;

namespace NNDataFunctions
{
    class KeyPressedInfo
    {
        public  char Key { get; set; }
        public  int PressedCount { get; set; }
    }
    class Report
    {
        public  float OverallRating { get; set; }
        public   int KeyBoardRating { get; set; }
        public  int MouseRating { get; set; }
        public  int ProcessRating { get; set; }

        public  List<KeyPressedInfo> KeyPressedInfo { get; set; }
        public  bool isMouseCoordChanged { get; set; }
        public  int ProcessChangedCount { get; set; }
        public Process[] OldProcesses { get; set; }
        public Process[] LastProcesses { get; set; }

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
        public static Process[] OldProcesses { get; set; }
        public static Process[] LastProcesses { get; set; }
    }

    class NNDataReceiving
    {
        public static Stopwatch watch = new Stopwatch();
        public static string keys = "";

        public static string keyspressed = "";//Строка нажатых клавиш


        public static void ValuesChecker() //Проверка, заполнены ли все значения
        {
            bool key = false;
            while (!key)
            {
                if (Values.MouseInfo != -1 && Values.KeyBoardInfo != -1 && Values.ProcessesInfo != -1)
                {
                    key = true;
                }
            }
        }
        public static void MouseInfoReceiving()
        {
            int counter = 15;//Счетчик того, сколько раз изменилась позиция курсора мыши
            Point pos1;
            Point pos2;
            while (watch.ElapsedMilliseconds <= 30000)
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
            while (watch.ElapsedMilliseconds <= 28000)
            {
            }
            string keysString = keys;
            int counterOfGameControlKeys = keysString.Length / 2;//Счетчик, который равен половине от всех нажатых клавиш, он отсчитывает количество нажатых клавиш, которые отвечают за управление игровым процессом
            char[] simbols = keysString.ToCharArray();//Разбивает строку на массив символов
            var uniqueSimbols = simbols.Distinct();//Удаляет в массиве повторяющиеся символы, оставляя только уникальные
            foreach  (char ch in uniqueSimbols)//Перебирает символы в массиве 
            {
                KeyPressedInfo keyPressed = new KeyPressedInfo
                {
                    Key = ch,//Записывает значение текущего символа
                    PressedCount = simbols.Count(c => c==ch)//Записывает сколько символов равных текущему ch существует в массиве
                };
                Values.keyPressedInfos.Add(keyPressed);//Добавляет экземпляр класса KeyPressedInfo  в коллекцию 
            }
            counterOfGameControlKeys = counterOfGameControlKeys - simbols.Count(c => c == 'W') 
                - simbols.Count(c => c == 'A') - simbols.Count(c => c == 'S') 
                - simbols.Count(c => c == 'D'); //Вычетаем из счетчика нажатий игровых клавишь(которых допускается не более половины от всех клавиш) количество нажатых игровых клавиш
            if ((counterOfGameControlKeys <= 0) || (keysString.Length <= 10)) 
            {
                Values.KeyBoardInfo = 0;
            }
            else
            {
                Values.KeyBoardInfo = 1;
            }
            keys = "";//Обнуляем строку нажатых символов для следующей итерации
        }
        
        public static void ProcessesInfoReceiving()
        {
            Process[] processes1 = Process.GetProcesses();
            Process[] processes2;
            while (watch.ElapsedMilliseconds <= 28500)
            {
            }
            processes2 = Process.GetProcesses();
            if (processes1 == processes2)
            {
                Values.ProcessesInfo = 0;
            }
            else
            {
                Values.ProcessesInfo = 1;
                Values.OldProcesses = processes1;
                Values.LastProcesses = processes2;
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
            NNDataReceiving.ValuesChecker();//Проверяет вся ли информация собрана, если не вся, то ожидает, пока сбор завершится
            App.CreateJournalLines("*Сбор информации завершен*");
            thr1.Abort();
            thr2.Abort();
            NNDataReceiving.watch.Stop();
            App.CreateJournalLines("*Основная сводка полученных данных:\n" +
                                   $"                                   MouseInfo: {Values.MouseInfo}\n" +
                                   $"                                   KeyBoardInfo: {Values.KeyBoardInfo}\n"+
                                   $"                                   ProcessesInfo: {Values.ProcessesInfo}*");
            nnResult = NN.GetDataFromNN(MouseInfo, KeyBoardInfo, ProcessesInfo);
            Values.OverallRating = nnResult;
            App.CreateJournalLines($"*Оценка нейронной сети: {nnResult}*");
            if (nnResult >= 0.5)//Проверка значения, предзказанного нейронной сетью. Если подозрение 1 - создается отчет, если 0 - не создается
            {
                App.CreateJournalLines("*Начат процесс создания отчета...*");
                ReportCreator();
                App.CreateJournalLines("*Отчет создан*");
            }
        }
        private static void ReportCreator()//Составляет отчет и отправляет его
        {
            Report report = new Report
            {
                OverallRating = Values.OverallRating,
                KeyBoardRating = (int)Values.KeyBoardInfo,
                MouseRating = (int)Values.MouseInfo,
                ProcessRating = (int)Values.ProcessesInfo,
                KeyPressedInfo = Values.keyPressedInfos,
                isMouseCoordChanged = Values.isMouseCoordChanged,
                ProcessChangedCount = Values.ProcessesChangedCount,
                OldProcesses = Values.OldProcesses,
                LastProcesses = Values.LastProcesses
            };
        }
    }
}
