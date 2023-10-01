using Pulumi;
using Pulumi.AzureNative.Authorization;
using stargripcorp.dataplatform.infra.azure.Resources;
using stargripcorp.dataplatform.infra.utils.Naming;
using stargripcorp.dataplatform.infra.utils.Stack;

namespace stargripcorp.dataplatform.infra.azure.Infrastructure;

internal class CoreComponents(StackConfig config, NamingConvention naming, Dictionary<string, string> tags)
{
    private readonly StackConfig _config = config;
    private readonly NamingConvention _naming = naming;
    private readonly string shortName = "core";
    private readonly Dictionary<string, string> _tags = tags;

    public void Run()
    {
        var rg = new AzResourceGroup($"{shortName}-rg", _naming, _tags).WithBudget(20, ["merca@cetera.desunt.com"]);
        var currentServicePrincipalId = Output.Create(GetClientConfig.InvokeAsync()).Apply(c => c.ObjectId);

        var admins = new Dictionary<string, bool>
    {
        {"f5d889ea-e64e-467a-9240-f875ff284c04", false}, //deploy-sp
        {"3a668e53-336b-4d30-94bf-6620cdd036ec", true } //me
    };

        _ = new AzKeyVault($"{shortName}-kv", _naming, rg.ResourceGroupName, _tags)
            .WithKeyVaultSecretsAdmins(Output.Create(admins)).WithSecret("test", "test");
    }
}