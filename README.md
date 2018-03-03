# Model Validation
Cree este proyecto tratando de salir de una problematica en C# en el framework .NET Core, el cual cuando uno trataba de validar mediante validacion de modelo, este no lo hacia. 

Pasos para que funcione correctamente:

Dependencias:
  1-Newtonsoft.Json
  
Codigo:
  1-El modelo debe tener una funcion llamada GetValidation() donde retorne un JObject de esta caracteristica:
  public JObject GetValidation()
		{
			return JObject.Parse(@"
				{
    'Nombre':{
        'required':true,
        'min': 2,
        'max':150
    }
}
			");
		}

Se nombra a la propiedad a validar y por ultimo se agrega lo que se intenta validar.

  2-A travez del metodo Validate(Model), le pasamos el modelo a validar
  
  3-Se obtendra un JObject con una estructura asi:
  {
    NombreAtributo:[ "Error 1" ]
  }
