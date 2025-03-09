using Microsoft.ML.Data;

namespace PetShopML.Model
{
    public class SalesIssue
    {
        [LoadColumn(0)]
        public string TaxNumber { get; set; } = string.Empty;

        [LoadColumn(1)]
        public string ProductId { get; set;} = string.Empty;

        //[LoadColumn(2)]
        //public string SalesDate { get; set; } = string.Empty;

    }
}
