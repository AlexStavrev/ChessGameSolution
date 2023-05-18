#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["GameManager/GameManager.csproj", "GameManager/"]
COPY ["SharedDTOs/SharedDTOs.csproj", "SharedDTOs/"]
RUN dotnet restore "GameManager/GameManager.csproj"
COPY . .
WORKDIR "/src/GameManager"
RUN dotnet build "GameManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GameManager.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameManager.dll"]