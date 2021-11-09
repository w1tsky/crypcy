#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["crypcy.stun/crypcy.stun.csproj", "crypcy.stun/"]
COPY ["crypcy.shared/crypcy.shared.csproj", "crypcy.shared/"]
RUN dotnet restore "crypcy.stun/crypcy.stun.csproj"
COPY . .
WORKDIR "/src/crypcy.stun"
RUN dotnet build "crypcy.stun.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "crypcy.stun.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "crypcy.stun.dll"]