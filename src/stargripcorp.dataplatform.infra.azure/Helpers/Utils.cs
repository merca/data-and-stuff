using Pulumi.AzureNative.Authorization;
using System.Net.Http.Headers;
using System.Text.Json;

namespace stargripcorp.dataplatform.infra.azure.Helpers
{
    internal class Utils
    {
        public static async Task<string> GetRoleIdByName(string roleName, string? scope = null)
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
                throw new Exception($"Request failed with {response.StatusCode}");
            }
            var body = await response.Content.ReadAsStringAsync();
            var definition = JsonSerializer.Deserialize<RoleDefinition>(body);
            return definition!.value[0].id;
        }
    }
    public record RoleDefinition(List<RoleDefinitionValue> value);
    public record RoleDefinitionValue(string id, string type, string name);
}
