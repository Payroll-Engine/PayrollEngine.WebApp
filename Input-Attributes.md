# Input Attributes
The case input attributes can be used to control the behavior of user input.

| Name                       | Description                               | Type          | Default  | Supported by             |
|--|--|--|--|--|
| <b>General</b> |
`input.icon`                 | Custom item icon <sup>11)</sup>           | string        | false    | Case                     |
`input.priority`             | Item priority <sup>12)</sup>              | string        | normal   | Case                     |
`input.hidden`               | Input is hidden                           | bool          | false    | all                      |
`input.hiddenDates`          | Hidden start and end dates                | bool          | false    | Case field               |
`input.showDescription`      | Input description is visible              | bool          | false    | all                      |
| <b>Start</b> |
`input.startLabel`           | Start date input label                    | string        | Start    | all                      |
`input.startHelp`            | Start date help text                      | string        | -        | all                      |
`input.startRequired`        | Start date required error text            | string        | Start    | all                      |
`input.startReadOnly`        | Start date input to read only             | bool          | false    | all (start date)         |
`input.startHidden`          | Hide start date                           | bool          | false    | all (start date)         |
`input.startFormat`          | Start date input format <sup>4)</sup>     | string        | system   | date, date-time          |
`input.startPickerOpen`      | Start date picker open mode <sup>2)</sup> | string        | day      | date                     |
`input.startPickerType`      | Start date datetime type <sup>5)</sup>    | string        | date     | all                      |
| <b>End</b> |
`input.endLabel`             | End date input label                      | string        | End      | all                      |
`input.endHelp`              | End date help text                        | string        | -        | all                      |
`input.endRequired`          | End date required error text              | string        | Start    | all                      |
`input.endReadOnly`          | End date input to read only               | bool          | false    | all (end date)           |
`input.endHidden`            | Hide end date                             | bool          | false    | all (start date)         |
`input.endFormat`            | End date input format <sup>4)</sup>       | string        | system   | date, date-time          |
`input.endPickerOpen`        | End date picker date type <sup>2)</sup>   | string        | day      | date                     |
`input.endPickerType`        | End  date datetime type <sup>5)</sup>     | string        | date     | all                      |
| <b>Value</b> |
`input.valueLabel`           | Input value label                         | string        | system   | all                      |
`input.valueAdornment`       | Input value adornment text                | string        | -        | text, numeric            |
`input.valueHelp`            | Input value help text                     | string        | -        | all                      |
`input.valueMask`            | Input mask <sup>3)</sup>                  | string        | -        | text                     |
`input.valueRequired`        | Input value required error text           | string        | system   | all                      |
`input.valueReadOnly`        | Input is read only                        | bool          | false    | all                      |
`input.valuePickerOpen`      | Date picker date type <sup>2)</sup>       | string        | day      | date                     |
`input.culture`              | Display culture <sup>1)</sup>             | string        | system   | money                    |
`input.minValue`             | Minimum input value                       | datetime/num  | -        | numeric, date, date-time |
`input.maxValue`             | Maximum input value                       | datetime/num  | -        | numeric, date, date-time |
`input.stepSize`             | Step size on spin buttons                 | num           | 1        | numeric                  |
`input.format`               | Input format <sup>4)</sup>                | string        | system   | date, date-time          |
`input.lineCount`            | Show multiple lines of text               | int           | 1        | text                     |
`input.maxLength`            | Maximum text length                       | int           | -        | text                     |
`input.check`                | Input checkbox instead of switch          | bool          | false    | bool toggles             |
`input.valueHistory`         | Enable value history                      | bool          | false    | all                      |
| <b>Attachment</b> |
`input.attachment`           | Enable document upload <sup>6)</sup>      | string        | none     | all                      |
`input.attachmentExtensions` | Allowed files for upload <sup>7)</sup>    | string        | -        | all                      |
| <b>List</b> |
`input.list`                 | Provide list of possible inputs           | object[] <sup>8)</sup>  | -        | all            |
`input.listValues`           | Provide values for a list                 | object[] <sup>9)</sup>  | key      | all            |
`input.listSelection`        | Preselected list value                    | string <sup>10)</sup>   | -        | all            |
<br/>

