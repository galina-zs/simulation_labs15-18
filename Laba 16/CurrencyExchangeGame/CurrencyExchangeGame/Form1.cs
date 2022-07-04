using System;
using System.Windows.Forms;

namespace CurrencyExchangeGame
{
    public partial class Form1 : Form
    {
		private int currentDay, userCurrencyAmount;
		private double currentPrice, userMoney;
		private GBM gbm;
		public Form1()
		{
			InitializeComponent();
		}

		private void Timer1_Tick(object sender, EventArgs e)
		{
			currentDay += 1;
			currentPrice = gbm.GetNextRV();

			if (currentDay > 10)
			{
				chart1.ChartAreas[0].AxisX.Minimum += 1;
			}

			chart1.Series[0].Points.AddXY(currentDay, currentPrice);
		}

		private void StartButton_Click(object sender, EventArgs e)
		{
			if (!timer1.Enabled)
			{
				chart1.Series[0].Points.Clear();
				chart1.ChartAreas[0].AxisX.Minimum = 0;
				startButton.Text = "Finish";

				userCurrencyAmount = 0;
				userMoney = 100;
				currentDay = 1;
				currentPrice = (double)initPriceValue.Value;

				gbm = new GBM(currentPrice, (double)muValue.Value, (double)sigmaValue.Value);

				ShowMoney();

				chart1.Series[0].Points.AddXY(currentDay, currentPrice);

				buyButton.Enabled = true;
				sellButton.Enabled = true;

				timer1.Start();
			}
			else
			{
				timer1.Stop();

				buyButton.Enabled = false;
				sellButton.Enabled = false;

				startButton.Text = "Start";
			}
		}

		private void BuyButton_Click(object sender, EventArgs e)
		{
			if (userMoney >= currentPrice)
			{
				userMoney -= currentPrice;
				userCurrencyAmount += 1;

				ShowMoney();
			}
		}


        private void SellButton_Click(object sender, EventArgs e)
		{
			if (userCurrencyAmount > 0)
			{
				userMoney += currentPrice;
				userCurrencyAmount -= 1;

				ShowMoney();
			}
		}

		private void ShowMoney()
		{
			countLabel.Text = "Amount of currency: " + userCurrencyAmount;
			moneyLabel.Text = "Money: " + userMoney.ToString("0.0000");
		}
	}
}
