using Pulumi;
using stargripcorp.dataplatform.infra.azure;

return await Deployment.RunAsync<AzureStack>().ConfigureAwait(false);
