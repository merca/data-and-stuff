using Pulumi;
using stargripcorp.dataplatform.infra.utils.Naming;
using Azure = Pulumi.AzureNative;

namespace stargripcorp.dataplatform.infra.azure.Resources;

internal class AzStorage(string id, NamingConvention naming, Output<string> resourceGroupName) : ComponentResource("pkg:azure:storage", id)
{

    public void GenerateStorageAccount(bool isHnsEnabled)
    {
        _ = new Azure.Storage.StorageAccount(naming.GenerateResourceId("azure-native:storage:StorageAccount"), new()
        {
            AccountName = naming.GetResourceName("azure-native:storage:StorageAccount"),
            ResourceGroupName = resourceGroupName,
            Sku = new Azure.Storage.Inputs.SkuArgs
            {
                Name = Azure.Storage.SkuName.Standard_LRS,
            },
            Kind = Azure.Storage.Kind.StorageV2,
            EnableHttpsTrafficOnly = true,
            AccessTier = Azure.Storage.AccessTier.Hot,
            AllowBlobPublicAccess = false,
            NetworkRuleSet = new Azure.Storage.Inputs.NetworkRuleSetArgs
            {
                Bypass = Azure.Storage.Bypass.AzureServices,
                DefaultAction = Azure.Storage.DefaultAction.Deny,
            },
            MinimumTlsVersion = Azure.Storage.MinimumTlsVersion.TLS1_2,
            IsHnsEnabled = isHnsEnabled,
        }, new CustomResourceOptions { Parent = this });
    }
    public AzStorage AsDataLake()
    {
        GenerateStorageAccount(true);
        return this;
    }
    public AzStorage AsBlobStorage()
    {
        GenerateStorageAccount(false);
        return this;
    }
}
