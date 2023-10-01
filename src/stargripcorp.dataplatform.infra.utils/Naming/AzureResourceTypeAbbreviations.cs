namespace stargripcorp.dataplatform.infra.utils.Naming;

internal class AzureResourceTypeAbbreviations : ResourceTypeAbbreviations
{
    public AzureResourceTypeAbbreviations()
    {
        Add("azure-native:resources:ResourceGroup", "rg");
        Add("azure-native:storage:StorageAccount", "sa");
        Add("azure-native:keyvault:Vault", "kv");
        Add("azure-native:authorization:RoleAssignment", "ra");
        Add("azure-native:resources:Budget", "budget");
        Add("azure-native:keyvault:Secret", "secret");
        // Add more Azure resource types here
    }
}
