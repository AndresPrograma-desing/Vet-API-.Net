$RutaArchivo = "Ruta"

$CodigoReal = Get-Content -Raw -Path $RutaArchivo


$Pregunta = @"
...
$CodigoReal
"@


& "Ruta" run qwen2.5-coder $Pregunta
