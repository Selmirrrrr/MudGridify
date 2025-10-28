# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**MudGridify** is a Blazor component library that provides a dynamic, type-safe filter builder UI for Gridify (dynamic LINQ filtering library), built with MudBlazor. The solution consists of two projects:

- **Selmir.MudGridify**: The reusable Razor component library
- **Selmir.MudGridify.Playground**: A Blazor WASM demo application

## Key Commands

### Build & Run
```bash
# Build entire solution
dotnet build

# Clean and rebuild
dotnet clean && dotnet build

# Run the playground app (from solution root)
dotnet run --project src/Selmir.MudGridify.Playground

# Run playground from its directory
cd src/Selmir.MudGridify.Playground
dotnet run
```

The playground runs at `http://localhost:5259`.

### Testing Commands
This project does not currently have automated tests. The playground application serves as the testing/demo environment.

## Architecture

### Component Library (Selmir.MudGridify)

**Core Models** (`Models/`):
- `FilterableProperty`: Defines a filterable property with name, display name, type, and optional boolean labels
- `FilterCondition`: Represents a single filter condition (property, operator, value, case sensitivity)
- `FilterOperator`: Enum of Gridify operators (Equals, Contains, GreaterThan, etc.) with extension methods for Gridify string conversion and localization
- `FilterPropertyType`: Enum of supported data types (String, Number, Boolean, Date, DateTime)
- `LogicalOperator`: AND/OR operators for combining conditions

**Core Components** (`Components/`):
- `GridifyFilterBuilder.razor`: Main component that orchestrates the filter UI
  - Manages list of FilterConditions
  - Handles AND/OR logical operator toggle
  - Generates real-time Gridify query string
  - Provides programmatic API (GetFilterQuery, SetConditions, AddCondition)
  - Exposes OnFilterChanged event callback
- `FilterConditionRow.razor`: Individual filter row component
  - Dynamic input rendering based on FilterPropertyType
  - Property/Operator/Value dropdowns and inputs
  - Case-insensitive toggle for strings
  - Remove button

**Localization** (`Resources/`):
- Uses .NET resource files (.resx) for multi-language support
- Supports: English (default), French, German, Italian
- `Localization.cs`: Marker class for IStringLocalizer
- Components inject `IStringLocalizer<Resources.Localization>` for translations
- Embedded resources compiled into satellite assemblies (fr/, de/, it/)

### Playground Application (Selmir.MudGridify.Playground)

**Demo Pages** (`Pages/`):
- `Home.razor`: Feature overview and Gridify operator reference
- `Employees.razor`: Employee filter demo with responsive table
- `Products.razor`: Product filter demo
- `Advanced.razor`: Quick filter presets, statistics, and programmatic control examples

**Layout** (`Layout/`):
- `MainLayout.razor`: MudBlazor app structure with drawer navigation and LanguageSwitcher
- `NavMenu.razor`: Side navigation menu

**Sample Data** (`Models/`):
- `Employee.cs`: Sample employee model with various property types
- `Product.cs`: Sample product model

## Gridify Integration

The component generates valid Gridify filter query strings that can be applied to IQueryable:

```csharp
// Generated query example: "FirstName=John,Age>30"
var filtered = dataQuery.ApplyFiltering(generatedQuery).ToList();
```

### Gridify Query Syntax
- Single condition: `PropertyName=Value`
- Multiple conditions (AND): `Prop1=Val1,Prop2=Val2`
- Multiple conditions (OR): `Prop1=Val1|Prop2=Val2`
- String operators: `=*` (contains), `^` (starts with), `$` (ends with), negations with `!`
- Comparison: `>`, `<`, `>=`, `<=`, `=`, `!=`
- Case-insensitive: append `/i` (e.g., `Name=john/i`)

## Responsive Design

The solution is fully responsive with MudBlazor breakpoint system:
- **xs**: < 600px (mobile)
- **sm**: 600-960px (tablet)
- **md**: 960-1280px (small desktop)
- **lg**: 1280-1920px (desktop)
- **xl**: > 1920px (large desktop)

Components use MudGrid with responsive column specs (e.g., `xs="12" md="4"`). Buttons have adaptive labels using `<MudHidden>` to show shorter text on mobile.

## Technology Stack

- **.NET 10.0 RC2** (preview version)
- **Blazor WebAssembly** (client-side)
- **MudBlazor 8.13.0** (Material Design components)
- **Gridify 2.17.0** (dynamic LINQ filtering)
- **Microsoft.Extensions.Localization 10.0.0-rc.2** (multi-language support)

## Important Configuration

### Blazor WASM Globalization
The playground project includes `<BlazorWebAssemblyLoadAllGlobalizationData>true</BlazorWebAssemblyLoadAllGlobalizationData>` in its .csproj to support dynamic culture changes at runtime.

### Embedded Resources
The component library embeds .resx files:
```xml
<ItemGroup>
  <EmbeddedResource Update="Resources\**\*.resx" />
</ItemGroup>
```

## Development Patterns

### Adding New Filterable Properties
```csharp
new FilterableProperty("PropertyName", "Display Name", FilterPropertyType.String)
```

### Adding Localization Keys
1. Add key-value pairs to all .resx files (Localization.resx, Localization.fr.resx, etc.)
2. Use in components: `@L["KeyName"]` where `L` is `IStringLocalizer<Resources.Localization>`
3. For FilterOperator display names, pass localizer to `GetDisplayName(IStringLocalizer)`

### Component Communication
The GridifyFilterBuilder uses EventCallback pattern:
- Parent passes `OnFilterChanged` callback
- Component invokes it with generated query string whenever filters change
- Parent applies filter to data using Gridify's `ApplyFiltering(query)`

## Known Limitations

### Localization Runtime Issue
The localization infrastructure is complete (resource files, satellite assemblies, IStringLocalizer injection), but translations are not applying at runtime in Blazor WASM. The UI remains in English after language switching despite satellite resource DLLs being generated correctly. This may require additional Blazor WASM-specific configuration for proper satellite assembly loading.

## File Naming Conventions

- Blazor components: `ComponentName.razor`
- Component code-behind: `ComponentName.razor.cs` (not used in this project; code is in @code blocks)
- Models: `ModelName.cs` in `Models/` folder
- Resource files: `Localization.{culture}.resx` (e.g., Localization.fr.resx for French)
