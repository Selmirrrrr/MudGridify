using Microsoft.Extensions.Localization;
using System.Globalization;

namespace Selmir.MudGridify.Services;

/// <summary>
/// Service for managing localization throughout the component library
/// </summary>
public class LocalizationService
{
    private readonly IStringLocalizer _localizer;
    private CultureInfo _currentCulture;

    public event Action? OnCultureChanged;

    public LocalizationService(IStringLocalizer<Resources.Localization> localizer)
    {
        _localizer = localizer;
        _currentCulture = CultureInfo.CurrentCulture;
    }

    /// <summary>
    /// Gets the current culture
    /// </summary>
    public CultureInfo CurrentCulture => _currentCulture;

    /// <summary>
    /// Gets a localized string by key
    /// </summary>
    public string this[string key] => _localizer[key];

    /// <summary>
    /// Gets a localized string by key with fallback
    /// </summary>
    public string GetString(string key, string? fallback = null)
    {
        var localizedString = _localizer[key];
        return localizedString.ResourceNotFound && fallback != null
            ? fallback
            : localizedString.Value;
    }

    /// <summary>
    /// Sets the current culture
    /// </summary>
    public void SetCulture(CultureInfo culture)
    {
        if (_currentCulture.Name == culture.Name)
            return;

        _currentCulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        OnCultureChanged?.Invoke();
    }

    /// <summary>
    /// Sets the current culture by language code
    /// </summary>
    public void SetCulture(string cultureName)
    {
        try
        {
            var culture = CultureInfo.GetCultureInfo(cultureName);
            SetCulture(culture);
        }
        catch (CultureNotFoundException)
        {
            // Fallback to default culture
            SetCulture(new CultureInfo("en"));
        }
    }
}
