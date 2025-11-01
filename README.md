# MudGridify

A Blazor filter builder UI for [Gridify](https://alirezanet.github.io/Gridify/) that actually feels good to use. Built with [MudBlazor](https://mudblazor.com/).

## What it does

MudGridify gives you a dynamic filter builder that works seamlessly with Gridify. Instead of writing filter queries by hand, your users get a proper UI with dropdowns, inputs, and buttons that generate the queries for them.

The component is type-aware – it knows when to show a date picker versus a text input versus a number field. It supports all Gridify operators (equals, contains, starts with, greater than, etc.) and lets you combine conditions with AND/OR logic.

## Key features

- **Smart property types** – Configure what users can filter on, and the component renders the right controls automatically
- **All Gridify operators** – Full support for `=`, `!=`, `>`, `<`, `>=`, `<=`, `=*`, `!*`, `^`, `!^`, `$`, `!$`
- **AND/OR logic** – Combine conditions however you need
- **Case-insensitive matching** – Optional `/i` suffix for string comparisons
- **Live query preview** – See the Gridify query string as you build filters
- **URL persistence** – Share filtered views via URL, bookmark them, or use browser back/forward
- **Mobile-friendly** – Works on phones, tablets, and desktops with responsive layouts
- **Programmatic control** – Set, add, or clear filters from code

## Projects in this repo

- **Selmir.MudGridify** – The component library
- **Selmir.MudGridify.Playground** – Demo app you can run locally

## Quick start

**1. Add the packages**
```xml
<ItemGroup>
  <PackageReference Include="MudBlazor" Version="8.13.0" />
  <PackageReference Include="Gridify" Version="2.17.0" />
  <ProjectReference Include="../Selmir.MudGridify/Selmir.MudGridify.csproj" />
</ItemGroup>
```

**2. Configure MudBlazor**
```csharp
builder.Services.AddMudServices();
```

**3. Use it in your page**
```razor
@page "/products"
@using Selmir.MudGridify.Components
@using Selmir.MudGridify.Models

<GridifyFilterBuilder @ref="_filterBuilder"
                      FilterableProperties="@_filterableProperties"
                      OnFilterChanged="@OnFilterChanged" />

<MudTable Items="@_filteredData" />

@code {
    private GridifyFilterBuilder? _filterBuilder;
    private List<FilterableProperty> _filterableProperties = new();
    private List<Product> _allData = new();
    private List<Product> _filteredData = new();

    protected override void OnInitialized()
    {
        _filterableProperties = new()
        {
            new("Name", "Product Name", FilterPropertyType.String),
            new("Price", "Price", FilterPropertyType.Number),
            new("InStock", "Available", FilterPropertyType.Boolean)
            {
                TrueLabel = "Yes",
                FalseLabel = "No"
            }
        };

        _allData = GetProducts();
        _filteredData = _allData;
    }

    private async Task OnFilterChanged(string filterQuery)
    {
        if (string.IsNullOrWhiteSpace(filterQuery))
        {
            _filteredData = _allData;
        }
        else
        {
            _filteredData = _allData.AsQueryable()
                .ApplyFiltering(filterQuery)
                .ToList();
        }
    }
}
```

## Component parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `FilterableProperties` | `List<FilterableProperty>` | Yes | What properties users can filter on |
| `InitialConditions` | `List<FilterCondition>` | No | Pre-populate filters (useful for URL restoration) |
| `ShowGeneratedQuery` | `bool` | No | Show the Gridify query string (default: true) |
| `OnFilterChanged` | `EventCallback<string>` | No | Fires when filters change |

## Configuring properties
```csharp
var properties = new List<FilterableProperty>
{
    // Simple string property
    new("FirstName", "First Name", FilterPropertyType.String),
    
    // Number with comparison operators
    new("Age", "Age", FilterPropertyType.Number),
    
    // Date picker
    new("HireDate", "Hire Date", FilterPropertyType.Date),
    
    // Boolean with custom labels
    new("IsActive", "Status", FilterPropertyType.Boolean)
    {
        TrueLabel = "Active",
        FalseLabel = "Inactive"
    }
};
```

## URL persistence (shareable filters)

You can make filter states shareable via URL. The component doesn't handle routing – that's your job – but it gives you the tools to do it cleanly.

**Why bother?**
- Share filtered views with teammates via link
- Bookmark frequently used filters
- Page refresh doesn't lose filter state
- Browser back/forward navigation works

**How it works:**
```razor
@inject NavigationManager Navigation
@using System.Web

<GridifyFilterBuilder @ref="_filterBuilder"
                      FilterableProperties="@_filterableProperties"
                      InitialConditions="@_initialConditions"
                      OnFilterChanged="@OnFilterChanged" />

@code {
    private List<FilterCondition> _initialConditions = new();

    protected override async Task OnInitializedAsync()
    {
        _filterableProperties = DefineProperties();
        _allData = await LoadData();

        // Restore filters from URL
        await LoadFiltersFromUrl();
    }

    private async Task LoadFiltersFromUrl()
    {
        var uri = new Uri(Navigation.Uri);
        var query = HttpUtility.ParseQueryString(uri.Query);
        var filterParam = query["filter"];

        if (!string.IsNullOrEmpty(filterParam))
        {
            // Parse the Gridify query string back into FilterConditions
            // The parser now sets the NextLogicalOperator on each condition
            var conditions = GridifyQueryParser.Parse(
                filterParam,
                _filterableProperties
            );

            if (conditions.Any())
            {
                _initialConditions = conditions;
                _filteredData = _allData.AsQueryable()
                    .ApplyFiltering(filterParam)
                    .ToList();
            }
        }
    }

    private void OnFilterChanged(string filterQuery)
    {
        UpdateUrl(filterQuery);
        ApplyFilter(filterQuery);
    }

    private void UpdateUrl(string filterQuery)
    {
        var uri = new Uri(Navigation.Uri);
        var baseUrl = $"{uri.Scheme}://{uri.Authority}{uri.AbsolutePath}";
        
        if (string.IsNullOrWhiteSpace(filterQuery))
        {
            Navigation.NavigateTo(baseUrl, forceLoad: false, replace: true);
        }
        else
        {
            var encoded = Uri.EscapeDataString(filterQuery);
            Navigation.NavigateTo($"{baseUrl}?filter={encoded}", 
                forceLoad: false, replace: true);
        }
    }
}
```

## Programmatic control
```csharp
// Get current query
string query = _filterBuilder.GetFilterQuery();

// Set filters from code
_filterBuilder.SetConditions(new List<FilterCondition>
{
    new() 
    { 
        Property = salaryProperty,
        Operator = FilterOperator.GreaterThan,
        Value = "100000"
    }
});

// Add a condition
_filterBuilder.AddCondition(new FilterCondition { ... });
```

## Example queries

Here's what the component generates:
```
FirstName=John                          # Exact match
FirstName=*john/i                       # Contains, case-insensitive
FirstName^J                             # Starts with J
Email$@company.com                      # Ends with domain
Age>30,Department=Sales                 # AND logic
Department=Sales|Department=Marketing   # OR logic
Salary>=100000,HireDate<2023-01-01      # Combined conditions
```

## Running the demo
```bash
cd src/Selmir.MudGridify.Playground
dotnet run
```

Open `http://localhost:5259` to see examples with employees, products, and URL persistence patterns.

## Tech stack

- .NET 9
- Blazor WebAssembly
- MudBlazor 8.13.0
- Gridify 2.17.0

## Credits

Built on top of [Gridify](https://github.com/alirezanet/Gridify) by Alireza Sabouri and [MudBlazor](https://github.com/MudBlazor/MudBlazor).

## License

Use it however you want – it's here for you.