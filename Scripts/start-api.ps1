Clear-Host
Write-Host "=== PANEL DE [ARRANQUE DE API] ===" -ForegroundColor Cyan
Write-Host "1. Arrancar API Normal"
Write-Host "2. Arrancar API con Auto-Reload (Watch)"
Write-Host "3. Compilar Proyecto (Build)"
Write-Host "4. Salir"
Write-Host "========================================="

$opcion = Read-Host "Elige una opcion (1-4)"

Set-Location "$PSScriptRoot"

switch ($opcion) {
    "1" { dotnet run --project ..\vet-api-Net.csproj }
    "2" { dotnet watch run --project ..\vet-api-Net.csproj }
    "3" { dotnet build ..\vet-api-Net.csproj }
    "4" { Write-Host "¡Good bye!" -ForegroundColor Yellow; exit }
    default { Write-Host "Opcion no valida." -ForegroundColor Red }
}
