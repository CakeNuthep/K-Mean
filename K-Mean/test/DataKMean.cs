using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    class DataKMean
    {
        public double V1 { get; set; }
        public double V2 { get; set; }

        public DataKMean()
        {

        }

        public DataKMean(double v1, double v2)
        {
            V1 = v1;
            V2 = v2;
        }

        public void randomValue(int maxValue)
        {
            Random rand = new Random();
            V1 = rand.Next(maxValue);
            V2 = rand.Next(maxValue);
        }
    }
}
