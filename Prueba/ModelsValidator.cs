using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;

namespace naturalmente.Models
{
    public class ModelsValidator
    {
        private static String[] typeValidation = new String[] { "required", "min", "max", "email" };

        public static JObject Validate(Object model)
        {
            JObject errs = new JObject();

            Type modelType = model.GetType();

            JToken val = GetValidationModel(modelType.Name);

            foreach (var prop in modelType.GetProperties())
            {
                bool error_count = false;
                JArray propm = new JArray();

                bool isNull = false;
                foreach (String validationReg in typeValidation)
                {
                    try
                    {
                        switch (validationReg)
                        {
                            case "required":
                                if (!IsRequiredValid(prop, model, Convert.ToBoolean(val[prop.Name][validationReg].ToString())))
                                {
                                    error_count = true;
                                    isNull = true;
                                    JToken jToken = "El campo " + prop.Name + " no puede ser nulo.";
                                    propm.Add(jToken);
                                }
                                break;
                            case "min":
                                if (!isNull)
                                {
                                    var err = IsMinValid(prop, model, Convert.ToDecimal(val[prop.Name][validationReg].ToString()));
                                    if (!String.IsNullOrEmpty(err))
                                    {
                                        error_count = true;
                                        propm.Add(err);
                                    }
                                }
                                break;
                            case "max":
                                if (!isNull)
                                {
                                    var err = IsMinValid(prop, model, Convert.ToDecimal(val[prop.Name][validationReg].ToString()));
                                    if (!String.IsNullOrEmpty(err))
                                    {
                                        error_count = true;
                                        propm.Add(err);
                                    }
                                }
                                break;
                            case "email":
                                if (!isNull)
                                {
                                    if (!IsValidEmail(prop, model, Convert.ToBoolean(val[prop.Name][validationReg].ToString())))
                                    {
                                        error_count = true;
                                        propm.Add("El campo " + prop.Name + " no es un email valido.");
                                    }
                                }
                                break;
                        }
                    }
                    catch
                    {

                    }
                }
                if (error_count)
                {
                    errs.Add(prop.Name, propm);
                }
            }

            if (errs.Count > 0)
            {
                return errs;
            }
            return null;
        }


        private static bool IsRequiredValid(PropertyInfo property, Object model, bool isRequired)
        {
            if (isRequired)
            {
                Console.Write(property.PropertyType.IsGenericType);
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
                {
                    var list = property.GetValue(model);
                    if (list != null)
                    {
                        if (int.Parse(list.GetType().GetProperty("Count").GetValue(list).ToString()) == 0)
                        {
                            return false;
                        }
                    }
                    return false;
                }else if (String.IsNullOrEmpty(property.GetValue(model).ToString()))
                {
                    return false;
                }
                


            }
            return true;
        }

        private static string IsMinValid(PropertyInfo property, Object model, decimal min)
        {
            if (property.PropertyType == typeof(String))
            {
                if (property.GetValue(model).ToString().Length < Convert.ToInt32(min))
                {
                    return "El campo " + property.Name + " debe tener como minimo " + Convert.ToInt32(min) + " carateres.";
                }
            }
            else if (property.PropertyType == typeof(float) || property.PropertyType == typeof(decimal))
            {
                var value = Convert.ToDecimal(property.GetValue(model));
                if (value < min)
                {
                    return "El campo " + property.Name + " debe tener un valor mayor o igual a " + min;
                }
            }
            else if (property.PropertyType == typeof(int))
            {
                if (Convert.ToInt32(property.GetValue(model)) < Convert.ToInt32(min))
                {
                    return "El campo " + property.Name + " debe tener un valor mayor o igual a " + Convert.ToInt32(min);
                }
            }
            else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
            {
                var list = property.GetValue(model);
                if (int.Parse(list.GetType().GetProperty("Count").GetValue(list).ToString()) < Convert.ToInt32(min))
                {
                    return "El campo " + property.Name + " debe contener mas de " + Convert.ToInt32(min) + " elementos";
                }
            }
            return null;
        }

        private static string IsMaxValid(PropertyInfo property, Object model, decimal max)
        {
            if (property.PropertyType == typeof(string))
            {
                if (property.GetValue(model).ToString().Length < Convert.ToInt32(max))
                {
                    return "El campo " + property.Name + " debe tener como maximo " + Convert.ToInt32(max) + " carateres.";
                }
            }
            else if (property.PropertyType == typeof(float) || property.PropertyType == typeof(decimal))
            {
                var value = Convert.ToDecimal(property.GetValue(model));
                if (value < max)
                {
                    return "El campo " + property.Name + " debe tener un valor menor o igual a " + max;
                }
            }
            else if (property.PropertyType == typeof(int))
            {
                if (Convert.ToInt32(property.GetValue(model)) < Convert.ToInt32(max))
                {
                    return "El campo " + property.Name + " debe tener un valor menor o igual a " + Convert.ToInt32(max);
                }
            }
            else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
            {
                var list = property.GetValue(model);
                if (int.Parse(list.GetType().GetProperty("Count").GetValue(list).ToString()) > Convert.ToInt32(max))
                {
                    return "El campo " + property.Name + " debe contener un maximo de " + Convert.ToInt32(max) + " elementos";
                }
            }
            return null;
        }

        private static bool IsValidEmail(PropertyInfo property, Object model, bool isEmail)
        {
            if (isEmail)
            {
                try
                {
                    MailAddress m = new MailAddress(property.GetValue(model).ToString());
                    return true;

                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }



        private static JToken GetValidationModel(String modelName)
        {
            JObject jObject = JObject.Parse(File.ReadAllText(Directory.GetCurrentDirectory() + "/Validation.json"));
            return jObject[modelName];
        }
    }
}
