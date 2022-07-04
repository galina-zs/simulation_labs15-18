using System;
using System.Linq;

namespace AgentModeling
{
    class Simulator
    {
        static readonly Random random = new Random();
        public Simulator(double lambda, double mu, int agentsNum)
        {
            Queue = 0;
            CustomerDelay = lambda;
            AgentDelay = mu;
            Agents = new Agent[agentsNum];
            for (int i = 0; i < Agents.Length; i++)
            {
                Agents[i] = new Agent();
            }

            NextCustomerTime = 0;
            Time = 0;

            Number = 0;
            Frequency = new double[Agents.Length + 1];
        }
        static int Queue { get; set; }
        static double CustomerDelay { get; set; }
        static double AgentDelay { get; set; }
        public Agent[] Agents { get; set; }
        double NextCustomerTime { get; set; }
        double Time { get; set; }

        static int Number { get; set; }
        double[] Frequency { get; set; }

        public class Agent
        {
            public Agent()
            {
                IsBusy = false;
                WorkTime = 0;
            }

            public double WorkTime { get; set; }
            public bool IsBusy { get; set; }

            public void ProcessEvent()
            {
                IsBusy = false;
                if (Queue > 0)
                {
                    IsBusy = true;
                    WorkTime = Math.Ceiling(ExpRV(AgentDelay));

                    Queue--;
                }
            }
        }

        public void Process()
        {
            CustomersGenerator();

            double[] times = new double[Agents.Length];
            for (int i = 0; i < times.Length; i++)
            {
                times[i] = Agents[i].WorkTime;
            }

            double minT = double.MaxValue;
            int ind = -1;
            for (int i = 0; i < times.Length; i++)
            {
                if (times[i] < minT)
                {
                    minT = times[i];
                    ind = i;
                }
            }
            
            Agents[ind].ProcessEvent();

            foreach (var a in Agents)
            {
                a.WorkTime -= minT;
                if (a.WorkTime == 0)
                {
                    a.ProcessEvent();
                }
            }

            NextCustomerTime -= minT > 0 ? minT : 1;
            Time += minT > 0 ? minT : 1;
        }

        static double ExpRV(double lambda)
        {
            return lambda * Math.Exp(-lambda * random.NextDouble());
        }

        void CustomersGenerator()
        {
            if (NextCustomerTime <= 0)
            {
                NextCustomerTime = Math.Ceiling(ExpRV(CustomerDelay));

                foreach (var a in Agents)
                {
                    if (!a.IsBusy)
                    {
                        a.IsBusy = true;
                        a.WorkTime = Math.Ceiling(ExpRV(AgentDelay));
                        return;
                    }
                }
                Queue++;
            }
        }

        public double[] GetTheoretical()
        {
            var ro = CustomerDelay / AgentDelay;

            var N = Agents.Length;
            double[] probs = new double[N + 1];

            for (int k = 0; k <= N; k++)
            {
                probs[0] += Math.Pow(ro, k) / Factorial(k);
            }

            probs[0] += Math.Pow(ro, N + 1) / Factorial(N) / (N - ro);
            probs[0] = Math.Pow(probs[0], -1);

            for (int k = 0; k <= N; k++)
            {
                probs[k] = Math.Pow(ro, k) / Factorial(k) * probs[0];
            }

            return probs;
        }

        public double[] GetEmpirical()
        {
            var n = Agents.Length;
            Number++;
            double[] probs = new double[n + 1];

            var c = Agents.Where(a => a.IsBusy == true).Count();
            Frequency[c]++;

            for (int i = 0; i <= n; i++)
            {
                probs[i] = Frequency[i] / Number;
            }

            return probs;
        }

        static int Factorial(int n)
        {
            return (n == 1 || n == 0) ? 1 : n * Factorial(n - 1);
        }
    }
}
