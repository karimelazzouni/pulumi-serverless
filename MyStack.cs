using Pulumi;
using Pulumi.Serialization;
using Azure = Pulumi.Azure;

class MyStack : Stack
{
    public MyStack()
    {
        var resourceGroup = new Azure.Core.ResourceGroup(Settings.ResourceGroupName);

        var storageAccount = new Azure.Storage.Account(Settings.StorageAccountName, new Azure.Storage.AccountArgs
        {
            ResourceGroupName = resourceGroup.Name,
            AccountReplicationType = "LRS",
            AccountTier = "Standard"
        });

        var plan = new Azure.AppService.Plan(Settings.ConsumptionPlanName, new Azure.AppService.PlanArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Kind = "FunctionApp",
            Sku = new Azure.AppService.Inputs.PlanSkuArgs
            {
                Tier = "Dynamic",
                Size = "Y1"
            }
        });

        var container = new Azure.Storage.Container(Settings.ZipsContainerName, new Azure.Storage.ContainerArgs 
        {
            StorageAccountName = storageAccount.Name,
            ContainerAccessType = "private"
        });

        var blob = new Azure.Storage.ZipBlob(Settings.ZipBlobName, new Azure.Storage.ZipBlobArgs 
        {
            StorageAccountName = storageAccount.Name,
            StorageContainerName = container.Name,
            Type = "block",
            Content = new FileArchive("./functions")
        });

        var codeBlobUrl = Azure.Storage.SharedAccessSignature.SignedBlobReadUrl(blob, storageAccount);

        var app = new Azure.AppService.FunctionApp(Settings.FunctionAppName, new Azure.AppService.FunctionAppArgs
        {
            ResourceGroupName = resourceGroup.Name,
            AppServicePlanId = plan.Id,
            StorageConnectionString = storageAccount.PrimaryConnectionString,
            Version = "~2",
            AppSettings =
            {
                { "FUNCTIONS_WORKER_RUNTIME", "node" },
                { "WEBSITE_NODE_DEFAULT_VERSION", "10.14.1" },
                { "WEBSITE_RUN_FROM_PACKAGE", codeBlobUrl }
            }
        });

        this.Endpoint = Output.Format($"https://{app.DefaultHostname}/api/hello");
    }

    [Output]
    public Output<string> Endpoint { get; set; }
}
