# Model Validation
Cree este proyecto tratando de salir de una problematica en C# en el framework .NET Core, el cual cuando uno trataba de validar mediante validacion de modelo, este no lo hacia. 

# Pasos para que funcione correctamente:

# Dependencias:
  1-Newtonsoft.Json
  
# Tipo de validaciones
	-Requerido(required), valida todos los tipos de datos, hasta listas.
	-Minimo(min), valida Strings, Integers, Flots, Decimals y Listas.
	-Maximo(max), valida Strings, Integers, Flots, Decimals y Listas.
	-Email(email), valida Strings.
	-URIs(uri), valida Strings.
	-Diferentes lenguajes para nombrar a la propiedad o ponerle un apodo.

# Codigo:
  1-En la razi del proyecto se debe crear un archivo llamado Validation.json con la sigiente estructura:
  {
  	"NombreDelModeloAValidar":{
		"Propiedad1":{
			"required":true,
			"min": 10
		},
		"Propiedad2":{...}
	},
	....
  }

  2-A travez del metodo Validate(Model), le pasamos el modelo a validar
  
  3-Se obtendra un JObject con una estructura asi:
  {
    NombreAtributo:[ "Error 1" ]
  }


# Multiples lenguajes o Apodos
En caso de necesitar que sea necesario usar varios lenguajes para darles un nuevo apodo a la propiedad es necesario añadir al archivo
"Validation.json" en la propiedad la key "lang"(por defecto los lenguajes soportados son Español e Ingles):
- "Password": {
-			"required": true,
-			"min": 8,
-			"lang": {
-				"es": "Contraseña",
-				"en": "Password"
-			}
-		}

- En el caso de que se desee añadir mas lenguajes, se debera editar el  archivo "Validations-Langs", añadiendo el lenguaje que se desea.
