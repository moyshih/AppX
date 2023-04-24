namespace AppX.Services
{
    /// <summary>
    /// App config data with default values for case there isn't an appsettings file
    /// </summary>
    public class ConfigData
    {
        public IList<string> VehicleTypes { get; set; } = new List<string> {
        "Sedan",
        "Hatchback",
        "SUV",
        "Coupe",
        "Convertible",
        "Wagon",
        "PickupTruck",
        "Electric"
    };
        public string HostName { get; set; } = "localhost";
        public string ConsumeQueueName { get; set; } = "motorcycleQueue";
        public string PublishQueueName { get; set; } = "carQueue";
        public string PublishUiQueueName { get; set; } = "motorcycleUiQueue";
        public int PublishRateInMilliseconds { get; set; } = 500;
    }
}
