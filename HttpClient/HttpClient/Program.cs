using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandsLibrary;

namespace HttpClient
{
    class Program
    {
        static void Main(string[] args)
        {
			try
			{
				int id = HttpCommands.createUserNameAndGetIdAsync().Result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
        }
    }
}
