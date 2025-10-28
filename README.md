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
- **URL Persistence**: Built-in parser utility for shareable, bookmarkable filter states via URL parameters
- **Fully Responsive**: Optimized layouts for mobile, tablet, and desktop screens
  - Adaptive grid layouts with proper breakpoints (xs, sm, md, lg)
  - Mobile-friendly button labels that shorten on small screens
  - Horizontal scrolling tables on mobile devices
  - Touch-friendly controls and spacing

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

## URL Persistence

MudGridify supports URL persistence through the `GridifyQueryParser` utility, enabling shareable, bookmarkable filter states. This feature follows the **application developer responsibility pattern** - the component remains pure and routing-agnostic, while providing the tools you need to integrate URL persistence.

### Benefits

- **📤 Shareable**: Copy and share URLs with colleagues to show specific filtered views
- **🔖 Bookmarkable**: Save frequently used filters as browser bookmarks for quick access
- **⏮️ Browser History**: Use back/forward buttons to navigate through filter states
- **🔄 Page Refresh**: Filters survive page reload - no state lost on refresh

### GridifyQueryParser Utility

The `GridifyQueryParser` class parses Gridify query strings back into `FilterCondition` objects for state restoration.

**Namespace**: `Selmir.MudGridify.Utilities`

**Method Signature**:
```csharp
public static (List<FilterCondition> conditions, bool isOrOperator) Parse(
    string? gridifyQuery,
    List<FilterableProperty> filterableProperties)
```

**Parameters**:
- `gridifyQuery`: The Gridify query string from URL (e.g., `"FirstName=John,Age>30"`)
- `filterableProperties`: Your list of filterable properties to resolve references

**Returns**:
- `conditions`: List of parsed `FilterCondition` objects
- `isOrOperator`: `true` if OR logic is used (`|` separator), `false` for AND (`,` separator)

### Implementation Example

```razor
@page "/my-data"
@inject NavigationManager Navigation
@using System.Web

<GridifyFilterBuilder @ref="_filterBuilder"
                      FilterableProperties="@_filterableProperties"
                      InitialConditions="@_initialConditions"
                      OnFilterChanged="@OnFilterChanged" />

@code {
    private GridifyFilterBuilder? _filterBuilder;
    private List<FilterableProperty> _filterableProperties = new();
    private List<FilterCondition> _initialConditions = new();
    private List<MyData> _allData = new();
    private List<MyData> _filteredData = new();

    protected override async Task OnInitializedAsync()
    {
        // 1. Define filterable properties
        _filterableProperties = new List<FilterableProperty>
        {
            new("Name", "Name", FilterPropertyType.String),
            new("Price", "Price", FilterPropertyType.Number),
            new("CreatedDate", "Created", FilterPropertyType.Date)
        };

        // 2. Load data
        _allData = await GetDataAsync();
        _filteredData = _allData;

        // 3. Parse URL parameters to restore filter state
        await LoadFiltersFromUrl();
    }

    private async Task LoadFiltersFromUrl()
    {
        try
        {
            var uri = new Uri(Navigation.Uri);
            var queryString = HttpUtility.ParseQueryString(uri.Query);
            var filterParam = queryString["filter"];

            if (!string.IsNullOrEmpty(filterParam))
            {
                // Parse Gridify query back into FilterConditions
                var (conditions, isOrOperator) = GridifyQueryParser.Parse(
                    filterParam,
                    _filterableProperties
                );

                if (conditions.Any())
                {
                    _initialConditions = conditions;

                    // Apply filter to data immediately
                    var query = _allData.AsQueryable();
                    _filteredData = query.ApplyFiltering(filterParam).ToList();
                }
            }
        }
        catch (Exception ex)
        {
            // Handle parsing errors gracefully
            Console.WriteLine($"Error parsing URL filters: {ex.Message}");
        }
    }

    private async Task OnFilterChanged(string filterQuery)
    {
        // Update URL with new filter (without page reload)
        UpdateUrl(filterQuery);

        // Apply filter to data
        if (string.IsNullOrWhiteSpace(filterQuery))
        {
            _filteredData = new List<MyData>(_allData);
        }
        else
        {
            var query = _allData.AsQueryable();
            _filteredData = query.ApplyFiltering(filterQuery).ToList();
        }
    }

    private void UpdateUrl(string filterQuery)
    {
        var uri = new Uri(Navigation.Uri);
        var baseUrl = $"{uri.Scheme}://{uri.Authority}{uri.AbsolutePath}";

        if (string.IsNullOrWhiteSpace(filterQuery))
        {
            // Clear filter from URL
            Navigation.NavigateTo(baseUrl, forceLoad: false, replace: true);
        }
        else
        {
            // Add/update filter query parameter
            var encodedFilter = Uri.EscapeDataString(filterQuery);
            var newUrl = $"{baseUrl}?filter={encodedFilter}";
            Navigation.NavigateTo(newUrl, forceLoad: false, replace: true);
        }
    }
}
```

### Key Implementation Points

1. **Parse on Init**: Use `GridifyQueryParser.Parse()` in `OnInitializedAsync` to read URL state
2. **Pass to Component**: Set `InitialConditions` parameter with parsed conditions
3. **Update URL**: Call `NavigationManager.NavigateTo()` with `forceLoad: false, replace: true` to update URL without reload
4. **URL Encoding**: Always use `Uri.EscapeDataString()` when adding filter to URL
5. **Error Handling**: Wrap parsing in try-catch to handle malformed or invalid URLs gracefully

### URL Format

The URL structure follows this pattern:
```
https://example.com/page?filter=FirstName%3DJohn%2CAge%3E30
```

Where `filter` parameter contains the URL-encoded Gridify query string.

### Browser History Support

Using `NavigationManager.NavigateTo()` with `replace: true` updates the current history entry. To add new history entries (enabling back/forward navigation), use `replace: false`:

```csharp
Navigation.NavigateTo(newUrl, forceLoad: false, replace: false);
```

### Live Demo

See the **URL Persistence** page in the [playground application](#running-the-playground) for a working example with shareable links, browser history support, and detailed implementation notes.

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
5. **URL Persistence**: Shareable filters, bookmarks, browser history, and URL integration patterns

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
