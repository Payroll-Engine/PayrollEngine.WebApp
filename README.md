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
<br/>

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

## Application Configuration
The server configuration file `Server\appsetings.json` contains the following settings:

**Startup Configuration**
| Setting      | Description            | Default |
|:--|:--|:--|
| `StartupCulture` | The web application process culture (string) | System culture |
| `AutoLogin` | Automatic logon using the `StartupTenant` and `StartupUser` (bool) <sup>1)</sup> | `false` |
| `StartupTenant` | The startup tenant using th auto login option (string) <sup>1)</sup> | - |
| `StartupUser` | The startup user using the auto login option (string) <sup>1)</sup> | - |
| `ClearStorage` | Clear the user storage (bool) | - |

<sup>1)</sup> Only in debug mode<br />

**App Configuration**
| Setting      | Description            | Default |
|:--|:--|:--|
| `AppTitle` | The application title (string) | Payroll Engine |
| `AppImage` | The application image (string) | Payroll Engine image |
| `AppImageDarkMode` | The application dark mode image (string) | Payroll Engine image |
| `AdminEmail` | The administration email for error pages (string) | - |
| `DefaultFeatures` | The default features for new users (string[]) | - |
| `LogCaseChanges` | Add case changes to the tenant log (bool) | `false` |
| `SessionTimeout` | Web application user session timeout (timespan) | 10 minutes |

**Payroll Http Configuration**
| Setting      | Description            | Default |
|:--|:--|:--|
| `BaseUrl` | The backend base url (string) | |
| `Port` | The backend url port (string) | |
| `Timeout` | The backend request timeout (TimeSpan) | 100 seconds |

**Serilog**<br />
File and console logging with [Serilog](https://serilog.net/).

> It is recommended that you save the application settings within your local [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets).

## Application Logs
Under Windows, the web application server stores its logs in the system folder `%ProgramData%\WebApp\logs`.

## Input Attributes
The case input attributes can be used to control the behaviour of user input.

| Name                       | Description                             | Type          | Default  | Supported by |
|--|--|--|--|--|
| <b>General</b> |
`input.hidden`               | input is hidden                         | bool          | false    | all |
`input.showDescription`      | input description is visible            | bool          | false    | all |
`input.sortOrder`            | sort order <sup>5)</sup>                | int           | system   | all |
| <b>Start</b> |
`input.startLabel`           | start date input label                  | string        | Start    | all |
`input.startHelp`            | start date help text                    | string        | -        | all |
`input.startRequired`        | start date required error text          | string        | Start    | all |
`input.startReadOnly`        | start date is read only                 | bool          | false    | all (start date) |
`input.startFormat`          | start date input format <sup>4)</sup>   | string        | system   | date, date-time |
`input.startPickerOpen`      | start picker open mode <sup>2)</sup>    | string        | day      | date |
`input.startPickerType`      | start date datetime type <sup>6)</sup>  | string        | date     | all |
| <b>End</b> |
`input.endLabel`             | end date input label                    | string        | End      | all |
`input.endHelp`              | end date help text                      | string        | -        | all |
`input.endRequired`          | end date required error text            | string        | Start    | all |
`input.endReadOnly`          | end date is read only                   | bool          | false    | all (end date) |
`input.endFormat`            | end date input format <sup>4)</sup>     | string        | system   | date, date-time |
`input.endPickerOpen`        | end picker date type <sup>2)</sup>      | string        | day      | date |
| <b>Value</b> |
`input.valueLabel`           | input value label                       | string        | system   | all |
`input.valueAdornment`       | input value adornment text              | string        | -        | text, numeric |
`input.valueHelp`            | input value help text                   | string        | -        | all |
`input.valueMask`            | input mask <sup>3)</sup>                | string        | -        | text |
`input.valueRequired`        | input value required error text         | string        | system   | all |
`input.valueReadOnly`        | input is read only                      | bool          | false    | all |
`input.valuePickerOpen`      | date picker date type <sup>2)</sup>     | string        | day      | date |
`input.culture`              | the display culture <sup>1)</sup>       | string        | system   | money |
`input.minValue`             | minimum input value                     | DateTime/num  | -        | numeric, date, date-time |
`input.maxValue`             | maximum input value                     | DateTime/num  | -        | numeric, date, date-time |
`input.stepSize`             | step size on spin buttons               | num           | 1        | numeric |
`input.format`               | input format <sup>4)</sup>              | string        | system   | date, date-time |
`input.lineCount`            | show multiple lines of text             | int           | 1        | text |
`input.maxLength`            | maximum text length                     | int           | -        | text |
`input.check`                | input checkbox instead of switch        | bool          | false    | boolean |
| <b>Attachment</b> |
`input.attachment`           | enable document upload <sup>7)</sup>    | string        | none     | all |
`input.attachmentExtensions` | allowed files for upload <sup>8)</sup>  | string        | -        | all |
| <b>List</b> |
`input.list`                 | provide list of possible inputs         | object[] <sup>9)</sup>  | -  | all |
`input.listValues`           | provide values for a list               | object[] <sup>10)</sup> | key | all |
`input.listSelection`        | preselected list value                  | string <sup>11)</sup>   | -        | all |
<br/>

