# Payroll Engine Web Application
👉 This application is part of the [Payroll Engine](https://github.com/Payroll-Engine/PayrollEngine/wiki).

The Web Application provides full access to the Payroll Engine. For a better understanding of the working concepts, it is recommended to read the [Payroll Engine Whitepaper](https://github.com/Payroll-Engine/PayrollEngine/blob/main/Documents/PayrolEnginelWhitepaper.pdf).

## Features
The functions of the web app are divided into features:

| Feature             | Group          | Description                             |
|--|--|--|
| Tasks               | General        | Manage the user tasks                   |
| Employee Cases      | General        | Add new employee case                   |
| Company Cases       | General        | Add new company case                    |
| National Cases      | General        | Add new national case                   |
| Global Cases        | General        | Add new global case                     |
| Reports             | General        | Build reports <sup>1)</sup>             |
| Payrun Results      | Payrun         | View and Export payrun results          |
| Payrun Jobs         | Payrun         | Start and manage payrun jobs            |
| Payruns             | Payrun         | Add or change a payrun                  |
| Payrolls            | Payroll        | Add or change payroll and clusters      |
| Payroll Layers      | Payroll        | Add or change payroll layer             |
| Regulations         | Payroll        | Add or remove regulation                |
| Regulation          | Payroll        | Derived regulation edit                 |
| Shared Regulations  | Administration | Manage shared regulations               |
| Tenants             | Administration | Add or change a tenant                  |
| Users               | Administration | Add or change an users<sup>2) 3)</sup>  |
| Calendars           | Administration | Add or change a payroll calendar        |
| Divisions           | Administration | Add or change a division                |
| Employees           | Administration | Add or change an employee               |
| Webhooks            | Administration | Add or change webhooks and messages     |
| Logs                | Administration | View the tenant logs <sup>4)</sup>      |
| User Storage        | System         | Manage the local user storage           |

<sup>1)</sup> Based on [FastReports](https://github.com/FastReports).<br/>
<sup>2)</sup> The available functions can be assigned to the user.<br/>
<sup>3)</sup> The "Administrator" user type can manage all functions.<br/>
<sup>4)</sup> Tenant logs are generated by the regulations and should not be confused with the application log.<br/>

## Web Application Server
To run the web application server, the web host must support the execution of .NET Core applications. For local development, [IIS Express](https://learn.microsoft.com/en-us/iis/extensions/introduction-to-iis-express/iis-express-overview) serves as the host in two execution variants:
- [CLI dotnet command](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet) using the command within the binary folder:
```
start "" dotnet PayrollEngine.WebApp.Server.dll --urls=https://localhost:7179/
```

- Visual Studio Solution `PayrollEngine.WebApp.sln` using the debugger.

## User Login
When logging in for the first time, the user must choose a password that complies with the following rules:
- at least 8 characters
- 1 numeric character
- 1 lower case character
- 1 upper case character
- 1 special character

### Developer login
In debug mode the web application can be started with an automatic login. The desired context is defined with the program settings `AutoLogin`, `StartupTenant` and `StartupUser`. In this mode it is possible to switch between the tenants.

## Application Configuration
The server configuration file `appsetings.json` contains the following settings:

### Startup Configuration

| Setting          | Description                                                                | Type     | Default        |
|:--|:--|:--|:--|
| `StartupCulture` | The web application process culture                                        | string   | System culture |
| `StartupTenant`  | The startup tenant using th auto login option <sup>1)</sup>                | string   | -              |
| `StartupUser`    | The startup user using the auto login option  <sup>1)</sup>                | string   | -              |
| `AutoLogin`      | Automatic logon using the `StartupTenant` and `StartupUser` <sup>1)</sup>  | bool     | false          |
| `ClearStorage`   | Clear the user storage                                                     | bool     | false          |

<sup>1)</sup> Only in debug mode<br />

### App Configuration

| Setting                | Description                                 | Type     | Default              |
|:--|:--|:--|:--|
| `AppTitle`             | The application title                       | string   | Payroll Engine       |
| `AppImage`             | The application image                       | string   | Payroll Engine image |
| `AppImageDarkMode`     | The application dark mode image             | string   | Payroll Engine image |
| `DarkMode`             | Default dark mode at startup                | bool?    | User system setting  |
| `AdminEmail`           | The administration email for error pages    | string   | -                    |
| `ProductUrl`           | The product url                             | string   | -                    |
| `PreferredCultures`    | Preferred cultures in dropdown lists <sup>1)</sup> | string[] | -             |
| `DefaultFeatures`      | The default features for new users          | string[] | -                    |
| `AllowTenantSwitch`    | Allow to switch between tenants             | bool     | false                |
| `LogHttpRequests`      | Log Http request to file                    | bool     | false                |
| `LogCaseChanges`       | Add case changes to the tenant log          | bool     | false                |
| `SessionTimeout`       | Web application user session timeout        | timespan | 10 minutes           |
| `ExcelExportMaxRecords`| Maximum count of excel export rows          | int      | 10'000               |
| `MaxDownloadSize`      | Maximum download size                       | long     | 512'000              |

<sup>1)</sup> Preferred cultures examples: [ "en-US", "en-AU", "de"]<br />

### Payroll Http Configuration

| Setting   | Description                     | Type       | Default     |
|:--|:--|:--|:--|
| `BaseUrl` | The backend base url            | string     |             |
| `Port`    | The backend url port            | string     |             |
| `Timeout` | The backend request timeout     | TimeSpan   | 100 seconds |
| `ApiKey`  | The backend API key             | string     |             |

The Payroll HTTP client configuration can be declared in the following locations.

| Priority | Source                                                      | Description                                                        |
|--|--|--|
| 1.       | Environment variable `PayrollApiConnection`                 | Connection string with the HTTP client configuration               |
| 2.       | Environment variable `PayrollApiConfiguration`              | HTTP client configuration JSON file name                           |
| 3.       | File `apisettings.json`                                     | HTTP client configuration JSON file located in the program folder  |
| 4.       | File `appsettings.json`                                     | HTTP client configuration from the program configuration JSON file |

### Serilog
File and console logging with [Serilog](https://serilog.net/).

> It is recommended that you save the application settings within your local [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets).

## Application Logs
The web application server stores its logs in the application folder `logs`.

## Api Key
If a key is required to access the backend API, it must be obtained from one of the following sources (in order of priority):

1. Environment variable `PayrollApiKey`.
2. From the [Payroll HTTP configuration](#payroll-http-configuration).

## Input Attributes
The case input attributes can be used to control the behavior of user input.

👉 Input Attributes [Reference](Input-Attributes.md).

## Solution projects
The.NET Core application consists of the following projects:

| Name                                 | Type             | Description                                       |
|:--|:--|:--|
| `PayrollEngine.WebApp.Shared`        | Library          | Shared resources                                  |
| `PayrollEngine.WebApp.Core`          | Library          | Core types and services                           |
| `PayrollEngine.WebApp.ViewModel`     | Razor Library    | View model objects                                |
| `PayrollEngine.WebApp.Presentation`  | Razor Library    | Presentation components                           |
| `PayrollEngine.WebApp.Server`        | Exe              | Web application server with pages and dialogs     |

## Third party components
- UI with [MudBlazor](https://github.com/MudBlazor/MudBlazor/) - license `MIT`
- Storage with [LocalStorage](https://github.com/Blazored/LocalStorage/) - license `MIT`
- Logging with [Serilog](https://github.com/serilog/serilog/) - license `Apache 2.0`
