using Prueba.Models;
using System;
using System.Collections.Generic;

namespace Prueba
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentCulture = ci;

            Console.WriteLine("Creando login");
            Login login = new Login()
            {
                Email = "nikoxd123@gmail.com",
                Password = "ac",
                UriMercado = "a",
                Megas = "casc"
            };
            Console.WriteLine("Login Creado");
            var valid = login.Validate();
            if(valid != null) {
                Console.WriteLine(valid.ToString());
            }
            Console.Read();
        }
    }
}
