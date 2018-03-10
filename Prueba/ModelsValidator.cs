using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace naturalmente.Models
{
    public class ModelsValidator
    {
        private static String[] typeValidation = new String[] { "required", "min", "max", "email", "uri" };

        public static JObject Validate(Object model)
        {
            CultureInfo actualLenguaje = Thread.CurrentThread.CurrentCulture;
            JToken messages = GetValidationLang(actualLenguaje.TwoLetterISOLanguageName);

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
                                    propm.Add(String.Format(messages["required"].ToString(), GetAlias((JObject)val[prop.Name], actualLenguaje.TwoLetterISOLanguageName, prop.Name)));
                                }
                                break;
                            case "min":
                                if (!isNull)
                                {
                                    String err = IsMinValid(prop, model, Convert.ToDecimal(val[prop.Name][validationReg].ToString()), (JObject)messages["min"], GetAlias((JObject)val[prop.Name], actualLenguaje.TwoLetterISOLanguageName));
                                  
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
                                    String err = IsMaxValid(prop, model, Convert.ToDecimal(val[prop.Name][validationReg].ToString()), (JObject)messages["max"], GetAlias((JObject) val[prop.Name], actualLenguaje.TwoLetterISOLanguageName));
                                    
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
                                        propm.Add(String.Format(messages["email"].ToString(), GetAlias((JObject)val[prop.Name], actualLenguaje.TwoLetterISOLanguageName, prop.Name)));
                                    }
                                }
                                break;
                            case "uri":
                                if (!isNull)
                                {
                                    if (!IsUriValid(prop, model, Convert.ToBoolean(val[prop.Name][validationReg].ToString())))
                                    {
                                        error_count = true;

                                        propm.Add(String.Format(messages["uri"].ToString(), GetAlias((JObject)val[prop.Name], actualLenguaje.TwoLetterISOLanguageName, prop.Name)));
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
                }
                else if (String.IsNullOrEmpty(property.GetValue(model).ToString()))
                {
                    return false;
                }



            }
            return true;
        }

        private static string IsMinValid(PropertyInfo property, Object model, decimal min, JObject messages, String alias = null)
        {
            if (property.PropertyType == typeof(String))
            {
                if (property.GetValue(model).ToString().Length < Convert.ToInt32(min))
                {
                    if (alias != null)
                        return String.Format(messages["string"].ToString(), alias, Convert.ToInt32(min));
                    else
                        return String.Format(messages["string"].ToString(), property.Name, Convert.ToInt32(min));
                }
            }
            else if (property.PropertyType == typeof(float) || property.PropertyType == typeof(decimal))
            {
                var value = Convert.ToDecimal(property.GetValue(model));
                if (value <= min)
                {
                    if (alias != null)
                        return String.Format(messages["float-decimal"].ToString(), alias, min);
                    else
                        return String.Format(messages["float-decimal"].ToString(), property.Name, min);
                }
            }
            else if (property.PropertyType == typeof(int))
            {
                if (Convert.ToInt32(property.GetValue(model)) <= Convert.ToInt32(min))
                {
                    if (alias != null)
                        return String.Format(messages["int"].ToString(), alias, Convert.ToInt32(min));
                    else
                        return String.Format(messages["int"].ToString(), property.Name, Convert.ToInt32(min));
                }
            }
            else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
            {
                var list = property.GetValue(model);
                if (int.Parse(list.GetType().GetProperty("Count").GetValue(list).ToString()) < Convert.ToInt32(min))
                {
                    if (alias != null)
                        return String.Format(messages["list-dictionary"].ToString(), alias, Convert.ToInt32(min));
                    else
                        return String.Format(messages["list-dictionary"].ToString(), property.Name, Convert.ToInt32(min));
                }
            }
            return null;
        }

        private static string IsMaxValid(PropertyInfo property, Object model, decimal max, JObject messages, String alias = null)
        {
            if (property.PropertyType == typeof(string))
            {
                if (property.GetValue(model).ToString().Length > Convert.ToInt32(max))
                {
                    if (alias != null)
                        return String.Format(messages["string"].ToString(), alias, Convert.ToInt32(max));
                    else
                        return String.Format(messages["string"].ToString(), property.Name, Convert.ToInt32(max));
                }
            }
            else if (property.PropertyType == typeof(float) || property.PropertyType == typeof(decimal))
            {
                var value = Convert.ToDecimal(property.GetValue(model));
                if (value > max)
                {
                    if (alias != null)
                        return String.Format(messages["float-decimal"].ToString(), alias, max);
                    else
                        return String.Format(messages["float-decimal"].ToString(), property.Name, max);
                }
            }
            else if (property.PropertyType == typeof(int))
            {
                if (Convert.ToInt32(property.GetValue(model)) > Convert.ToInt32(max))
                {
                    if (alias != null)
                        return String.Format(messages["int"].ToString(), alias, Convert.ToInt32(max));
                    else
                        return String.Format(messages["int"].ToString(), property.Name, Convert.ToInt32(max));
                }
            }
            else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
            {
                var list = property.GetValue(model);
                if (int.Parse(list.GetType().GetProperty("Count").GetValue(list).ToString()) > Convert.ToInt32(max))
                {
                    if (alias != null)
                        return String.Format(messages["list-dictionary"].ToString(), alias, Convert.ToInt32(max));
                    else
                        return String.Format(messages["list-dictionary"].ToString(), property.Name, Convert.ToInt32(max));
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
                catch
                {
                    return false;
                }
            }
            return false;
        }

        private static bool IsUriValid(PropertyInfo property, Object model, bool isUri)
        {
            Uri a = null;

            return Uri.TryCreate(property.GetValue(model).ToString(), UriKind.Absolute, out a);
        }

        private static JToken GetValidationModel(String modelName)
        {
            JObject jObject = JObject.Parse(File.ReadAllText(Directory.GetCurrentDirectory() + "/Validation.json"));
            return jObject[modelName];
        }

        private static JToken GetValidationLang(String actual)
        {
            JObject jObject = JObject.Parse(File.ReadAllText(Directory.GetCurrentDirectory() + "/Validations-Langs.json"));
            return jObject[actual];
        }

        private static String GetAlias(JObject property, String lang, String propName = null)
        {
            try
            {
                return property["lang"][lang].ToString();
            }
            catch
            {
                try
                {
                    return property["only-lang"].ToString();
                }
                catch
                {
                    if (propName != null)
                        return propName;

                    return null;
                }
            }
        }

    }
}