# Responsive Design Implementation

## Overview

The MudGridify filter builder component is fully responsive and optimized for all screen sizes, from mobile phones to large desktop displays. This document outlines the responsive design strategies implemented throughout the application.

## Breakpoint Strategy

MudBlazor uses the following breakpoint system (following Material Design guidelines):

| Breakpoint | Size Range | Device Type |
|------------|------------|-------------|
| xs | < 600px | Mobile phones (portrait) |
| sm | 600px - 960px | Tablets (portrait), Large phones (landscape) |
| md | 960px - 1280px | Tablets (landscape), Small laptops |
| lg | 1280px - 1920px | Desktops, Large laptops |
| xl | > 1920px | Large desktops |

## Component Responsive Features

### FilterConditionRow

**Grid Layout Breakpoints:**
- **Mobile (xs)**: Full width (12 columns) for all inputs
  - Property selector: 12/12
  - Operator selector: 12/12
  - Value input: 12/12
  - Actions (checkbox + delete): 12/12

- **Tablet (sm)**: Optimized 2-column layout
  - Property selector: 12/12 (full width)
  - Operator selector: 6/12 (half width)
  - Value input: 6/12 (half width)
  - Actions: 12/12 (full width)

- **Desktop (md/lg)**: Full horizontal layout
  - Property selector: 3/12
  - Operator selector: 3/12 (md) or 2/12 (lg)
  - Value input: 4/12 (md) or 5/12 (lg)
  - Actions: 2/12

**Responsive Features:**
- `FullWidth="true"` on all form inputs ensures proper sizing
- Case-insensitive checkbox label changes from "Aa" to "Case Insensitive" on larger screens
- Delete button size adjusts based on available space

### GridifyFilterBuilder

**Header Layout:**
- **Mobile (xs)**: Stacked layout
  - Title: Full width
  - Controls: Full width with wrapping buttons
  - Button text shortens ("Add Filter" → "Add", "Clear All" → "Clear")

- **Tablet/Desktop (sm+)**: Horizontal layout
  - Title: Left-aligned (6/12 on sm, 4/12 on md+)
  - Controls: Right-aligned (6/12 on sm, 8/12 on md+) with flex-wrap

**Button Labels:**
- Implemented with `MudHidden` component for conditional rendering
- Short labels on xs screens, full labels on sm+ screens
- Examples:
  - "Add Filter" (desktop) → "Add" (mobile)
  - "Clear All" (desktop) → "Clear" (mobile)
  - "Reset Filters" (desktop) → "Reset" (mobile)

### Playground Pages

#### Data Tables

All data tables include:
- `Breakpoint="Breakpoint.Sm"`: Tables become scrollable cards on mobile
- `HorizontalScrollbar="true"`: Enables horizontal scrolling when needed
- Proper `DataLabel` attributes for mobile card view

#### Quick Filter Presets (Advanced Page)

**Grid Layout:**
- **Mobile (xs)**: 1 column - full width buttons
- **Tablet (sm)**: 2 columns - buttons in pairs
- **Desktop (md)**: 3-4 columns - compact layout
- **Large (lg)**: Optimized spacing with variable widths

**Button Text Adaptation:**
- Long descriptions on large screens: "High Earners (Salary > $100k)"
- Shortened on medium/small: "High Earners"
- Maintains full functionality on all sizes

#### Statistics Cards

Responsive grid for metric cards:
- **Mobile (xs)**: 1 column (12/12) - stacked
- **Tablet (sm)**: 2 columns (6/12) - side by side
- **Desktop (md+)**: 4 columns (3/12) - full row

## CSS Classes Used

### MudBlazor Utility Classes

- `d-flex`: Display flex container
- `align-center`: Vertically center items
- `justify-end`: Right-align items
- `justify-space-between`: Space items evenly
- `flex-wrap`: Allow items to wrap
- `flex-grow-1`: Grow to fill space
- `flex-sm-grow-0`: Don't grow on small+ screens

### Custom Responsive Classes

```html
<!-- Button that grows on mobile, fixed on desktop -->
<div Class="flex-grow-1 flex-sm-grow-0">

<!-- Justify end on small+, stretch on xs -->
<div Class="justify-sm-end justify-xs-stretch">
```

## Testing Responsive Design

### Browser DevTools

1. Open browser DevTools (F12)
2. Toggle device toolbar (Ctrl+Shift+M / Cmd+Shift+M)
3. Test these key breakpoints:
   - 360px (Mobile S)
   - 768px (Tablet)
   - 1024px (Desktop)
   - 1920px (Large Desktop)

### Key Areas to Test

1. **Filter Condition Rows**
   - Inputs stack properly on mobile
   - Layout adapts smoothly across breakpoints
   - Buttons remain accessible

2. **Filter Builder Header**
   - Controls wrap appropriately
   - Button labels adapt to screen size
   - Logical operator toggle remains accessible

3. **Data Tables**
   - Horizontal scroll works on mobile
   - Card layout displays properly
   - All columns remain visible

4. **Preset Buttons**
   - Grid layout adapts properly
   - Text truncates or abbreviates on small screens
   - Buttons remain touch-friendly (min 44px tap target)

## Best Practices Implemented

1. **Mobile-First Approach**: Start with mobile layout (xs), enhance for larger screens
2. **Touch-Friendly**: Minimum 44x44px tap targets for all interactive elements
3. **Content Priority**: Most important content/actions visible without scrolling on mobile
4. **Readable Text**: Font sizes remain readable across all screen sizes
5. **Performance**: Breakpoint-specific rendering doesn't impact performance
6. **Accessibility**: Semantic HTML and ARIA labels work across all screen sizes

## Future Enhancements

Potential responsive improvements:
- [ ] Add landscape-specific optimizations for mobile
- [ ] Implement collapsible filter rows on very small screens
- [ ] Add swipe gestures for mobile table navigation
- [ ] Create a compact "chip" view for applied filters on mobile
- [ ] Add bottom sheet for filter controls on mobile

## Troubleshooting

### Common Issues

**Buttons overlap on mobile:**
- Ensure `flex-wrap` is enabled on parent container
- Check that buttons have proper spacing (`gap` or `Spacing` props)

**Table columns too narrow:**
- Verify `Breakpoint` is set on MudTable
- Ensure `HorizontalScrollbar="true"` is enabled

**Labels cut off:**
- Use `MudHidden` with `Invert` for conditional text
- Check that container has proper width constraints

**Grid layout breaks:**
- Verify column widths add up to 12
- Check that all breakpoints are defined (xs, sm, md, lg)
- Ensure `Spacing` prop is consistent across MudGrid

## Resources

- [MudBlazor Grid System](https://mudblazor.com/components/grid)
- [MudBlazor Breakpoints](https://mudblazor.com/features/breakpoints)
- [Material Design Responsive Layout](https://material.io/design/layout/responsive-layout-grid.html)
