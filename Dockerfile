FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["AI-Sales Agent/AI-Sales Agent.csproj", "AI-Sales Agent/"]
RUN dotnet restore "AI-Sales Agent/AI-Sales Agent.csproj"

COPY . .
WORKDIR "/src/AI-Sales Agent"
RUN dotnet publish "AI-Sales Agent.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AI-Sales Agent.dll"]
