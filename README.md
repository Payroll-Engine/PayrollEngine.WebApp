# PayrollEngine.WebApp
[Payroll Engine](https://github.com/Payroll-Engine) web application (Blazor):

- View model
- Component with adaptor
- Browser client application

## Input Attributes
| Name                       | Description                             | Type          | Default  | Supported by |
|--|--|--|--|--|
*input.startLabel*           | start date input label                  | string        | Start    | all |
*input.startHelp*            | start date help text                    | string        | -        | all |
*input.startRequired*        | start date required error text          | string        | Start    | all |
*input.startReadOnly*        | start date is read only                 | bool          | false    | all (start date) |
*input.startFormat*          | start date input format <sup>4)</sup>   | string        | system   | date, date-time |
*input.startPickerOpen*      | start picker open mode <sup>2)</sup>    | string        | day      | date |
*input.startPickerType*      | start date datetime type <sup>6)</sup>  | string        | date     | all |
| | | | | |
*input.endLabel*             | end date input label                    | string        | End      | all |
*input.endHelp*              | end date help text                      | string        | -        | all |
*input.endRequired*          | end date required error text            | string        | Start    | all |
*input.endReadOnly*          | end date is read only                   | bool          | false    | all (end date) |
*input.endFormat*            | end date input format <sup>4)</sup>     | string        | system   | date, date-time |
*input.endPickerOpen*        | end picker date type <sup>2)</sup>      | string        | day      | date |
| | | | | |
*input.valueLabel*           | input value placeholder text            | string        | system   | all |
*input.valueAdornment*       | value adornment text                    | string        | -        | text, numeric |
*input.valueHelp*            | value help text                         | string        | -        | all |
*input.valueRequired*        | input value required error text         | string        | system   | all |
*input.valuePickerOpen*      | date picker date type <sup>2)</sup>     | string        | day      | date |
| | | | | |
*input.readOnly*             | input is read only                      | bool          | false    | all |
*input.hidden*               | input is hidden                         | bool          | false    | all |
*input.showDescription*      | input description is visible            | bool          | false    | all |
*input.culture*              | the display culture/currency <sup>1)</sup> | string     | system   | money |
*input.minValue*             | minimum input value                     | DateTime/num  | -        | numeric, date, date-time |
*input.maxValue*             | maximum input value                     | DateTime/num  | -        | numeric, date, date-time |
*input.stepSize*             | step size on spin buttons               | num           | 1        | numeric |
*input.format*               | input format <sup>4)</sup>              | string        | system   | date, date-time |
*input.mask*                 | input mask <sup>3)</sup>                | string        | -        | text |
*input.sortOrder*            | sorting order <sup>5)</sup>             | int           | system   | all |
*input.lineCount*            | show multiple text lines                | int           | 1        | text |
*input.maxLength*            | maximum text length                     | int           | -        | text |
*input.check*                | input checkbox instead of switch        | bool          | false    | boolean |
*input.customValue*          | able to enter custom lookup value       | bool          | false    | lookups |
| | | | | |
*input.attachment*           | enable document upload <sup>7)</sup>   | string        | none     | all |
*input.attachmentExtensions* | allowed files for upload <sup>8)</sup> | string        | -        | all |
| | | | | |
*input.list*                 | provide list of possible inputs         | object[] <sup>9)</sup> | -  | all |
*input.listValues*           | provide values for a list               | object[] <sup>10)</sup> | key | all |
*input.listSelection*        | preselected list value                  | string <sup>11)</sup>       | -        | all |

<sup>1\)</sup> culture names https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-lcid/a9eac961-e77d-41a6-90a5-ce1a8b0cdb9c

<sup>2\)</sup> date picker open mode: day, month, year

<sup>3\)</sup> text box input mask *

<sup>4\)</sup> Date and time format
   
    - Standard format strings: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings
    - Custom format strings: https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings

<sup>5\)</sup> Related cases sorting order using types "Ordered" (sorted by input.sortOrder attribute), "Ascending" or "Descending"

<sup>6\)</sup> Date picker type: "DatePicker" (date only), "DateTimePicker" (date and time)

<sup>7\)</sup> Document attachment mode: "None", "Optional", "Mandatory"

<sup>8\)</sup> Comma separated string,  example: ".jpg,.png"

<sup>9\)</sup> Json array with field value type

<sup>10\)</sup> Json array with the same count of list values

<sup>11\)</sup> Sleetced list value when available otherwise the selected list item (field value type)

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

MaskedTextBox documentation: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.maskedtextbox.mask

## Date expressions
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

## Data Grids
### Grid Identifiers
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

Attribute syntax:
**grid.<GridId>=<GridSettings>**

GridSettings: JSON dictionary with grid settings, see chapter 'Grid settings'

### Grid settings
Grid settings are stored as tenant attribute, using the grid identifier as attribute key.
The attribute value contains a json array with custom column configuration:

| Property | Description | Type | Optional |
|--|--|--|--|
Attribute | The attribute name | string | 
Header    | The column header  | string | x (default: attribute name) |
ValueType | Payroll value type | string | ?

**Configuration Example**
"grid.TenantsGrid": "[{\"Attribute\":\"ErpId\",\"Header\":\"Erp Id\",\"ValueType\":\"String\"}]"

### Grid filtering
- String
    - foo: contains foo (case sensitive, contains is the default filter mode)
    - *foo: starts with foo
    - %foo: ends with foo
- Number (default is equal)
    - =22: equal 22
    - !=22: not equal 22
    - \>22: greater than 22
    - \>=22: greater than or equal 22
    - <22: less than 22
    - <=22: less than or equal 22
- Date
    - year filter:
        - 19
        - 2019
    - month filter:
        - 1.19
        - 1.2019
        - Jan 19 (month name from calendar)
        - Jan 2019
    - date period filter:
        - 1.1.19 - 1.10.19
        - 1.1.2019 - 1.10.2019
    - date filter:
        - 1.1.19
        - 1.1.2019
        - operators (default is equal)
            - =1.1.19: equal 1.1.2019
            - !=1.1.19: not equal 1.1.2019
            - \>1.1.19: greater than 1.1.2019
            - \>=1.1.19: greater than or equal 1.1.2019
            - <1.1.19: less than 1.1.2019
            - <=1.1.19: less than or equal 1.1.2019
- Boolean
    - compare true/false

## User Login
User login supports url parameters to pre-select inputs and redirect user to specific page.

Supported arguments are:
- user: user identifier (e.g. peter.schmid@foo.com)
- tenant: tenant identifier (e.g. SimplePayroll) Note: requires user to be set
- redirectTo: page name (i.e. employeecases, report, payroll, ...)

## Logging
- Configuration: Client\appsettings.json
- Log-File:      Client\Logs
