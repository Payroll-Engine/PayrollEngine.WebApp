<h1>Payroll Engine Web Application</h1>

The Web Application provides full access to the Payroll Engine.
Zum Verständnis der Arbeitskonzepte empfieht sich das **[Payroll Engine White Paper](https://github.com/Payroll-Engine/PayrollEngine/blob/main/Documents/PayrolEnginelWhitePaper.pdf)** zu lesen.

<br />

# Features

Die Funktionen der Web App sind in Features unterteilt:
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
| Divisions           | Administration | Add or change a division                |
| Employees           | Administration | Add or change an employee               |
| Logs                | Administration | View the tenant logs <sup>4)</sup>      |
| User Storage        | System         | Manage the local user storage           |
<br/>

<sup>1)</sup> Based on [FastReports](https://github.com/FastReports).<br/>
<sup>2)</sup> Dem *User* können die verfügbaren Feature zugeordnet werden.<br/>
<sup>3)</sup> User mit der Option *Supervisor* verwalten die Features.<br/>
<sup>4)</sup> Tenant Logs werden von den Regulierungen generiert und sind nicht mit dem Applikations-Log zu verwechseln.<br/>

<br />

# Web Applicaion Server Hosting
Für den Betrieb des Web Applikations Servers muss der Webhoster die Ausführung von .NET Core Applikationen unterstützen. Für die lokale Entwicklung dient [IIS Express](https://learn.microsoft.com/en-us/iis/extensions/introduction-to-iis-express/iis-express-overview) als Host in zwei Ausführungsvarianten:
- [CLI dotnet command](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet) using the batch:
```
PayrollEngine\Batches\WebApp.Server.Start.bat
```
- Visual Studio Solution ***PayrollEngine\PayrollEngine.Backend\PayrollEngine.Backend.sln*** using the debugger

# User Login
Beim erstmaligen anmelden muss der User ein Passwort welches folgende Regeln einhält, bestimmen:
- minimum 8 characters
- 1 digit character
- 1 lowercase character
- 1 uppercase character
- 1 special character

> For local development you can use the auto-login feature (*appsettings.json*) with various startup options.

<br/>

# Configuration
Die Applikations-Konfiguration *Server\appsetings.json* beinhaltet folgende Einstellungen:
- Culture
- Default features for new users
- Application title
- Case change log
- Session timeout
- Systemlog mit [Serilog](https://serilog.net/), andere Logging-Tools können integriert werden

> It is recommended to save the application settings within your local [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets).

<br/>

# Input Attributes
Mit den Case Input-Attributen kann das Verhalten der Benutzereingaben gesteuert werden.

| Name                       | Description                             | Type          | Default  | Supported by |
|--|--|--|--|--|
| <b>General</b> |
*input.hidden*               | input is hidden                         | bool          | false    | all |
*input.showDescription*      | input description is visible            | bool          | false    | all |
*input.sortOrder*            | sorting order <sup>5)</sup>             | int           | system   | all |
| <b>Start</b> |
*input.startLabel*           | start date input label                  | string        | Start    | all |
*input.startHelp*            | start date help text                    | string        | -        | all |
*input.startRequired*        | start date required error text          | string        | Start    | all |
*input.startReadOnly*        | start date is read only                 | bool          | false    | all (start date) |
*input.startFormat*          | start date input format <sup>4)</sup>   | string        | system   | date, date-time |
*input.startPickerOpen*      | start picker open mode <sup>2)</sup>    | string        | day      | date |
*input.startPickerType*      | start date datetime type <sup>6)</sup>  | string        | date     | all |
| <b>End</b> |
*input.endLabel*             | end date input label                    | string        | End      | all |
*input.endHelp*              | end date help text                      | string        | -        | all |
*input.endRequired*          | end date required error text            | string        | Start    | all |
*input.endReadOnly*          | end date is read only                   | bool          | false    | all (end date) |
*input.endFormat*            | end date input format <sup>4)</sup>     | string        | system   | date, date-time |
*input.endPickerOpen*        | end picker date type <sup>2)</sup>      | string        | day      | date |
| <b>Value</b> |
*input.valueLabel*           | input value label                       | string        | system   | all |
*input.valueAdornment*       | input value adornment text              | string        | -        | text, numeric |
*input.valueHelp*            | input value help text                   | string        | -        | all |
*input.valueMask*            | input mask <sup>3)</sup>                | string        | -        | text |
*input.valueRequired*        | input value required error text         | string        | system   | all |
*input.valueReadOnly*        | input is read only                      | bool          | false    | all |
*input.valuePickerOpen*      | date picker date type <sup>2)</sup>     | string        | day      | date |
*input.culture*              | the display culture <sup>1)</sup>       | string        | system   | money |
*input.minValue*             | minimum input value                     | DateTime/num  | -        | numeric, date, date-time |
*input.maxValue*             | maximum input value                     | DateTime/num  | -        | numeric, date, date-time |
*input.stepSize*             | step size on spin buttons               | num           | 1        | numeric |
*input.format*               | input format <sup>4)</sup>              | string        | system   | date, date-time |
*input.lineCount*            | show multiple text lines                | int           | 1        | text |
*input.maxLength*            | maximum text length                     | int           | -        | text |
*input.check*                | input checkbox instead of switch        | bool          | false    | boolean |
| <b>Attachment</b> |
*input.attachment*           | enable document upload <sup>7)</sup>    | string        | none     | all |
*input.attachmentExtensions* | allowed files for upload <sup>8)</sup>  | string        | -        | all |
| <b>List</b> |
*input.list*                 | provide list of possible inputs         | object[] <sup>9)</sup>  | -  | all |
*input.listValues*           | provide values for a list               | object[] <sup>10)</sup> | key | all |
*input.listSelection*        | preselected list value                  | string <sup>11)</sup>   | -        | all |
<br/>

