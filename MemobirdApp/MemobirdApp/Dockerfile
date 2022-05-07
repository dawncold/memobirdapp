FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MemobirdApp/MemobirdApp.csproj", "MemobirdApp/"]
RUN dotnet restore "MemobirdApp/MemobirdApp.csproj"
COPY . .
WORKDIR "/src/MemobirdApp"
RUN dotnet build "MemobirdApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MemobirdApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MemobirdApp.dll"]
