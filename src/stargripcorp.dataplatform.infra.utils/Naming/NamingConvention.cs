using System.Text.RegularExpressions;

namespace stargripcorp.dataplatform.infra.utils.Naming;

public class NamingConvention
{
    private readonly string owner;
    private readonly string shortName;
    private readonly string environment;
    private readonly Dictionary<string, Func<string, string>> resourceTypeGenerators;
    private readonly ResourceTypeAbbreviations resourceTypeAbbreviations;

    public NamingConvention(string owner, string shortName, string environment, string cloudProvider)
    {
        this.owner = owner;
        this.shortName = shortName;
        this.environment = environment;
        resourceTypeAbbreviations = cloudProvider == "azure" ?
            new AzureResourceTypeAbbreviations() : 
            throw new ArgumentException(cloudProvider, $"No abbreviations for {cloudProvider}");

        resourceTypeGenerators = new Dictionary<string, Func<string, string>>
        {
            { "azure-native:storage:StorageAccount", GenerateStorageAccountResourceName },
            { "azure-native:keyvault:Vault", GenerateKeyVaultResourceName }
        };
    }

    public string GetResourceName(string resourceType)
    {
        if (!resourceTypeGenerators.TryGetValue(resourceType, out Func<string, string>? value))
        {
            throw new ArgumentException($"Unknown resource type: {resourceType}");
        }
        var generator = value;
        return generator(resourceType);
    }

    public string GenerateResourceId(string resourceType)
    {
        if (!resourceTypeGenerators.ContainsKey(resourceType))
        {
            throw new ArgumentException($"Unknown resource type: {resourceType}");
        }

        var resourceTypeAbbreviation = resourceTypeAbbreviations.GetAbbreviation(resourceType);
        var resourceName = $"{owner}-{shortName}-{environment}-{resourceTypeAbbreviation}";
        return $"{resourceName}";
    }

    private string GenerateStorageAccountResourceName(string resourceType)
    {
        var resourceTypeAbbreviation = resourceTypeAbbreviations.GetAbbreviation(resourceType);
        var shortNameLength = 17 - resourceTypeAbbreviation.Length;
        var name = $"{owner[..Math.Min(owner.Length, 5)]}{shortName}{environment}{resourceTypeAbbreviation}";
        name = StringExpressions.NoSpecialCharactersLowerCase().Replace(name, "");
        return name.Length > 24 ? $"{owner[..Math.Min(owner.Length, 5)]}{shortName[..shortNameLength]}{environment}{resourceTypeAbbreviation}" : name;
    }

    private string GenerateKeyVaultResourceName(string resourceType)
    {
        var resourceTypeAbbreviation = resourceTypeAbbreviations.GetAbbreviation(resourceType);
        var shortNameLength = Math.Min(12, shortName.Length);
        var name = $"{owner}-{shortName[..shortNameLength]}-{environment}-{resourceTypeAbbreviation}";
        name = StringExpressions.AllowDashes().Replace(name, "");
        return name.Length > 24 ? $"{owner[..Math.Min(owner.Length, 5)]}-{shortName[..shortNameLength]}-{environment}-{resourceTypeAbbreviation}" : name;
    }
}