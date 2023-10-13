using Pulumi;
using stargripcorp.dataplatform.infra.azure.Resources;
using stargripcorp.dataplatform.infra.utils.Naming;

namespace stargripcorp.dataplatform.infra.azure.Infrastructure;

internal class DataPlatform(NamingConvention naming, Dictionary<string, string> tags)
{
    public void Run()
    {
        var admins = new Dictionary<string, bool>
        {
            {"f5d889ea-e64e-467a-9240-f875ff284c04", false}, //deploy-sp
            {"3a668e53-336b-4d30-94bf-6620cdd036ec", true } //me
        };
        var shortName = "data";
        var rg = new AzResourceGroup($"{shortName}-rg", naming, tags).WithBudget(20, ["merca.ovnerud@pulumi.me"]);
        _ = new AzStorage($"{shortName}-sa", naming, rg.ResourceGroupName).AsDataLake();
        _ = new AzKeyVault($"{shortName}-kv", naming, rg.ResourceGroupName, tags)
            .WithKeyVaultSecretsAdmins(Output.Create(admins)).WithSecret("test", "test");
    }
}
