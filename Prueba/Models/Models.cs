using naturalmente.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prueba.Models
{
    class Login
    {
        public String Email { get; set; }
        public String Password { get; set; }
        public String Megas { get; set; }
        public String UriMercado { get; set; }

        public JObject Validate()
        {
            return ModelsValidator.Validate(this);
        }
    }
}
