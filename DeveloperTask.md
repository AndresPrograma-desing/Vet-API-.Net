## Comando para generar migraciones

dotnet ef migrations add NombreDeLaMigracion

dotnet ef database update


## Comandos para crear la imagen de docker
docker build -t happy-pets-backend .
docker run -d -p 8080:8080 --name happy-pets-api happy-pets-backend

## Comandos para limpiar y crear la imagen de docker
docker container rm -f happy-pets-api
docker rmi happy-pets-backend
docker build -t happy-pets-backend .
docker run -d -p 8080:8080 --name happy-pets-api happy-pets-backend