<sup>1)</sup> culture names https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-lcid/a9eac961-e77d-41a6-90a5-ce1a8b0cdb9c<br/>
<sup>2)</sup> date picker open mode: day, month, year<br/>
<sup>3)</sup> text box input mask *<br/>
<sup>4)</sup> Date and time format<br/>
    - Standard format strings: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings<br/>
    - Custom format strings: https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings<br/>
<sup>5)</sup> Related cases sorting order using types "Ordered" (sorted by input.sortOrder attribute), "Ascending" or "Descending"<br/>
<sup>6)</sup> Date picker type: "DatePicker" (date only), "DateTimePicker" (date and time)<br/>
<sup>7)</sup> Document attachment mode: "None", "Optional", "Mandatory"<br/>
<sup>8)</sup> Comma separated string,  example: ".jpg,.png"<br/>
<sup>9)</sup> Json array with field value type<br/>
<sup>10)</sup> Json array with the same count of list values<br/>
<sup>11)</sup> Sleetced list value when available otherwise the selected list item (field value type)
<br />

## Text input mask
| Mask  | Description |
|--|--|
|0   | Digit required. This element will accept any single digit from 0 to 9 |
|9   | Digit or space, optional |
|\#   | Digit or space, optional, Plus(+) and minus(-) signs are allowed |
|L   | Letter required. It will accept letters a-z and A-Z |
|?   | Letter or space, optional |
|&   | Requires a character |
|C   | Character or space, optional |
|A   | Alphanumeric (A-Za-z0-9) required |
|a   | Alphanumeric (A-Za-z0-9) or space, optional |
|<   | Shift down. Converts all characters to lower case |
|\>   | Shift up. Converts all characters to upper case |
<br/>

Escapes a mask character, turning it into a literal.
All other characters: Literals. All non-mask elements (literals) will appear as themselves within MaskedTextBox.

**Input Mask Examples**
| Mask  | Input |
|--|--|
| \#####    | 012+- |
| LLLLLL    | Sample |
| &&&&&     | A12# |
| \>LLL<LLL | SAMple |
| \\\A999   | A321 |
<br/>

MaskedTextBox documentation: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.maskedtextbox.mask

<br />

**Date expressions**
| Expression  | Description |
|--|--|
yesterday     | yesterday |
today         | today |
tomorrow      | tomorrow |
previousmonth | first day of previous month |
month         | first day of current month |
nextmonth     | first day of next month |
previousyear  | first day of previous calendar year |
year          | first day of current calendar year |
nextyear      | first day of next calendar year |
offset:<c><t> | offset from today* |
DateTime      | the date/time string representation, supported types: JSON and .net |
<br/>

\*\<c> is the offset count
\<t> is the offset type
d = day | example 10 days: offset:10d
w = week | example 2 weeks: offset:2w
m = month | example 9 months: offset:9m
y = year | example 1 year: offset:1y

negative offset is also possible:
example minus 5 day: offset:-5d
example minus 6 weeks: offset:-6w
example minus 3 months: offset:-3m
example minus 4 years: offset:-4y

TimeSpan documentation: https://docs.microsoft.com/en-us/dotnet/api/system.timespan

# Data Grids
| Source  | Grid Id |
|--|--|
Tasks     | TasksGrid |
Payroll cases     | PayrollCasesGrid |
Payroll cases     | EmployeeCaseChangeValuesGrid |
Payroll cases     | CompanyCaseChangeValuesGrid |
Payroll cases     | NationalCaseChangeValuesGrid |
Payroll cases     | GlobalCaseChangeValuesGrid |
Reports     | ReportsGrid |
Payrun Results     | PayrunResultsGrid |
Payruns     | PayrunsGrid |
Payrun - Legal Jobs  | PayrunJobsLegalGrid |
Payrun - Forecast Jobs     | PayrunJobsForecastGrid |
Payrolls     | PayrollsGrid |
Payroll Layers     | PayrollLayersGrid |
Regulations     | RegulationsGrid |
Users     | UsersGrid |
Tenants     | TenantsGrid |
Divisions     | DivisionsGrid |
Employees     | EmployeesGrid |
Logs     | LogsGrid |
<br/>

Attribute syntax:<br />
```
**grid.<GridId>=<GridSettings>**
```
<br />

## Data Grid settings

Data Grid settings are stored as tenant attribute, using the grid identifier as attribute key.
The attribute value contains a json array with custom column configuration:

| Property | Description | Type | Optional |
|--|--|--|--|
Attribute | The attribute name | string | 
Header    | The column header  | string | x (default: attribute name) |
ValueType | Payroll value type | string | ?
<br/>

*Configuration example:*<br />
` 
"grid.TenantsGrid": "[{\"Attribute\":\"ErpId\",\"Header\":\"Erp Id\",\"ValueType\":\"String\"}]"
`
