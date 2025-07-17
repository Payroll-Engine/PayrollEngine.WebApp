# Build stage with multi-platform support
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG TARGETARCH
ARG BUILDPLATFORM
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

# Restore with architecture-specific runtime
RUN if [ "$TARGETARCH" = "arm64" ]; then \
      dotnet restore "PayrollEngine.WebApp.sln" --runtime linux-arm64; \
    else \
      dotnet restore "PayrollEngine.WebApp.sln" --runtime linux-x64; \
    fi

# copy everything else
COPY . .
WORKDIR "/src/Server"

# Publish with architecture-specific runtime and restore included
RUN if [ "$TARGETARCH" = "arm64" ]; then \
      dotnet publish "PayrollEngine.WebApp.Server.csproj" -c Release -o /app/publish --runtime linux-arm64 --self-contained false; \
    else \
      dotnet publish "PayrollEngine.WebApp.Server.csproj" -c Release -o /app/publish --runtime linux-x64 --self-contained false; \
    fi

# final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PayrollEngine.WebApp.Server.dll"]