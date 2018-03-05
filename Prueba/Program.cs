using Prueba.Models;
using System;
using System.Collections.Generic;

namespace Prueba
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Creando login");
            Login login = new Login()
            {
                Email = "nikoxd123@gmail.com",
                Password = "ac",
                Pedos = new List<string>()
            };
            Console.WriteLine("Login Creado");
            var valid = login.Validate();
            if(valid != null) {
                Console.WriteLine(valid["Password"][0].ToString());
            }
            Console.Read();
        }
    }
}
