﻿using Pulumi;
using Pulumi.AzureNative.Authorization;
using stargripcorp.dataplatform.infra.azure.Helpers;
using stargripcorp.dataplatform.infra.utils.Naming;
using Azure = Pulumi.AzureNative;

namespace stargripcorp.dataplatform.infra.azure.Resources
{
    internal class AzKeyVault : ComponentResource
    {
        private static Output<GetClientConfigResult> _clientConfig => GetClientConfig.Invoke();
        private readonly NamingConvention _naming;
        protected Azure.KeyVault.Vault Vault;
        private Output<string> _resourceGroupName;
        public AzKeyVault(string id, NamingConvention naming, Output<string> resourceGroupName) : base("pkg:azure:keyvault", id)
        {
            _naming = naming;
            _resourceGroupName = resourceGroupName;
            Vault = new Azure.KeyVault.Vault(_naming.GetResourceId("azure-native:keyvault:Vault"), new()
            {
                VaultName = _naming.GetResourceName("azure-native:keyvault:Vault"),
                ResourceGroupName = resourceGroupName,
                Properties = new Azure.KeyVault.Inputs.VaultPropertiesArgs
                {
                    TenantId = _clientConfig.Apply(o => o.TenantId),
                    Sku = new Azure.KeyVault.Inputs.SkuArgs
                    {
                        Name = Azure.KeyVault.SkuName.Standard,
                        Family = Azure.KeyVault.SkuFamily.A,
                    },
                    EnableRbacAuthorization = true,
                }
            }, new CustomResourceOptions { Parent = this });
        }
        public AzKeyVault WithSecret(string secretName, string secretValue)
        {
            var secret = new Azure.KeyVault.Secret(_naming.GetResourceId("azure-native:keyvault:Secret"), new()
            {
                VaultName = Vault.Name,
                ResourceGroupName = _resourceGroupName,
                SecretName = secretName,
                Properties = new Azure.KeyVault.Inputs.SecretPropertiesArgs
                {
                    Value = secretValue
                }
            }, new CustomResourceOptions { Parent = this });

            return this;
        }
        public AzKeyVault WithSecretsReader(string[] readerIds)
        {
            foreach (var readerId in readerIds)
            {
                var secret = new RoleAssignment(_naming.GetResourceId("azure-native:authorization:RoleAssignment"), new()
                {
                    PrincipalId = readerId,
                    RoleDefinitionId = Utils.GetRoleIdByName("Key Vault Reader").Result,
                    Scope = Vault.Id
                }, new CustomResourceOptions { Parent = this });
            }
            return this;
        }
        public AzKeyVault WithKeyVaultSecretsAdmins(Output<Dictionary<string,bool>> contributors)
        {
            object value = contributors.Apply(x =>
            {
                List<RoleAssignment> roleAssignments = new();
                foreach(var key in x.Keys)
                {
                    x.TryGetValue(key, out bool user);
                    if (user)
                        roleAssignments.Add( ForUser(key, $"{_naming.GetResourceId("azure-native:authorization:RoleAssignment")}-{key}"));
                    else
                        roleAssignments.Add( ForSp(key, $"{_naming.GetResourceId("azure-native:authorization:RoleAssignment")}-{key}"));
                }
                return roleAssignments;
            });
            
            return this;
        }
        private RoleAssignment ForUser(string objectId,string name)
        {
            return new RoleAssignment(name, new()
            {
                PrincipalId = objectId,
                RoleDefinitionId = Utils.GetRoleIdByName("Key Vault Secrets Officer").Result,
                PrincipalType = "User",
                Scope = Vault.Id
            }, new CustomResourceOptions { Parent = this });
        }
        private RoleAssignment ForSp(string objectId, string name)
        {
            return new RoleAssignment(name, new()
            {
                PrincipalId = objectId,
                RoleDefinitionId = Utils.GetRoleIdByName("Key Vault Secrets Officer").Result,
                PrincipalType = "ServicePrincipal",
                Scope = Vault.Id
            }, new CustomResourceOptions { Parent = this });
        }
    }
}
