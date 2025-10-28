# MudGridify - Dynamic Filter Builder for Gridify

A powerful, type-safe Blazor component library that provides a dynamic filter builder UI for [Gridify](https://alirezanet.github.io/Gridify/), built with [MudBlazor](https://mudblazor.com/).

## Features

- **Dynamic Property Selection**: Configure filterable properties with display names and data types
- **Type-Safe Filtering**: Automatically renders appropriate input controls based on property types:
  - String: Text input with string operators (equals, contains, starts with, ends with, etc.)
  - Number: Numeric input with comparison operators (equals, greater than, less than, etc.)
  - Boolean: Dropdown with custom true/false labels
  - Date/DateTime: Date picker with comparison operators
- **All Gridify Operators**: Full support for Gridify filter operators:
  - `=` (equals), `!=` (not equals)
  - `>` (greater than), `<` (less than), `>=` (greater or equal), `<=` (less or equal)
  - `=*` (contains), `!*` (not contains)
  - `^` (starts with), `!^` (not starts with)
  - `$` (ends with), `!$` (not ends with)
- **Logical Operators**: Combine multiple conditions with AND (`,`) or OR (`|`)
- **Case-Insensitive Matching**: Optional case-insensitive string matching with `/i` suffix
- **Real-Time Query Generation**: Live preview of generated Gridify query string
- **Copy to Clipboard**: Quick copy functionality for generated queries
- **Programmatic Control**: Set conditions, add/remove filters, and get query strings programmatically

## Projects

- **Selmir.MudGridify**: The component library containing the filter builder
- **Selmir.MudGridify.Playground**: Blazor WASM demo application showcasing the component

## Quick Start

### 1. Add Package Reference

```xml
<ItemGroup>
  <PackageReference Include="MudBlazor" Version="8.13.0" />
  <PackageReference Include="Gridify" Version="2.17.0" />
  <ProjectReference Include="../Selmir.MudGridify/Selmir.MudGridify.csproj" />
</ItemGroup>
```

### 2. Configure Services

```csharp
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add MudBlazor services
builder.Services.AddMudServices();

await builder.Build().RunAsync();
```

### 3. Add Required Imports

```razor
@using MudBlazor
@using Selmir.MudGridify.Models
@using Selmir.MudGridify.Components
@using Gridify
```

### 4. Use the Component

```razor
@page "/filter-demo"
@using Selmir.MudGridify.Components
@using Selmir.MudGridify.Models

<GridifyFilterBuilder @ref="_filterBuilder"
                      FilterableProperties="@_filterableProperties"
                      ShowGeneratedQuery="true"
                      OnFilterChanged="@OnFilterChanged" />

<MudTable Items="@_filteredData" />

@code {
    private GridifyFilterBuilder? _filterBuilder;
    private List<FilterableProperty> _filterableProperties = new();
    private List<MyModel> _allData = new();
    private List<MyModel> _filteredData = new();

    protected override void OnInitialized()
    {
        // Define filterable properties
        _filterableProperties = new List<FilterableProperty>
        {
            new("FirstName", "First Name", FilterPropertyType.String),
            new("Age", "Age", FilterPropertyType.Number),
            new("BirthDate", "Birth Date", FilterPropertyType.Date),
            new("IsActive", "Active", FilterPropertyType.Boolean)
            {
                TrueLabel = "Yes",
                FalseLabel = "No"
            }
        };

        _allData = GetData();
        _filteredData = _allData;
    }

    private async Task OnFilterChanged(string filterQuery)
    {
        if (string.IsNullOrWhiteSpace(filterQuery))
        {
            _filteredData = new List<MyModel>(_allData);
        }
        else
        {
            // Apply Gridify filter
            var query = _allData.AsQueryable();
            _filteredData = query.ApplyFiltering(filterQuery).ToList();
        }
    }
}
```

## Component API

### GridifyFilterBuilder Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `FilterableProperties` | `List<FilterableProperty>` | Required | List of properties that can be filtered |
| `InitialConditions` | `List<FilterCondition>` | `[]` | Initial filter conditions to load |
| `ShowGeneratedQuery` | `bool` | `true` | Whether to show the generated Gridify query string |
| `OnFilterChanged` | `EventCallback<string>` | - | Event fired when the filter query changes |

### FilterableProperty Model

```csharp
public class FilterableProperty
{
    public string PropertyName { get; set; }    // Property name in your model
    public string DisplayName { get; set; }     // Display name shown to users
    public FilterPropertyType PropertyType { get; set; }  // Data type
    public string? TrueLabel { get; set; }      // Custom label for boolean true
    public string? FalseLabel { get; set; }     // Custom label for boolean false
}
```

### FilterPropertyType Enum

```csharp
public enum FilterPropertyType
{
    String,
    Number,
    Boolean,
    Date,
    DateTime
}
```

### Programmatic Control

```csharp
// Get the current filter query
string query = _filterBuilder.GetFilterQuery();

// Set conditions programmatically
_filterBuilder.SetConditions(new List<FilterCondition>
{
    new FilterCondition
    {
        Property = myProperty,
        Operator = FilterOperator.GreaterThan,
        Value = "100"
    }
});

// Add a single condition
_filterBuilder.AddCondition(new FilterCondition { ... });
```

## Running the Playground

```bash
cd src/Selmir.MudGridify.Playground
dotnet run
```

Navigate to `http://localhost:5259` to see the component in action.

### Playground Examples

1. **Home**: Overview and feature list
2. **Employee Filter**: Filter employees by name, department, salary, age, hire date, and status
3. **Product Filter**: Filter products by name, category, price, stock, and availability
4. **Advanced Demo**: Quick filter presets, statistics, and programmatic control examples

## Gridify Query Examples

The component generates valid Gridify query strings:

```
# Single condition
FirstName=John

# Multiple conditions with AND
FirstName=John,Age>30

# Multiple conditions with OR
Department=Sales|Department=Marketing

# String operators
FirstName^J                    # Starts with J
Email$@company.com            # Ends with @company.com
Department=*ing               # Contains "ing"

# Case-insensitive
FirstName=john/i              # Matches John, JOHN, john, etc.

# Comparison operators
Age>25                        # Greater than 25
Salary>=100000               # Greater or equal to 100000
HireDate<2023-01-01          # Before Jan 1, 2023

# Complex queries
(FirstName=*Jo,Age<30)|(FirstName!=Hn,Age>30)
```

## Technology Stack

- **.NET 10**: Latest .NET framework
- **Blazor WebAssembly**: Client-side Blazor
- **MudBlazor 8.13.0**: Material Design component library
- **Gridify 2.17.0**: Dynamic LINQ library for filtering, sorting, and paging

## License

This project is provided as-is for demonstration purposes.

## Credits

- [Gridify](https://github.com/alirezanet/Gridify) by Alireza Sabouri
- [MudBlazor](https://github.com/MudBlazor/MudBlazor) by MudBlazor Team

## Contributing

Contributions are welcome! Feel free to submit issues and pull requests.
