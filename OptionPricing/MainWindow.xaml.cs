using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OptionPricing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public object dataGridView1 { get; private set; }

        /* double assetPrice = 0;
double volatility = 0;
double yearsToExpiry = 0;
double interestRate = 0;
*/

        public MainWindow()
        {
            InitializeComponent();
        }


        private void calculateButton_Click(object sender, RoutedEventArgs e)
        {
            Calculate();
        }
        private void CalculateOptionPrices()
        {
            ModelParams modelParams = GetModelParams();
        }
        private void Calculate()
        {
            double assetPrice = double.Parse(assetPriceTextBox.Text);
            double volatility = double.Parse(volatilityTextBox.Text);
            double yearsToExpiry = double.Parse(yearsToExpiryTextBox.Text);
            double interestRate = double.Parse(interestRateTextBox.Text);
            int numberOfSteps = int.Parse(numberOfStepsTextBox.Text);
            int numberOfSims = int.Parse(numberOfSimsTextBox.Text);
            Random rand = new Random();
            double discountFactor = Math.Exp(-interestRate * yearsToExpiry);


            double step = yearsToExpiry / numberOfSteps;
            String finalMessage = "";

            Debug.Print("Current Price: " + assetPrice);
            Debug.Print("Now calculating prices for " + numberOfSims + " simulations, each one having " + numberOfSteps + " steps.");


            /* Set Up Average Option Price Arrays */
            //double[] monteCarloCallPayoffs = new double[numberOfSims];
            //double[] monteCarloPutPayoffs = new double[numberOfSims];
            //double[] aritFixedCallPayoffs = new double[numberOfSims];
            //double[] aritFixedPutPayoffs = new double[numberOfSims];
            //double[] geoFixedCallPayoffs = new double[numberOfSims];
            //double[] geoFixedPutPayoffs = new double[numberOfSims];
            //double[] aritFloatCallPayoffs = new double[numberOfSims];
            //double[] aritFloatPutPayoffs = new double[numberOfSims];
            //double[] geoFloatCallPayoffs = new double[numberOfSims];
            //double[] geoFloatPutPayoffs = new double[numberOfSims];

            List<double> monteCarloCallPayoffs = new List<double>();
            List<double> monteCarloPutPayoffs = new List<double>();
            List<double> aritFixedCallPayoffs = new List<double>();
            List<double> aritFixedPutPayoffs = new List<double>();
            List<double> geoFixedCallPayoffs = new List<double>();
            List<double> geoFixedPutPayoffs = new List<double>();
            List<double> aritFloatCallPayoffs = new List<double>();
            List<double> aritFloatPutPayoffs = new List<double>();
            List<double> geoFloatCallPayoffs = new List<double>();
            List<double> geoFloatPutPayoffs = new List<double>();

            /* Loop runs for each simulation */
            for (int i = 0; i < numberOfSims; i++)
            {
                finalMessage += "\n Simulation " + (i + 1) + ": \n \n";
                assetPrice = double.Parse(assetPriceTextBox.Text);
                var assetPriceList = new ObservableCollection<KeyValuePair<int, double>>();
                assetPriceList.Add(new KeyValuePair<int, double>(0, assetPrice));

                /* Loop runs for each step of each simulation */
                for (int p = 1; p <= numberOfSteps; p++)
                {
                    double standardNormalCDFInverse = getStandardNormalCDFInverse(rand);
                    assetPrice = assetPrice * (1 + interestRate * step + volatility * standardNormalCDFInverse * Math.Sqrt(step));
                    // finalMessage += "Price at step " + p + ": " + assetPrice + "\n";
                    // assetPriceList.Add(new KeyValuePair<int, double>(p, assetPrice));

                    if (i < 100)
                    {
                        assetPriceList.Add(new KeyValuePair<int, double>(p, assetPrice));
                    }
                }
                finalMessage += "Price at final step = " + assetPrice + "\n";
                //UpdateSimulationDisplay(i, assetPriceList, numberOfSteps);

                /* Calculates Arithmetic Average */
                double aritAverage = 0;

                for (int k = 0; k < assetPriceList.Count; k++)
                {
                    aritAverage += assetPriceList[k].Value;
                }

                aritAverage = aritAverage / assetPriceList.Count;

                Debug.Print("aritAverage: " + aritAverage);

                /* Calculates Geometric Average */
                double geoAverage = 0;

                for (int k = 0; k < assetPriceList.Count; k++)
                {
                    geoAverage += Math.Log(assetPriceList[k].Value);
                }

                geoAverage = Math.Exp(geoAverage / assetPriceList.Count);

                Debug.Print("geoAverage: " + geoAverage);

               // UpdateSimulationDisplay(i, assetPriceList, numberOfSteps, aritAverage, geoAverage);
               
                /* Option Payoffs */
                double finalRealization = assetPriceList[numberOfSteps].Value;

                double monteCarloCallPayoff = Math.Max(finalRealization - double.Parse(option2StrikeWeightTextBox.Text), 0);
                double monteCarloPutPayoff = Math.Max(double.Parse(option2StrikeWeightTextBox.Text) - finalRealization, 0);
                double aritFixedCallPayoff = Math.Max(aritAverage - double.Parse(option3StrikeWeightTextBox.Text), 0);
                double aritFixedPutPayoff = Math.Max(double.Parse(option2StrikeWeightTextBox.Text) - aritAverage, 0);
                double geoFixedCallPayoff = Math.Max(geoAverage - double.Parse(option2StrikeWeightTextBox.Text), 0);
                double geoFixedPutPayoff = Math.Max(double.Parse(option2StrikeWeightTextBox.Text) - geoAverage, 0);
                double aritFloatCallPayoff = Math.Max(finalRealization - double.Parse(option5StrikeWeightTextBox.Text) * aritAverage, 0);
                double aritFloatPutPayoff = Math.Max(double.Parse(option5StrikeWeightTextBox.Text) * aritAverage - finalRealization, 0);
                double geoFloatCallPayoff = Math.Max(finalRealization - double.Parse(option6StrikeWeightTextBox.Text) * geoAverage, 0);
                double geoFloatPutPayoff = Math.Max(double.Parse(option6StrikeWeightTextBox.Text) * geoAverage - finalRealization, 0);

                UpdateSimulationDisplay(i, assetPriceList, numberOfSteps, aritAverage, geoAverage, monteCarloCallPayoff, monteCarloPutPayoff, aritFixedCallPayoff, aritFixedPutPayoff,
                    geoFixedCallPayoff, geoFixedPutPayoff, aritFloatCallPayoff, aritFloatPutPayoff, geoFloatCallPayoff, geoFloatPutPayoff);

                /* Add Option Payoffs to Arrays */

                monteCarloCallPayoffs.Add(monteCarloCallPayoff);
                monteCarloPutPayoffs.Add(monteCarloPutPayoff);
                aritFixedCallPayoffs.Add(aritFixedCallPayoff);
                aritFixedPutPayoffs.Add(aritFixedPutPayoff);
                geoFixedCallPayoffs.Add(geoFixedCallPayoff);
                geoFixedPutPayoffs.Add (geoFixedPutPayoff);
                aritFloatCallPayoffs.Add(aritFloatCallPayoff);
                aritFloatPutPayoffs.Add(aritFloatPutPayoff);
                geoFloatCallPayoffs.Add(geoFloatCallPayoff);
                geoFloatPutPayoffs.Add(geoFloatPutPayoff);

                /* Set Up Average Option Price Arrays */
                // double[] callPayoffs = {monteCarloCallPayoff, aritFixedCallPayoff, geoFixedCallPayoff, aritFloatCallPayoff, geoFloatCallPayoff};
                // double[] putPayoffs = { monteCarloPutPayoff, aritFixedPutPayoff, geoFixedPutPayoff, aritFloatPutPayoff, geoFloatPutPayoff };


                /* Black Scholes Price */
                DateTime startTime = DateTime.Now;
                double timeStep = yearsToExpiry / numberOfSteps;

                for (int q = 0; q < numberOfSims; q++)
                {
                    double strikePV = int.Parse(option2StrikeWeightTextBox.Text);

                    double d1 = (Math.Log(assetPrice / strikePV) + (interestRate +
                        Math.Pow(volatility, 2) / 2) * yearsToExpiry) / (volatility * Math.Sqrt(yearsToExpiry));
                    double d2 = d1 - volatility * Math.Sqrt(yearsToExpiry);
                    // double d1cdf = Normal.CDF(0, 1, d1);
                    // double d2cdf = Normal.CDF(0, 1, d2);
                    //
                    // double bsPrice = d1cdf * assetPrice - d2cdf * strikePV;
                    
                
                }

            }

            /* Loop Ends */

            /* Calculate Payoff Averages */

            double monteCarloCallPayoffAverage = monteCarloCallPayoffs.Average() * discountFactor;
            double monteCarloPutPayoffAverage = monteCarloPutPayoffs.Average() * discountFactor;
            double aritFixedCallPayoffAverage = aritFixedCallPayoffs.Average() * discountFactor;
            double aritFixedPutPayoffAverage = aritFixedPutPayoffs.Average() * discountFactor;
            double geoFixedCallPayoffAverage = geoFixedCallPayoffs.Average() * discountFactor;
            double geoFixedPutPayoffAverage = geoFixedPutPayoffs.Average() * discountFactor;
            double aritFloatCallPayoffAverage = aritFloatCallPayoffs.Average() * discountFactor;
            double aritFloatPutPayoffAverage = aritFixedPutPayoffs.Average() * discountFactor;
            double geoFloatCallPayoffAverage = geoFloatCallPayoffs.Average() * discountFactor;
            double geoFloatPutPayoffAverage = geoFloatPutPayoffs.Average() * discountFactor;

            Debug.Print(finalMessage);

            /* Add Payoff Averages to GUI */
            option2CallPriceTextBox.Text = monteCarloCallPayoffAverage.ToString();
            option2PutPriceTextBox.Text =  monteCarloPutPayoffAverage.ToString();
            option3CallPriceTextBox.Text = aritFixedCallPayoffAverage.ToString();
            option3PutPriceTextBox.Text = aritFixedPutPayoffAverage.ToString();
            option4CallPriceTextBox.Text = geoFixedCallPayoffAverage.ToString();
            option4PutPriceTextBox.Text = geoFixedPutPayoffAverage.ToString();
            option5CallPriceTextBox.Text = aritFloatCallPayoffAverage.ToString();
            option5PutPriceTextBox.Text = aritFloatPutPayoffAverage.ToString();
            option6CallPriceTextBox.Text = geoFloatCallPayoffAverage.ToString();
            option6PutPriceTextBox.Text = geoFloatPutPayoffAverage.ToString();
        }

        private void UpdateSimulationDisplay(int i, ObservableCollection<KeyValuePair<int, double>> assetPriceList, int numberOfSteps, double aritAverage, double geoAverage,
           double monteCarloCallPayoff, double monteCarloPutPayoff, double aritFixedCallPayoff, double aritFixedPutPayoff, double geoFixedCallPayoff,
           double geoFixedPutPayoff, double aritFloatCallPayoff, double aritFloatPutPayoff, double geoFloatCallPayoff, double geoFloatPutPayoff)
        {
            if (valuationDataGrid.Columns.Count == 0)
            {
                SetValuationGridColumns(i, numberOfSteps, assetPriceList);
            }

            SetValuationGridRows(i, assetPriceList, numberOfSteps, aritAverage, geoAverage, monteCarloCallPayoff, monteCarloPutPayoff, aritFixedCallPayoff, aritFixedPutPayoff,
                    geoFixedCallPayoff, geoFixedPutPayoff, aritFloatCallPayoff, aritFloatPutPayoff, geoFloatCallPayoff, geoFloatPutPayoff);

            AddAssetPathToGraph(assetPriceList);

        }

        private void SetValuationGridRows(int i, ObservableCollection<KeyValuePair<int, double>> assetPriceList, int numberOfSteps, double aritAverage, double geoAverage,
           double monteCarloCallPayoff, double monteCarloPutPayoff, double aritFixedCallPayoff, double aritFixedPutPayoff, double geoFixedCallPayoff,
           double geoFixedPutPayoff, double aritFloatCallPayoff, double aritFloatPutPayoff, double geoFloatCallPayoff, double geoFloatPutPayoff)
        {
            dynamic row = new ExpandoObject();
            ((IDictionary<string, Object>)row)["Sim"] = i + 1;

            for (int j = 0; j <= numberOfSteps; j++)
            {
                ((IDictionary<string, Object>)row)["Step" + j.ToString()] = assetPriceList[j].Value;
            }

            ((IDictionary<string, Object>)row)["AritAverage"] = aritAverage;
            ((IDictionary<string, Object>)row)["GeoAverage"] = geoAverage;
            ((IDictionary<string, Object>)row)["EuroMCCall"] = monteCarloCallPayoff;
            ((IDictionary<string, Object>)row)["EuroMCPut"] = monteCarloPutPayoff;
            ((IDictionary<string, Object>)row)["AritFixedCall"] = aritFixedCallPayoff;
            ((IDictionary<string, Object>)row)["AritFixedPut"] = aritFixedPutPayoff;
            ((IDictionary<string, Object>)row)["GeoFixedCall"] = geoFixedCallPayoff;
            ((IDictionary<string, Object>)row)["GeoFixedPut"] = geoFixedPutPayoff;
            ((IDictionary<string, Object>)row)["AritFloatCall"] = aritFloatCallPayoff;
            ((IDictionary<string, Object>)row)["AritFloatPut"] = aritFloatPutPayoff;
            ((IDictionary<string, Object>)row)["GeoFloatCall"] = geoFloatCallPayoff;
            ((IDictionary<string, Object>)row)["GeoFloatPut"] = geoFloatPutPayoff;

            valuationDataGrid.Items.Add(row);

        }


        private void SetValuationGridColumns(int i, int numberOfSteps, ObservableCollection<KeyValuePair<int, double>> assetPriceList)
        {
            if (this.displayAssetSimsCheckBox.IsChecked == true || this.displayOptionPayoffCheckbox.IsChecked == true)
            {
                DataGridTextColumn column = new DataGridTextColumn();
                column.Binding = new Binding("Sim");
                column.Header = "Sim";
                valuationDataGrid.Columns.Add(column);
            }

            if (this.displayAssetSimsCheckBox.IsChecked == true)
            {
                for (int j = 0; j < numberOfSteps; j++)
                {
                    string key = "Step" + j.ToString();
                    DataGridTextColumn column = new DataGridTextColumn();
                    column.Binding = new Binding(key);
                    column.Header = key;
                    valuationDataGrid.Columns.Add(column);
                }
            }


            if (this.displayAssetSimsCheckBox.IsChecked == true || this.displayOptionPayoffCheckbox.IsChecked == true)
            {
                string key = "Step" + numberOfSteps.ToString();
                DataGridTextColumn column = new DataGridTextColumn();
                column.Binding = new Binding(key);
                column.Header = key;
                valuationDataGrid.Columns.Add(column);

                column = new DataGridTextColumn();
                column.Binding = new Binding("AritAverage");
                column.Header = "AritAverage";
                valuationDataGrid.Columns.Add(column);

                column = new DataGridTextColumn();
                column.Binding = new Binding("GeoAverage");
                column.Header = "GeoAverage";
                valuationDataGrid.Columns.Add(column);

                column = new DataGridTextColumn();
                column.Binding = new Binding("EuroMCCall");
                column.Header = "EuroMCCall";
                valuationDataGrid.Columns.Add(column);

                column = new DataGridTextColumn();
                column.Binding = new Binding("EuroMCPut");
                column.Header = "EuroMCPut";
                valuationDataGrid.Columns.Add(column);

                column = new DataGridTextColumn();
                column.Binding = new Binding("AritFixedCall");
                column.Header = "AritFixedCall";
                valuationDataGrid.Columns.Add(column);

                column = new DataGridTextColumn();
                column.Binding = new Binding("AritFixedPut");
                column.Header = "AritFixedPut";
                valuationDataGrid.Columns.Add(column);

                column = new DataGridTextColumn();
                column.Binding = new Binding("GeoFixedCall");
                column.Header = "GeoFixedCall";
                valuationDataGrid.Columns.Add(column);

                column = new DataGridTextColumn();
                column.Binding = new Binding("GeoFixedCall");
                column.Header = "GeoFixedPut";
                valuationDataGrid.Columns.Add(column);

                column = new DataGridTextColumn();
                column.Binding = new Binding("AritFloatCall");
                column.Header = "AritFloatCall";
                valuationDataGrid.Columns.Add(column);

                column = new DataGridTextColumn();
                column.Binding = new Binding("AritFloatPut");
                column.Header = "AritFloatPut";
                valuationDataGrid.Columns.Add(column);

                column = new DataGridTextColumn();
                column.Binding = new Binding("GeoFloatCall");
                column.Header = "GeoFloatCall";
                valuationDataGrid.Columns.Add(column);

                column = new DataGridTextColumn();
                column.Binding = new Binding("GeoFloatPut");
                column.Header = "GeoFloatPut";
                valuationDataGrid.Columns.Add(column);
            }

            //if (this.displayOptionPayoffCheckbox.IsChecked == true)
            //{
            //    for (int j = 0; j <= numberOfSteps; j++)
            //    {
            //        DataGridTextColumn column = new DataGridTextColumn();
            //        column.Binding = new Binding(assetPriceList[j].Value);
            //        column.Header = key;
            //        valuationDataGrid.Columns.Add(column);
            //    }
            //}
            //}
        }

        private void AddAssetPathToGraph(ObservableCollection<KeyValuePair<int,double>> assetPriceList)
        {
            LineSeries lineSeries = new LineSeries();
            lineSeries.DependentValuePath = "Value";
            lineSeries.IndependentValuePath = "Key";
            lineSeries.IsSelectionEnabled = false;
            lineSeries.ItemsSource = assetPriceList;
            valuationChart.Series.Add(lineSeries);
        }

        private double getStandardNormalCDFInverse(Random rand)
        {
          double standardNormalCDFInverse = (rand.NextDouble() + rand.NextDouble() + rand.NextDouble()
               + rand.NextDouble() + rand.NextDouble() + rand.NextDouble()
                  + rand.NextDouble() + rand.NextDouble() + rand.NextDouble()
                  + rand.NextDouble() + rand.NextDouble() + rand.NextDouble()) - 6.0;

            return standardNormalCDFInverse;
        }

        private GetModelParams()
        {
            double assetPrice;
            double volatility;
            double yearsToExpiry;
            double interestRate;

            double.TryParse(assetPriceTextBox.Text, out assetPrice);
            double.TryParse(volatilityTextBox.Text, out volatility);
            double.TryParse(yearsToExpiryTextBox.Text, out yearsToExpiry);
            double.TryParse(interestRateTextBox.Text, out interestRate);

            ModelParams modelParams = new ModelParams();
            modelParams.AssetPrice = assetPrice;
            modelParams.Volatility = volatility;
            modelParams.YearsToExpiry = yearsToExpiry;
            modelParams.InterestRate = interestRate;
            modelParams.AddMilsteinCorrection = (bool)
                this.addMilsteinCorrectionCheckBox.IsChecked;
        }

        private Option[] GetOptions()
        {

        }
    }
}

 