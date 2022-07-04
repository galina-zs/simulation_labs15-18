using System;
using System.Windows.Forms;

namespace AgentModeling
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
            simulator = new Simulator(15, 7, 4);

            var probs = simulator.GetTheoretical();
            chart1.Series[1].Points.Clear();
            for (int i = 0; i < probs.Length; i++)
            {
                chart1.Series[1].Points.AddXY(i, probs[i]);
            }

            timer1.Start();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            simulator.Process();

            listBox1.Items.Clear();
            for(int i = 0; i < simulator.Agents.Length; i++)
            {
                listBox1.Items.Add(i + " " + (simulator.Agents[i].IsBusy ? "Занят       " : "Свободен") + " Времени осталось " + simulator.Agents[i].WorkTime);
            }

            var probs = simulator.GetEmpirical();
            chart1.Series[0].Points.Clear();
            for (int i = 0; i < probs.Length; i++)
            {
                chart1.Series[0].Points.AddXY(i, probs[i]);
            }
        }
    }
}
