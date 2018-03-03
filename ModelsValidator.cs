using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
/** Este codigo fue creado por Nicolas Morena */
namespace NicolasMorena.Models
{
    public class ModelsValidator
    {
		private static String[] typeValidation = new String[] { "required", "min", "max" };

		public static JObject Validate(Object model)
		{
			JObject errs = new JObject();

			Type modelType = model.GetType();
						
			MethodInfo modelMethod = modelType.GetMethod("GetValidation");
			JObject val = (JObject) modelMethod.Invoke(model, null);

			foreach(var prop in modelType.GetProperties())
			{
				bool error_count = false;
				JObject propm = new JObject();
				bool isNull = false;
				foreach(String validationReg in typeValidation)
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
									propm.Add(prop.Name, "El campo " + prop.Name + " no puede ser nulo.");
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
						}
					}
					catch (Exception e)
					{

					}	
				}
				if (error_count)
				{
					errs.Add(prop.Name, propm);
				}
			}
			
			if(errs.Count > 0)
			{
				return errs;
			}
			return null;
		}


		private static bool IsRequiredValid(PropertyInfo property, Object model, bool isRequired)
		{
			if (isRequired)
			{
				if (String.IsNullOrEmpty(property.GetValue(model).ToString()))
				{
					return false;
				}
			}
			return true;
		}

		private static string IsMinValid(PropertyInfo property, Object model, decimal min)
		{
			if(property.GetType() == typeof(string))
			{
				if(property.GetValue(model).ToString().Length < Convert.ToInt32(min))
				{
					return "El campo" + property.Name + "debe tener como minimo " + Convert.ToInt32(min) + " carateres.";
				}
			}else if(property.GetType() == typeof(float) || property.GetType() == typeof(decimal))
			{
				var value = Convert.ToDecimal(property.GetValue(model));
				if(value < min)
				{
					return "El campo" + property.Name + "debe tener un valor mayor o igual a " + min;
				}
			}else if(property.GetType() == typeof(int))
			{
				if (Convert.ToInt32(property.GetValue(model)) < Convert.ToInt32(min))
				{
					return "El campo" + property.Name + "debe tener un valor mayor o igual a " + Convert.ToInt32(min);
				}
			}
			return null;
		}

		private static string IsMaxValid(PropertyInfo property, Object model, decimal max)
		{
			if (property.GetType() == typeof(string))
			{
				if (property.GetValue(model).ToString().Length < Convert.ToInt32(max))
				{
					return "El campo" + property.Name + "debe tener como maximo " + Convert.ToInt32(max) + " carateres.";
				}
			}
			else if (property.GetType() == typeof(float) || property.GetType() == typeof(decimal))
			{
				var value = Convert.ToDecimal(property.GetValue(model));
				if (value < max)
				{
					return "El campo" + property.Name + "debe tener un valor menor o igual a " + max;
				}
			}
			else if (property.GetType() == typeof(int))
			{
				if (Convert.ToInt32(property.GetValue(model)) < Convert.ToInt32(max))
				{
					return "El campo" + property.Name + "debe tener un valor menor o igual a " + Convert.ToInt32(max);
				}
			}
			return null;
		}
	}
}
