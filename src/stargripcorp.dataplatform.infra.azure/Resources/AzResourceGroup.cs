﻿using Pulumi;
using Pulumi.AzureNative.Authorization;
using stargripcorp.dataplatform.infra.utils.Naming;
using Azure = Pulumi.AzureNative;

namespace stargripcorp.dataplatform.infra.azure.Resources;

internal class AzResourceGroup : ComponentResource
{
    protected Azure.Resources.ResourceGroup ResourceGroup;
    public Output<string> ResourceGroupName;
    private static Output<GetClientConfigResult> _clientConfig => GetClientConfig.Invoke();
    private readonly NamingConvention _naming;

    public AzResourceGroup(string id, NamingConvention naming, Dictionary<string, string> _tags) : base("pkg:azure:resource_group", id)
    {
        _naming = naming;
        ResourceGroup = new Azure.Resources.ResourceGroup(_naming.GetResourceId("azure-native:resources:ResourceGroup"), new()
        {
            ResourceGroupName = _naming.GetResourceName("azure-native:resources:ResourceGroup"),
            Tags = _tags
        }, new CustomResourceOptions { Parent = this, IgnoreChanges = new List<string> { "tags.created" } });
        ResourceGroupName = ResourceGroup.Name;
    }
    public AzResourceGroup WithBudget(double budgetAmount, string[] notificationEmails)
    {
        Output<string> subscriptionId = _clientConfig.Apply(o => o.SubscriptionId);

        var budget = new Azure.Consumption.Budget(_naming.GetResourceId("azure-native:resources:Budget"), new()
        {
            Amount = budgetAmount,
            Category = "Cost",
            TimeGrain = "Monthly",
            Scope = Output.Format($"/subscriptions/{subscriptionId}"),
            TimePeriod = new Azure.Consumption.Inputs.BudgetTimePeriodArgs
            {
                StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddYears(1).ToString("yyyy-MM-ddTHH:mm:ssZ")
            },
            BudgetName = _naming.GetResourceName("azure-native:resources:Budget"),
            Filter = new Azure.Consumption.Inputs.BudgetFilterArgs
            {
                Dimensions = new Azure.Consumption.Inputs.BudgetComparisonExpressionArgs
                {
                    Name = "ResourceGroupName",
                    Operator = "In",
                    Values = new()
                {
                    $"/subscriptions/{_clientConfig.Apply(o=>o.SubscriptionId)}/resourceGroups/{ResourceGroup!.Name}"
                }
                },

            },
            Notifications =
        {
            { "Actual_GreaterThan_80_Percent", new Azure.Consumption.Inputs.NotificationArgs
            {
                ContactEmails = notificationEmails,
                Enabled = true,
                Locale = "en-us",
                Operator = "GreaterThan",
                Threshold = 80,
                ThresholdType = "Actual",
            } },
        },

        }, new CustomResourceOptions { Parent = this });
        return this;
    }
}