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

        var admins = new Dictionary<string, bool>
        {
            {"b2c1cc8f-38db-4838-8f59-4a4b5393848c", false}, //deploy-sp
            {"3a668e53-336b-4d30-94bf-6620cdd036ec", true } //me
        };

        var kv = new AzKeyVault($"{shortName}-kv", _naming, rg.ResourceGroupName)
            .WithKeyVaultSecretsAdmins(Output.Create(admins)).WithSecret("test", "test");
    }
}