using Microsoft.ML.Data;

namespace PetShopML.Model
{
    public class SaleData
    {
        // The index of column in LoadColumn(int index) should be matched with the position of columns in the underlying data file.
        // The next column is used by the Regression algorithm as the Label (e.g. the value that is being predicted by the Regression model).
        [LoadColumn(9)]
        public float next;

        [LoadColumn(0)]
        public float productId;

        [LoadColumn(1)]
        public float year;

        [LoadColumn(2)]
        public float month;

        [LoadColumn(3)]
        public float units;

        [LoadColumn(4)]
        public float avg;

        [LoadColumn(5)]
        public float count;

        [LoadColumn(6)]
        public float max;

        [LoadColumn(7)]
        public float min;

        [LoadColumn(8)]
        public float prev;
    }
}
