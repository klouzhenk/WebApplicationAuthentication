using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace AuthenticationUI.Components.Shared;

 public class ChooseLanguageBase : ComponentBase
{
    [Inject] protected NavigationManager NavigationManager { get; set; }
    [Inject] protected IConfiguration Configuration { get; set; }
    [Inject] protected IStringLocalizer<App> Localizer { get; set; }

    protected string selectedCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

    protected Dictionary<string, string> cultures;

    protected override void OnInitialized()
    {
        cultures = Configuration.GetSection("Cultures")
            .GetChildren().ToDictionary(x => x.Key, x => x.Value);
    }

    protected async Task RequestCultureChange(ChangeEventArgs e)
    {
        if (selectedCulture.Equals(e.Value.ToString())) return;
        selectedCulture = e.Value.ToString();
        if (string.IsNullOrWhiteSpace(selectedCulture)) return;

        var uri = new Uri(NavigationManager.Uri).GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
        var query = $"?culture={Uri.EscapeDataString(selectedCulture)}&" +
                $"redirectUri={Uri.EscapeDataString(uri)}";
        NavigationManager.NavigateTo("/Culture/SetCulture" + query, forceLoad: true);
        await InvokeAsync(StateHasChanged);
    }
}