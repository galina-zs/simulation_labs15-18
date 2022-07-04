using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;

namespace PoissonStream
{
    class Simulator
    {
        static readonly Random random = new Random();

        public Simulator(double time, int n, double lambda1, double lambda2)
        {
            Time = time;
            N = n;
            ALambda = lambda1 + lambda2;

            AStream = new AggregatedStream(lambda1, lambda2);
        }

        double Time { get; set; }
        int N { get; set; }
        double ALambda { get; set; }
        AggregatedStream AStream { get; set; }

        public double Average { get; private set; }
        public double AverageError { get; private set; }
        public double Variance { get; private set; }
        public double VarianceError { get; private set; }

        public double ChiSquare { get; private set; }
        public bool ChiSquareTest { get; private set; }
        public double CriticalValue { get; private set; }

        public double[] Probabilities { get; private set; }
        public double[] TProbs { get; private set; }

        class Stream
        {
            public Stream(double lambda)
            {
                Lambda = lambda;

                Frequency = new Dictionary<int, int>();
            }

            double Lambda { get; set; }
            IDictionary<int, int> Frequency { get; set; }

            public int Process(double time)
            {
                double a = 0;
                int n = 0;

                while(true)
                {
                    a += -Math.Log(random.NextDouble()) / Lambda;
                    if (a > time)
                    {
                        break;
                    }
                    n++;
                }

                if (!Frequency.ContainsKey(n))
                {
                    Frequency[n] = 0;
                }
                Frequency[n]++;

                return n;
            }
        }

        class AggregatedStream
        {
            public AggregatedStream(double lambda1, double lambda2)
            {
                Stream1 = new Stream(lambda1);
                Stream2 = new Stream(lambda2);

                Frequency = new Dictionary<int, int>();
            }

            Stream Stream1 { get; set; }
            Stream Stream2 { get; set; }
            IDictionary<int, int> Frequency { get; set; }

            public void Process(double time)
            {
                var n1 = Stream1.Process(time);
                var n2 = Stream2.Process(time);

                if (!Frequency.ContainsKey(n1 + n2)) 
                {
                    Frequency[n1 + n2] = 0;
                }
                Frequency[n1 + n2]++;
            }

            public double[] GetFrequency()
            {
                double[] res = new double[Frequency.Last().Key+1];
                for (int i = 0; i < res.Length; i++)
                {
                    res[i] = Frequency.ContainsKey(i) ? Frequency[i] : 0;
                }

                return res;
            }
        }

        public void Start()
        {
            for (int i = 0; i < N; i++)
            {
                AStream.Process(Time);
            }

            var frequency = AStream.GetFrequency();
            Probabilities = EmpiricalProbs(frequency, N);
            TProbs = TheoreticalProbs(ALambda, Time, Probabilities.Length);

            Average = 0;
            double E = 0;
            for (int i = 0; i < Probabilities.Length; i++)
            {
                Average += Probabilities[i] * i;
                E += TProbs[i] * i;
            }
            AverageError = Math.Abs((Average - E) / E);

            Variance = 0;
            double D = 0;
            for (int i = 0; i < Probabilities.Length; i++)
            {
                Variance += Probabilities[i] * i * i;
                D += TProbs[i] * i * i;
            }
            Variance -= Average * Average;
            D -= E * E;
            VarianceError = Math.Abs((Variance - D) / D);

            ChiSquare = 0;
            for (int i = 0; i < Probabilities.Length; i++)
            {
                ChiSquare += (frequency[i] - N * TProbs[i]) * (frequency[i] - N * TProbs[i]) / (N * TProbs[i]);
            }

            if (Probabilities.Length > 1)
            {
                CriticalValue = ChiSquared.InvCDF(Probabilities.Length - 1, 1 - 0.05);
                if (ChiSquare <= CriticalValue) ChiSquareTest = true;
                else ChiSquareTest = false;
            }
        }

        private double[] EmpiricalProbs(double[] frequency, int N)
        {
            double[] probs = new double[frequency.Length];
            for (int i = 0; i < probs.Length; i++)
            {
                probs[i] = frequency[i] / N;
            }

            return probs;
        }

        private double[] TheoreticalProbs(double lambda, double T, int n)
        {
            double[] probs = new double[n];
            for (int i = 0; i < n; i++)
            {
                probs[i] = Math.Pow(lambda * T, i) / Factorial(i) * Math.Exp(-lambda*T);
            }

            return probs;
        }

        static int Factorial(int n)
        {
            return (n == 1 || n == 0) ? 1 : n * Factorial(n - 1);
        }
    }
}
