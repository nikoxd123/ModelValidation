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
        public List<String> Pedos { get; set; }

        public JObject Validate()
        {
            return ModelsValidator.Validate(this);
        }
    }
}
