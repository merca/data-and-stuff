using System.Text.RegularExpressions;

namespace stargripcorp.dataplatform.infra.utils.Naming;

public partial class NamingConvention
{
    private readonly string owner;
    private readonly string shortName;
    private readonly string environment;

    private readonly ResourceTypeAbbreviations resourceTypeAbbreviations;
    private static readonly string[] sourceArray = ["dev", "test", "stage", "prod"];

    public NamingConvention(string owner, string shortName, string environment, string cloudProvider)
    {
        if (owner.Length > 5)
        {
            throw new ArgumentException("Owner name must be 5 characters or less.");
        }

        if (!sourceArray.Contains(environment))
        {
            throw new ArgumentException("Environment must be one of: dev, test, stage, prod.");
        }

        this.owner = owner;
        this.shortName = shortName;
        this.environment = environment;

        resourceTypeAbbreviations = cloudProvider switch
        {
            "azure" => new AzureResourceTypeAbbreviations(),
            _ => throw new ArgumentException($"Unknown cloud provider: {cloudProvider}"),
        };
    }

    public string GetResourceId(string resourceType)
    {
        if (!resourceTypeAbbreviations.Contains(resourceType))
        {
            throw new ArgumentException($"Unknown resource type: {resourceType}");
        }

        var resourceTypeAbbreviation = resourceTypeAbbreviations.GetAbbreviation(resourceType);

        return $"{owner}-{shortName}-{environment}-{resourceTypeAbbreviation}";
    }

    public string GetResourceName(string resourceType)
    {
        if (!resourceTypeAbbreviations.Contains(resourceType))
        {
            throw new ArgumentException($"Unknown resource type: {resourceType}");
        }

        var resourceTypeAbbreviation = resourceTypeAbbreviations.GetAbbreviation(resourceType);

        return resourceType switch
        {
            "azure-native:storage:StorageAccount" => GetStorageAccountResourceName(resourceTypeAbbreviation),
            "azure-native:keyvault:Vault" => GetKeyVaultResourceName(resourceTypeAbbreviation),
            _ => GetDefaultResourceName(resourceTypeAbbreviation),
        };
    }

    private string GetStorageAccountResourceName(string resourceTypeAbbreviation)
    {
        var name = $"{owner[0 .. Math.Min(owner.Length, 5)]}{shortName}{environment}{resourceTypeAbbreviation}";
        name = NoSpecialCharsRegex().Replace(name, "");
        if (name.Length > 24)
        {
            var shortNameLength = 24 - $"{nameof(owner)[0 .. Math.Min(owner.Length, 5)]}{environment}{resourceTypeAbbreviation}".Length;
            name = $"{owner[..Math.Min(owner.Length, 5)]}{shortName[.. Math.Min(shortName.Length, shortNameLength)]}{environment}{resourceTypeAbbreviation}";
        }
        return name;
    }

    private string GetKeyVaultResourceName(string resourceTypeAbbreviation)
    {
        var name = $"{owner[..Math.Min(owner.Length, 5)]}-{shortName}-{environment}-{resourceTypeAbbreviation}";
        name = NoSpecialCharsRegex().Replace(name, "");
        if (name.Length > 24)
        {
            // 24 is allowed for key vaults, but using 23 since we add a hyphen
            var shortNameLength = 23 - $"{nameof(owner)[.. Math.Min(owner.Length, 5)]}-{environment}-{resourceTypeAbbreviation}".Length;
            name = $"{owner[.. Math.Min(owner.Length, 5)]}-{shortName[.. Math.Min(shortName.Length, shortNameLength)]}-{environment}-{resourceTypeAbbreviation}";
        }
        return name;
    }

    private string GetDefaultResourceName(string resourceTypeAbbreviation)
    {
        return $"{owner}-{shortName}-{environment}-{resourceTypeAbbreviation}";
    }

    [GeneratedRegex(@"[^a-zA-Z0-9-]")]
    private static partial Regex NoSpecialCharsRegex();
}