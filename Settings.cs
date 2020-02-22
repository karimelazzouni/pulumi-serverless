using Pulumi;

static class Settings
{
    static Settings()
    {
        var config = new Config();
        ResourceGroupName = config.Require("resource-group");
        StorageAccountName = config.Require("storage-account");
        ConsumptionPlanName = config.Require("consumption-plan");
        FunctionAppName = config.Require("function-app");
        ZipsContainerName = config.Require("zips-container");
        ZipBlobName = config.Require("zip-blob");
    }

    public static string ResourceGroupName { get; }
    public static string StorageAccountName { get; }
    public static string ConsumptionPlanName { get; }
    public static string FunctionAppName { get; }
    public static string ZipsContainerName { get; }
    public static string ZipBlobName { get; }
}
