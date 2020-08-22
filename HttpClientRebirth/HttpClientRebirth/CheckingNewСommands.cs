using System;
using CommandsLibrary;
using System.Net.Http;

namespace HttpClientRebirth
{
    class CheckingNewСommands
    {
        public static bool checks(int idofpc) //Проверка на поступление новых команд на API
        {
            while (true)
            {
                int idofcommand = Convert.ToInt32(HttpCommands.getCommandIdAsync(idofpc)); //Получаем id будущей комманды
                if (idofcommand != 0) //Проверка, есть ли комманды на сервере
                {
                    idofcommand -= 1;
                    
                }
                else
                {
                    continue;
                }

            }
        }
    }
}
