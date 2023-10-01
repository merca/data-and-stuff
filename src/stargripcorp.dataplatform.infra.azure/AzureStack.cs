using Pulumi;
using stargripcorp.dataplatform.infra.azure.Infrastructure;
using stargripcorp.dataplatform.infra.azure.Resources;
using stargripcorp.dataplatform.infra.utils.Naming;
using stargripcorp.dataplatform.infra.utils.Stack;

namespace stargripcorp.dataplatform.infra.azure;

internal class AzureStack : Stack
{
    private static Config Config => new();
    private static string CloudProvider => "azure";
    [Output] public Output<string>? Readme { get; set; } = null;
    public AzureStack()
    {
        var config = new StackConfig(
                       Owner: Config.Require("owner"),
                       Environment: Config.Require("environment"));

        Readme = Output.Create(File.ReadAllText("../readme.md"));

        var tags = new Tags(new Dictionary<string, string>
    {
        {"owner", config.Owner},
        {"environment", config.Environment},
        {"cloudProvider", CloudProvider},
        {"stack", "data-platform"},
        {"project", "stargripcorp"},
        {"contact", "merca.ovnerud@proton.me"},
        {"created", DateTime.UtcNow.ToString("yyyy-MM-dd") },
        {"createdBy", "Pulumi"},
        {"is_automated", "true" }
    });

        new CoreComponents(config,
            new NamingConvention(
                    owner: config.Owner,
                    shortName: "core",
                    environment: config.Environment,
                    cloudProvider: CloudProvider
                ), tags: tags.Std_Tags).Run();
        new DataPlatform(config,
            new NamingConvention(
                owner: config.Owner,
                shortName: "data",
                environment: config.Environment,
                cloudProvider: CloudProvider
                ), tags: tags.Std_Tags).Run();
    }

}