<sup>1)</sup> Culture names https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-lcid/a9eac961-e77d-41a6-90a5-ce1a8b0cdb9c<br/>
<sup>2)</sup> Date picker open mode: `day`, `month` or `year`<br/>
<sup>3)</sup> Text box input mask `*`<br/>
<sup>4)</sup> Date and time format<br/>
    - Standard format strings: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings<br/>
    - Custom format strings: https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings<br/>
<sup>5)</sup> Related cases sort order using the types `Ordered` (sorted by input.sortOrder attribute), `Ascending` or `Descending`<br/>
<sup>6)</sup> Date picker type: `DatePicker` (date only), `DateTimePicker` (date and time)<br/>
<sup>7)</sup> Document attachment mode: `None`, `Optional`, `Mandatory`<br/>
<sup>8)</sup> Comma separated string, example: `.jpg,.png`<br/>
<sup>9)</sup> JSON array with field value type<br/>
<sup>10)</sup> JSON array with the same count of list values<br/>
<sup>11)</sup> Selected list value when available otherwise the selected list item (field value type)

### Text input mask
| Mask  | Description |
|--|--|
|`0`   | Digit required. This element accepts any single digit from `0` to `9` |
|`9`   | Digit or space, optional |
|`\#`   | Digit or space, optional, plus (+) and minus (-) signs are allowed |
|`L`   | Letter required. The letters `a`-`z` and `A`-`Z` are accepted |
|`?`   | Letter or space, optional |
|`&`   | Character required  |
|`C`   | Character or space, optional |
|`A`   | Alphanumeric (`A`-`Z`, `a`-`z`, `0`-`9`) required |
|`a`   | Alphanumeric (`A`-`Z`, `a`-`z`, `0`-`9`) or space, optional |
|`<`   | Shift down. Converts all characters to lower case |
|`\>`   | Shift up. Converts all characters to upper case |
<br/>

Escapes a mask character, turning it into a literal.
All other characters: Literals. All non-masked elements (literals) will appear as themselves within the masked text box.

**Input Mask Examples**
|`Mask  | Input |
|--|--|
| `\#####`    | 012+- |
| `LLLLLL`    | Sample |
| `&&&&&`     | A12# |
| `\>LLL<LLL` | SAMple |
| `\\\A999`   | A321 |
<br/>

> MaskedTextBox documentation: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.maskedtextbox.mask

 ### Date expressions
| Expression  | Description |
|--|--|
`yesterday`     | yesterday |
`today`         | today |
`tomorrow`      | tomorrow |
`previousmonth` | first day of previous month |
`month`         | first day of current month |
`nextmonth`     | first day of next month |
`previousyear`  | first day of previous calendar year |
`year`          | first day of current calendar year |
`nextyear`      | first day of next calendar year |
`offset:<count><type>` | offset from today (see *offset type*)|
`DateTime`      | the date/time string representation, supported types: JSON and .net |
<br/>

**Offset type**
- `d` - day
- `w` - week
- `m` - month
- `y` - year

**Offset examples**
- 10 days: `offset:10d`
- 2 weeks: `offset:2w`
- 9 months: `offset:9m`
- 1 year: `offset:1y`
- minus 5 day: `offset:-5d`
- minus 6 weeks: `offset:-6w`
- minus 3 months: `offset:-3m`
- minus 4 years: `offset:-4y`

> `TimeSpan` documentation: https://docs.microsoft.com/en-us/dotnet/api/system.timespan

## Third party components
- UI with [MudBlazor](https://github.com/MudBlazor/MudBlazor/) - licence `MIT`
- Storage with [LocalStorage](https://github.com/Blazored/LocalStorage/) - licence `MIT`
- Logging with [Serilog](https://github.com/serilog/serilog/) - licence `Apache 2.0`
