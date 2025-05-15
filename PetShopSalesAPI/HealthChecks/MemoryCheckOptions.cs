namespace PetShopSalesAPI.HealthChecks
{
    public class MemoryCheckOptions
    {
        public string Memorystatus { get; set; }
        public long Threshold { get; set; } = 1024L * 1024L * 1024L;
    }
}
