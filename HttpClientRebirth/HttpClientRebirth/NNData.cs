using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace HttpClientRebirth
{
    class Values
    {
        public static float MouseInfo = -1;
        public static float KeyBoardInfo = -1;
        public static float ProcessesInfo = -1;
    }
    public class NNDataReceiving
    {
        public static Stopwatch watch = new Stopwatch();
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
        public async  static void MouseInfoReceiving()
        {
                int counter = 15;
                Point pos1;
                Point pos2;
                while (watch.ElapsedMilliseconds <= 30000)
                {
                    pos1 = Cursor.Position;
                    Thread.Sleep(2000);
                    pos2 = Cursor.Position;
                    if (pos1 != pos2)
                    {
                        counter--;
                    }
                }
                if (counter <= 0)
                {
                    Values.MouseInfo = 1;
                }
                else
                {
                    Values.MouseInfo = 0;
                }
                
        }
        public async static void KeyBoardInfoReceiving()
        {
                string keyspressed = null;
                int gameKeysPressings;
                while (watch.ElapsedMilliseconds <= 29300)
                {
                    keyspressed += Console.ReadKey().Key;
                }

                gameKeysPressings = keyspressed.Length / 2;

                for (int i = 0; i < keyspressed.Length; i++)
                {
                    switch (keyspressed[i]) //Проверка нажатых клавишь на схожесть с клавишами управления игры(не более половины всех клавишь)
                    {
                        case 'W':
                            gameKeysPressings--;
                            break;
                        case 'A':
                            gameKeysPressings--;
                            break;
                        case 'S':
                            gameKeysPressings--;
                            break;
                        case 'D':
                            gameKeysPressings--;
                            break;
                        case 'w':
                            gameKeysPressings--;
                            break;
                        case 'a':
                            gameKeysPressings--;
                            break;
                        case 's': 
                            gameKeysPressings--;
                            break;
                        case 'd':
                            gameKeysPressings--;
                            break;
                        default:
                            continue;
                    }
                    if (gameKeysPressings == 0)
                    {
                        Values.KeyBoardInfo = 0;
                        break;
                    }
                }
                if (Values.KeyBoardInfo == -1)
                {
                    Values.KeyBoardInfo = 1;
                }
        }
        public async static void ProcessesIndoReceiving()
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
                }
        }
    }
    public class NN
    {
        public static float Start()
        {
            NNDataReceiving.watch.Start();
            Thread thr1 = new Thread(new ThreadStart(NNDataReceiving.ProcessesIndoReceiving));
            Thread thr2 = new Thread(new ThreadStart(NNDataReceiving.MouseInfoReceiving));
            Thread thr3 = new Thread(new ThreadStart(NNDataReceiving.KeyBoardInfoReceiving));
            thr1.Start();
            thr2.Start();
            //thr3.Start();
            Thread.Sleep(30000);
            NNDataReceiving.ValuesChecker();
            thr1.Abort();
            thr2.Abort();
            //thr3.Abort();
            NNDataReceiving.watch.Stop();
            return NNLibrary.Class1.LaunchNN(Values.MouseInfo, Values.KeyBoardInfo, Values.ProcessesInfo);
        }
    }
}
