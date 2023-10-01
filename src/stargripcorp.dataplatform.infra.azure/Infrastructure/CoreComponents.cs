using Pulumi;
using Pulumi.AzureNative.Authorization;
using stargripcorp.dataplatform.infra.azure.Resources;
using stargripcorp.dataplatform.infra.utils.Naming;
using stargripcorp.dataplatform.infra.utils.Stack;

internal class CoreComponents
{
    private readonly StackConfig _config;
    private readonly NamingConvention _naming;
    private readonly string shortName = "core";
    public CoreComponents(StackConfig config, NamingConvention naming)
    {
        _config = config;
        _naming = naming;
    }
    public void Run()
    {
        var rg = new AzResourceGroup($"{shortName}-rg", _naming).WithBudget(20, ["merca@cetera.desunt.com"]);
        var currentServicePrincipalId = Output.Create(GetClientConfig.InvokeAsync()).Apply(c => c.ObjectId);
        var kv = new AzKeyVault($"{shortName}-kv", _naming, rg.ResourceGroupName)
            .WithSecretsContributor(new List<Output<string>> {
                currentServicePrincipalId,
            }).WithSecret("test", "test");
    }
}