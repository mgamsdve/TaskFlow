using Microsoft.JSInterop;

namespace TaskFlow.Blazor.Services;

public sealed class LocalStorageService
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public ValueTask SetItemAsync(string key, string value)
    {
        return _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
    }

    public ValueTask<string?> GetItemAsync(string key)
    {
        return _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
    }

    public ValueTask RemoveItemAsync(string key)
    {
        return _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }
}
