# docker build . -f Dockerfile.jj -t tim-container && docker run -p 8080:8080 tim-container
# curl -i localhost:8080/api/Guest/getGuest/1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /src
COPY . .
RUN dotnet restore "csrs_dev.csproj"
RUN dotnet build "csrs_dev.csproj" -c Release -o /app/build
RUN dotnet publish "csrs_dev.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 as final
WORKDIR /app
COPY --from=base /app/publish .
ENTRYPOINT ["dotnet", "csrs_dev.dll"]