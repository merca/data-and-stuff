﻿using stargripcorp.dataplatform.infra.azure.Resources;
using stargripcorp.dataplatform.infra.utils.Naming;

namespace stargripcorp.dataplatform.infra.azure.Infrastructure;

internal class DataPlatform(NamingConvention naming, Dictionary<string, string> tags)
{
    private readonly string shortName = "data";

    public void Run()
    {
        var rg = new AzResourceGroup($"{shortName}-rg", naming, tags).WithBudget(20, ["merca.ovnerud@pulumi.me"]);
        _ = new AzStorage($"{shortName}-sa", naming, rg.ResourceGroupName).AsDataLake();
    }
}
