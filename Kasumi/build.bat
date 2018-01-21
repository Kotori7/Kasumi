git rev-parse --short HEAD > version
git pull
dotnet build /m /p:Configuration=Release