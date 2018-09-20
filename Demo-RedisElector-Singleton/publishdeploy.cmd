dotnet publish -v d -o ".\bin\linux-x64" --framework netcoreapp2.1 --runtime linux-x64 Demo-RedisElector-Singleton.csproj 
cf push -f Manifest.yml