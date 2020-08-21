using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CommandsLibrary;
using System.Windows;

namespace HttpClientRebirth
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Logic.Start(1);
        }
    }
    public static class Logic
    { 
        public static void Start(int idofpc)
        {

            idofpc = HttpCommands.createUserNameAndGetIdAsync().Result; /*Отправляем на API имя данного ПК и
            получает уникальный id, по которому будут проверяться входящие команды*/
            try
            {
                throw new Exception();

            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {

            }
        }

    }
}
