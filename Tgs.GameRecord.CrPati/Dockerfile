#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.CrPati/Tgs.GameRecord.CrPati.csproj", "Tgs.GameRecord.CrPati/"]
COPY ["Tgs.GameRecord.CrPati.Common/Tgs.GameRecord.CrPati.Common.csproj", "Tgs.GameRecord.CrPati.Common/"]
COPY ["Tgs.GameRecord.CrPati.Core/Tgs.GameRecord.CrPati.Core.csproj", "Tgs.GameRecord.CrPati.Core/"]
RUN dotnet restore "Tgs.GameRecord.CrPati/Tgs.GameRecord.CrPati.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.CrPati"
RUN dotnet build "Tgs.GameRecord.CrPati.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameRecord.CrPati.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.CrPati.dll"]