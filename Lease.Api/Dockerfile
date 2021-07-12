#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Lease.Api/Lease.Api.csproj", "Lease.Api/"]
RUN dotnet restore "Lease.Api/Lease.Api.csproj"
COPY . .
WORKDIR "/src/Lease.Api"
RUN dotnet build "Lease.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Lease.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lease.Api.dll"]