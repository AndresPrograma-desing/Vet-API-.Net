$RutaArchivo = "C:\Users\Usuario\ProjectoUniversidad\vet-api-Net\Services\InvoiceService.cs"

$CodigoReal = Get-Content -Raw -Path $RutaArchivo


$Pregunta = @"
Actúa como un programador senior de .NET. Háblame solo en texto explicativo normal y en español.

Quiero que modifiques EXCLUSIVAMENTE la función 'GenerateInvoiceForCitaAsync' que está en el código de abajo para agregar logs de diagnóstico usando la variable '_logger' que ya está inyectada en la clase.

Por favor, añade logs estratégicos en:
- El inicio de la función (LogInformation con el citaId).
- Alerta si la cita no existe o si arroja la excepción de estado no completado (LogWarning).
- Alerta si la consulta no existe (LogWarning).
- Al finalizar con éxito la generación del DTO (LogInformation con el número de factura generado).

Devuélveme la función COMPLETA modificada dentro de un bloque de código markdown csharp para que pueda copiarla y reemplazarla fácilmente.

Este es el código del archivo:
$CodigoReal
"@


& "C:\Users\Usuario\AppData\Local\Programs\Ollama\ollama.exe" run qwen2.5-coder $Pregunta