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
                CheckingNewСommands.checks(idofpc);
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
