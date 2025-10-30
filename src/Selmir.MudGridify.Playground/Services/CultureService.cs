using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Globalization;

namespace Selmir.MudGridify.Playground.Services;

public interface ICultureService
{
    Task<string> GetCurrentCultureAsync();
    Task SetCultureAsync(string culture);
}

public class CultureService : ICultureService
{
    private readonly ILocalStorageService _localStorage;
    private readonly NavigationManager _navigationManager;
    private readonly IJSRuntime _jsRuntime;
    private const string CultureKey = "culture";

    public CultureService(ILocalStorageService localStorage, NavigationManager navigationManager, IJSRuntime jsRuntime)
    {
        _localStorage = localStorage;
        _navigationManager = navigationManager;
        _jsRuntime = jsRuntime;
    }

    public async Task<string> GetCurrentCultureAsync()
    {
        var culture = await _localStorage.GetItemAsStringAsync(CultureKey);
        return culture ?? "en";
    }

    public async Task SetCultureAsync(string culture)
    {
        // Save the culture to localStorage
        await _localStorage.SetItemAsStringAsync(CultureKey, culture);

        // Reload the page to apply the new culture
        await _jsRuntime.InvokeVoidAsync("location.reload");
    }
}