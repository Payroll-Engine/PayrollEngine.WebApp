FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy solution and project files
COPY ["PayrollEngine.WebApp.sln", "./"]
COPY ["Core/PayrollEngine.WebApp.Core.csproj", "Core/"]
COPY ["Presentation/PayrollEngine.WebApp.Presentation.csproj", "Presentation/"]
COPY ["Server/PayrollEngine.WebApp.Server.csproj", "Server/"]
COPY ["Shared/PayrollEngine.WebApp.Shared.csproj", "Shared/"]
COPY ["ViewModel/PayrollEngine.WebApp.ViewModel.csproj", "ViewModel/"]

# copy Directory.Build.props
COPY ["Directory.Build.props", "./"]

RUN dotnet restore "PayrollEngine.WebApp.sln"

# copy everything else
COPY . .
WORKDIR "/src/Server"
RUN dotnet publish "PayrollEngine.WebApp.Server.csproj" -c Release -o /app/publish --no-restore

# final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PayrollEngine.WebApp.Server.dll"] 