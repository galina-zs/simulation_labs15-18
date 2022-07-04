using System;
using System.Collections.Generic;
using System.Linq;

namespace CurrencyExchangeGame
{
    class GBM
    {
        private static readonly Random random = new Random();

        private static readonly double dT = 0.01;
        private readonly double Mu;
        private readonly double Sigma;

        private readonly IList<double> X;
        private readonly IList<double> W;

        public GBM(double x0, double mu, double sigma)
        {
            Mu = mu;
            Sigma = sigma;

            X = new List<double> { x0 };
            W = new List<double> { 0 };
        }

        public double GetNextRV()
        {
            var wt = W.Last() + Math.Sqrt(dT) * NormalVR();
            var x = X.Last() * Math.Exp((Mu - Sigma * Sigma / 2) * dT + Sigma * wt);
            W.Add(wt);
            X.Add(x);
            return x;
        }

        double NormalVR()
        {
            double rv = 0;
            for (int j = 0; j < 12; j++)
            {
                rv += random.NextDouble();
            }
            rv -= 6;
            rv += (Math.Pow(rv, 3) - 3 * rv) / 240;

            return rv;
        }
    }
}
