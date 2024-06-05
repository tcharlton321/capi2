# docker build . -t csrs-dev:latest && docker run -p 8080:8080 csrs-dev:latest
# docker build . -t csrs-dev:latest && docker image push caesarsapi20240602161401.azurecr.io/csrs-dev:latest 

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /src
COPY . .
RUN dotnet restore "csrs_dev.csproj"
RUN dotnet build "csrs_dev.csproj" -c Release -o /app/build
RUN dotnet publish "csrs_dev.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 as final
WORKDIR /app
COPY --from=base /app/publish .
ENV ASPNETCORE_HTTP_PORTS=http://+:80
ENTRYPOINT ["dotnet", "csrs_dev.dll"]