#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.CrPati/Tgs.GameRecord.CrPati.csproj", "Tgs.GameRecord.CrPati/"]
COPY ["Tgs.GameRecord.CrPati.Common/Tgs.GameRecord.CrPati.Common.csproj", "Tgs.GameRecord.CrPati.Common/"]
COPY ["Tgs.GameRecord.CrPati.Core/Tgs.GameRecord.CrPati.Core.csproj", "Tgs.GameRecord.CrPati.Core/"]
RUN dotnet restore "./Tgs.GameRecord.CrPati/./Tgs.GameRecord.CrPati.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.CrPati"
RUN dotnet build "./Tgs.GameRecord.CrPati.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameRecord.CrPati.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.CrPati.dll"]