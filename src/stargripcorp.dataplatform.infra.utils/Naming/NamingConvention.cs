using System.Text.RegularExpressions;

namespace stargripcorp.dataplatform.infra.utils.Naming;

public class NamingConvention
{
    private readonly string owner;
    private readonly string shortName;
    private readonly string environment;

    private readonly ResourceTypeAbbreviations resourceTypeAbbreviations;

    public NamingConvention(string owner, string shortName, string environment, string cloudProvider)
    {
        if (owner.Length > 5)
        {
            throw new ArgumentException("Owner name must be 5 characters or less.");
        }

        if (!new[] { "dev", "test", "stage", "prod" }.Contains(environment))
        {
            throw new ArgumentException("Environment must be one of: dev, test, stage, prod.");
        }

        this.owner = owner;
        this.shortName = shortName;
        this.environment = environment;

        switch (cloudProvider)
        {
            case "azure":
                resourceTypeAbbreviations = new AzureResourceTypeAbbreviations();
                break;
            //case "aws":
            //    resourceTypeAbbreviations = new AwsResourceTypeAbbreviations();
            //    break;
            //case "gcp":
            //    resourceTypeAbbreviations = new GcpeResourceTypeAbbreviations();
            //    break;
            default:
                throw new ArgumentException($"Unknown cloud provider: {cloudProvider}");
        }
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

        switch (resourceType)
        {
            case "azure-native:storage:StorageAccount":
                return GetStorageAccountResourceName(resourceTypeAbbreviation);
            case "azure-native:keyvault:Vault":
                return GetKeyVaultResourceName(resourceTypeAbbreviation);
            default:
                return GetDefaultResourceName(resourceTypeAbbreviation);
        }
    }

    private string GetStorageAccountResourceName(string resourceTypeAbbreviation)
    {
        var name = $"{owner.Substring(0, Math.Min(owner.Length, 5))}{shortName}{environment}{resourceTypeAbbreviation}";
        name = Regex.Replace(name, @"[^a-zA-Z0-9]", "");
        if (name.Length > 24)
        {
            var shortNameLength = 24 - $"{nameof(owner).Substring(0, Math.Min(owner.Length, 5))}{environment}{resourceTypeAbbreviation}".Length;
            name = $"{owner.Substring(0, Math.Min(owner.Length, 5))}{shortName.Substring(0, Math.Min(shortName.Length, shortNameLength))}{environment}{resourceTypeAbbreviation}";
        }
        return name;
    }

    private string GetKeyVaultResourceName(string resourceTypeAbbreviation)
    {
        var name = $"{owner.Substring(0, Math.Min(owner.Length, 5))}-{shortName}-{environment}-{resourceTypeAbbreviation}";
        name = Regex.Replace(name, @"[^a-zA-Z0-9-]", "");
        if (name.Length > 24)
        {
            // 24 is allowed for keyvaults, but using 23 since we add a hyphen
            var shortNameLength = 23 - $"{nameof(owner).Substring(0, Math.Min(owner.Length, 5))}-{environment}-{resourceTypeAbbreviation}".Length;
            name = $"{owner.Substring(0, Math.Min(owner.Length, 5))}-{shortName.Substring(0, Math.Min(shortName.Length, shortNameLength))}-{environment}-{resourceTypeAbbreviation}";
        }
        return name;
    }

    private string GetDefaultResourceName(string resourceTypeAbbreviation)
    {
        return $"{owner}-{shortName}-{environment}-{resourceTypeAbbreviation}";
    }

}
