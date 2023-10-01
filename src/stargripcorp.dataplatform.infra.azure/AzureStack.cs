using Pulumi;
using stargripcorp.dataplatform.infra.azure.Infrastructure;
using stargripcorp.dataplatform.infra.utils.Naming;
using stargripcorp.dataplatform.infra.utils.Stack;

internal class AzureStack:Stack
{
    private static Config Config => new();
    private static string CloudProvider => "azure";
    [Output] public Output<string>? Readme { get; set; } = null;
    public AzureStack()
    {
        var config = new StackConfig(
                       Owner: Config.Require("owner"),
                       Environment: Config.Require("environment"));

        Readme = Output.Create(File.ReadAllText("../README.md"));

        new CoreComponents(config, 
            new NamingConvention(
                    owner: config.Owner,
                    shortName: "core",
                    environment: config.Environment,
                    cloudProvider: CloudProvider
                )).Run();
        new DataPlatform(config, 
            new NamingConvention(
                owner: config.Owner,
                shortName: "data",
                environment: config.Environment,
                cloudProvider: CloudProvider
                )).Run();
    }

}