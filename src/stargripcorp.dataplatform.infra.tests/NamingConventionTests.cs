using FluentAssertions;
using stargripcorp.dataplatform.infra.utils.Naming;

namespace stargripcorp.dataplatform.infra.tests;

public class NamingConventionTests
{
    private readonly NamingConvention azNamingConvention;
    private readonly NamingConvention azLongNamingConvention;

    public NamingConventionTests()
    {
        azNamingConvention = new NamingConvention("myco", "myapp", "dev", "azure");
        azLongNamingConvention = new NamingConvention("myco", "myappwithaverylongname", "dev", "azure");
    }
    [Fact]
    public void GetResourceName_StorageAccount_ReturnsCorrectName()
    {
        // Arrange
        var resourceType = "azure-native:storage:StorageAccount";

        // Act
        var resourceName = azNamingConvention.GetResourceName(resourceType);

        // Assert
        resourceName.Should().Be("mycomyappdevsa");
    }

    [Fact]
    public void GetResourceName_Vault_ReturnsCorrectName()
    {
        // Arrange
        var resourceType = "azure-native:keyvault:Vault";

        // Act
        var resourceName = azNamingConvention.GetResourceName(resourceType);

        // Assert
        resourceName.Should().Be("myco-myapp-dev-kv");
    }

    [Fact]
    public void GetResourceName_UnknownResourceType_ThrowsArgumentException()
    {
        // Arrange
        var resourceType = "unknown-resource-type";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => azNamingConvention.GetResourceName(resourceType))
            .Message.Should().Be($"Unknown resource type: {resourceType}");
    }

    [Fact]
    public void GetResourceName_StorageAccount_UsesCorrectNamingConvention()
    {
        // Arrange

        // Act
        var resourceName = azNamingConvention.GetResourceName("azure-native:storage:StorageAccount");

        // Assert
        resourceName.Should().Be("mycomyappdevsa", because: "the resource name should follow the correct naming convention");
    }
    [Fact]
    public void GetResourceName_StorageAccount_LongShortName_ReturnsCorrectName()
    {
        // Act
        var resourceName = azLongNamingConvention.GetResourceName("azure-native:storage:StorageAccount");

        // Assert
        resourceName.Should().Be("mycomyappwithaveryldevsa", because: "the resource name should follow the correct naming convention with a long short name");
        resourceName.Length.Should().Be(24, because: "the resource name should be 24 characters long");
    }

    [Fact]
    public void GetResourceName_KeyVault_LongShortName_ReturnsCorrectName()
    {
        // Act
        var resourceName = azLongNamingConvention.GetResourceName("azure-native:keyvault:Vault");

        // Assert
        resourceName.Should().Be("myco-myappwithave-dev-kv", because: "the resource name should follow the correct naming convention with a long short name");
        resourceName.Length.Should().Be(24, because: "the resource name should be 24 characters long");
    }

    [Fact]
    public void GetRecourseId_StorageAccount_ReturnsCorrectId()
    {
        // Arrange
        var resourceType = "azure-native:storage:StorageAccount";

        // Act
        var resourceIdx = azNamingConvention.GenerateResourceId(resourceType);

        // Assert
        resourceIdx.Should().Be("myco-myapp-dev-sa");
    }
    [Fact]
    public void Constructor_WithInvalidCloudProvider_ThrowsArgumentException()
    {
        // Arrange
        var owner = "myco";
        var shortName = "myapp";
        var environment = "dev";
        var cloudProvider = "invalid";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new NamingConvention(owner, shortName, environment, cloudProvider));
    }
}