using Pulumi.AzureNative.Authorization;
using stargripcorp.dataplatform.infra.azure.Exceptions;
using System.Net.Http.Headers;
using System.Text.Json;

namespace stargripcorp.dataplatform.infra.azure.Helpers;

internal static class Utils
{
    public static async Task<string> GetRoleIdByNameAsync(string roleName)
    {
        var config = await GetClientConfig.InvokeAsync();
        var token = await GetClientToken.InvokeAsync();

        // Unfortunately, Microsoft hasn't shipped an .NET5-compatible SDK at the time of writing this.
        // So, we have to hand-craft an HTTP request to retrieve a role definition.
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        var response = await httpClient.GetAsync($"https://management.azure.com/subscriptions/{config.SubscriptionId}/providers/Microsoft.Authorization/roleDefinitions?api-version=2018-01-01-preview&$filter=roleName%20eq%20'{roleName}'");
        if (!response.IsSuccessStatusCode)
        {
            throw new ApiResponseException($"Request failed with {response.StatusCode}");
        }
        var body = await response.Content.ReadAsStringAsync();
        var definition = JsonSerializer.Deserialize<RoleDefinition>(body);
        return definition!.value[0].id;
    }
}
#pragma warning disable IDE1006 // Naming Styles
public record RoleDefinition(List<RoleDefinitionValue> value);
public record RoleDefinitionValue(string id, string type, string name);
#pragma warning restore IDE1006 // Naming Styles