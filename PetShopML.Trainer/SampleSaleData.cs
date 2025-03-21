using PetShopML.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShopML.Trainer
{
    public static class SampleSaleData
    {
        public static SaleData[] MonthlyData { get; }

        static SampleSaleData()
        {
            MonthlyData = new SaleData[] {
                new SaleData()
                {
                    productId = 988,
                    month = 10,
                    year = 2017,
                    avg = 43,
                    max = 220,
                    min = 1,
                    count = 25,
                    prev = 1036,
                    next = 1076,
                    units = 1094
                },
                new SaleData()
                {
                    productId = 988,
                    month = 11,
                    year = 2017,
                    avg = 41,
                    max = 225,
                    min = 4,
                    count = 26,
                    prev = 1094,
                    units = 1076,
                    //next = float.NaN
                }
            };
        }
    }
}
