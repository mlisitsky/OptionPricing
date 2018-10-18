using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptionPricing
{
    class ModelParams
    {

        public double AssetPrice { get; set; }
        public double Volatility { get; set; }
        public double YearsToExpiry { get; set; }
        public double InterestRate { get; set; }
        public bool AddMilsteinCorrection { get; set; }
    }
}
