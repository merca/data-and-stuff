using stargripcorp.dataplatform.infra.azure.Resources;
using stargripcorp.dataplatform.infra.utils.Naming;
using stargripcorp.dataplatform.infra.utils.Stack;

namespace stargripcorp.dataplatform.infra.azure.Infrastructure;

internal class DataPlatform
{
    private readonly StackConfig _config;
    private readonly NamingConvention _naming;
    private readonly string shortName = "data";

    public DataPlatform(StackConfig config, NamingConvention naming)
    {
        _config = config;
        _naming = naming;
    }
    public void Run()
    {
        var rg = new AzResourceGroup($"{shortName}-rg", _naming).WithBudget(20, ["merca.ovnerud@pulumi.me"]);
        var storage = new AzStorage($"{shortName}-sa", _naming, rg.ResourceGroupName).AsDataLake();
    }
}
