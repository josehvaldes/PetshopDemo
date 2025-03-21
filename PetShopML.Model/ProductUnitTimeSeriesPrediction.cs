using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShopML.Model
{
    public class ProductUnitTimeSeriesPrediction
    {
        public float[] ForecastedProductUnits { get; set; }

        public float[] ConfidenceLowerBound { get; set; }

        public float[] ConfidenceUpperBound { get; set; }
    }
}
