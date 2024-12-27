namespace ProvidersMS.src.Providers.Application.Models
{
    public class Driver(string driverId, string craneAssigned)
    {
        public string Id { get; set; } = driverId;
        public string CraneAssigned { get; set; } = craneAssigned;
    }
}
