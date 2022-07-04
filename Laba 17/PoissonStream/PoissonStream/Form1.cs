using System;
using System.Windows.Forms;

namespace PoissonStream
{
    public partial class Form1 : Form
    {
        Simulator simulator;
        public Form1()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            simulator = new Simulator(
                (double)timeValue.Value,
                (int)numberValue.Value,
                (double)lambda1Value.Value,
                (double)lambda2Value.Value
                );

            simulator.Start();

            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            for (int i = 0; i < simulator.Probabilities.Length; i++)
            {
                chart1.Series[0].Points.AddXY(i, simulator.Probabilities[i]);
                chart1.Series[1].Points.AddXY(i, simulator.TProbs[i]);
            }

            averageLabel.Text = "Average: " + simulator.Average.ToString("F3") + " (error = " + Math.Round(simulator.AverageError * 100) + "%)";
            varianceLabel.Text = "Variance: " + simulator.Variance.ToString("F3") + " (error = " + Math.Round(simulator.VarianceError * 100) + "%)";

            chiSquareLabel.Text = "ChiSquared: " + simulator.ChiSquare.ToString("F3") + " <= " + simulator.CriticalValue.ToString("F3") + " is " + simulator.ChiSquareTest;
        }
    }
}
