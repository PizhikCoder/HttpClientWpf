using System;
using System.IO;
using System.Threading.Tasks;
using CommandsLibrary;
using System.Windows;
using System.Diagnostics;

namespace HttpClientRebirth
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            int idofpc = HttpCommands.createUserNameAndGetIdAsync().Result;/*Отправляем на API имя данного ПК и
            получает уникальный id, по которому будут проверяться входящие команды*/
            Logic.Start(idofpc);
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
