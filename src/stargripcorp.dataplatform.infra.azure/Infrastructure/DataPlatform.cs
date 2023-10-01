using stargripcorp.dataplatform.infra.azure.Resources;
using stargripcorp.dataplatform.infra.utils.Naming;
using stargripcorp.dataplatform.infra.utils.Stack;

namespace stargripcorp.dataplatform.infra.azure.Infrastructure;

internal class DataPlatform(StackConfig config, NamingConvention naming, Dictionary<string, string> tags)
{
    private readonly StackConfig _config = config;
    private readonly NamingConvention _naming = naming;
    private readonly string shortName = "data";
    private readonly Dictionary<string, string> _tags = tags;

    public void Run()
    {
        var rg = new AzResourceGroup($"{shortName}-rg", _naming, _tags).WithBudget(20, ["merca.ovnerud@pulumi.me"]);
        var storage = new AzStorage($"{shortName}-sa", _naming, rg.ResourceGroupName).AsDataLake();
    }
}
