#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ARTHS_API/ARTHS_API.csproj", "ARTHS_API/"]
COPY ["ARTHS_Service/ARTHS_Service.csproj", "ARTHS_Service/"]
COPY ["ARTHS_Data/ARTHS_Data.csproj", "ARTHS_Data/"]
COPY ["ARTHS_Utility/ARTHS_Utility.csproj", "ARTHS_Utility/"]
RUN dotnet restore "ARTHS_API/ARTHS_API.csproj"
COPY . .
WORKDIR "/src/ARTHS_API"
RUN dotnet build "ARTHS_API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ARTHS_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ARTHS_API.dll"]