<sup>1)</sup> Culture names https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-lcid/a9eac961-e77d-41a6-90a5-ce1a8b0cdb9c<br/>
<sup>2)</sup> Date picker open mode: `day`, `month` or `year`<br/>
<sup>3)</sup> Text box input mask `*`<br/>
<sup>4)</sup> Date and time format<br/>
    - Standard format strings: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings<br/>
    - Custom format strings: https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings<br/>
<sup>5)</sup> Date picker type: `DatePicker` (date only), `DateTimePicker` (date and time)<br/>
<sup>6)</sup> Document attachment mode: `None`, `Optional`, `Mandatory`<br/>
<sup>7)</sup> Comma separated string, example: `.jpg,.png`<br/>
<sup>8)</sup> JSON array with field value type<br/>
<sup>9)</sup> JSON array with the same count of list values<br/>
<sup>10)</sup> Selected list value when available otherwise the selected list item (field value type)<br/>
<sup>11)</sup> Icon name https://mudblazor.com/features/icons<br/>
<sup>12)</sup> Item priority: `Low`, `Normal`, `High`, `Critical`<br/>


## Text input mask
| Mask | Description                                                           |
|--|--|
|`0`   | Digit required. This element accepts any single digit from `0` to `9` |
|`9`   | Digit or space, optional                                              |
|`\#`  | Digit or space, optional, plus (+) and minus (-) signs are allowed    |
|`L`   | Letter required. The letters `a`-`z` and `A`-`Z` are accepted         |
|`?`   | Letter or space, optional                                             |
|`&`   | Character required                                                    |
|`C`   | Character or space, optional                                          |
|`A`   | Alphanumeric (`A`-`Z`, `a`-`z`, `0`-`9`) required                     |
|`a`   | Alphanumeric (`A`-`Z`, `a`-`z`, `0`-`9`) or space, optional           |
|`<`   | Shift down. Converts all characters to lower case                     |
|`\>`  | Shift up. Converts all characters to upper case                       |
<br/>

Escapes a mask character, turning it into a literal.
All other characters: Literals. All non-masked elements (literals) will appear as themselves within the masked text box.

### Input Mask Examples
|`Mask        | Input  |
|--|--|
| `\#####`    | 012+-  |
| `LLLLLL`    | Sample |
| `&&&&&`     | A12#   |
| `\>LLL<LLL` | SAMple |
| `\\\A999`   | A321   |
<br/>

> MaskedTextBox documentation: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.maskedtextbox.mask

 ### Date expressions
| Expression              | Description                                                           |
|--|--|
`yesterday`               | Yesterday                                                             |
`today`                   | Today                                                                 |
`tomorrow`                | Tomorrow                                                              |
`previousmonth`           | First day of previous month                                           |
`month`                   | First day of current month                                            |
`nextmonth`               | First day of next month                                               |
`previousyear`            | First day of previous calendar year                                   |
`year`                    | First day of current calendar year                                    |
`nextyear`                | First day of next calendar year                                       |
`offset:<count><type>`    | Offset from today (see *offset type*)                                 |
`DateTime`                | The date/time string representation, supported types: JSON and .net   |
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

## Input Attributes Examples
In the following example, the case has an additional icon that allows you to enter 1 to 3 children:

```json
"cases": [
  {
    "name": "ChildCount",
    "caseType": "Employee",
    "attributes": {
      "input.icon": "Filled.Article"
    },
    "fields": [
      {
        "name": "ChildCount",
        "description": "The children count (max 3)",
        "valueType": "Integer",
        "timeType": "Period",
        "attributes": {
          "input.minValue": 0,
          "input.maxValue": 3
        }
      }
    ]
  }
]
```

In this example, the value can be selected from a drop-down list:

```json
"cases": [
  {
    "name": "EmployeeState",
    "caseType": "Employee",
    "fields": [
      {
        "name": "StringList",
        "valueType": "String",
        "timeType": "Period",
        "defaultStart": "today",
        "attributes": {
          "input.list": "[\"State 1\", \"State 2\", \"State 3\"]",
          "input.listSelection": "State 2"
        }
      },
    ],
  }
]
```

The following example shows how documents can be associated with a case field value:

```json
"cases": [
  {
    "name": "EmployeeProfile",
    "caseType": "Employee",
    "fields": [
      {
        "name": "ProfilePicture",
        "valueType": "None",
        "timeType": "Period",
        "defaultStart": "today",
        "attributes": {
          "input.attachment": "Optional",
          "input.attachmentExtensions": ".jpg,.gif,.png"
        }
      },
    ],
  }
]
```
