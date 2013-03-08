using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Modeling.LabTwo.WpfGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        private readonly IList<String> distributionNames = new Collection<string>
            {
                "Uniform",
                "Gaussian",
                "Triangular",
                "Simpson",
                "Gamma",
                "Exponential"
            };


        private Double aParam;

        private Double bParam;

        private Double meanParam;

        private Double sigmaParam;

        private Double lambdaParam;

        private Int32 etaParam;


        /// <summary>
        /// 
        /// </summary>
        private Int32 selectedIndex;

        /// <summary>
        /// 
        /// </summary>
        private readonly Distribution distribution;

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.DistributionBox.ItemsSource = distributionNames;
            distribution = new Distribution();
            selectedIndex = 0;
        }


        /// <summary> 
        /// Disable all parameter inputs
        /// </summary>
        private void DisableAllInputs()
        {
            this.AValue.IsEnabled = false;
            this.BValue.IsEnabled = false;
            this.SigmaValue.IsEnabled = false;
            this.LambdaValue.IsEnabled = false;
            this.EtaValue.IsEnabled = false;
            this.MeanValue.IsEnabled = false;
            
        }

        /// <summary>
        /// Enable appropriate inputs while selected distribution changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DistributionBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedIndex = distributionNames.IndexOf(this.DistributionBox.SelectedItem.ToString());
            DisableAllInputs();
            switch (selectedIndex)
            {
                case 0:
                case 2:
                case 3:
                    this.AValue.IsEnabled = true;
                    this.BValue.IsEnabled = true;
                    break;

                case 1:
                    this.MeanValue.IsEnabled = true;
                    this.SigmaValue.IsEnabled = true;
                    break;

                case 4:
                    this.LambdaValue.IsEnabled = true;
                    this.EtaValue.IsEnabled = true;
                    break;

                case 5:
                    this.LambdaValue.IsEnabled = true;
                    break;

                default:
                    throw new IndexOutOfRangeException("Invalid selection index");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Go_OnClick(object sender, RoutedEventArgs e)
        {
            GetInputValues();
            UpdateView();
        }



        private void UpdateView()
        {
            ICollection<Double> realization;
            switch (selectedIndex)
            {
                case 0:
                    realization = distribution.Uniform(aParam, bParam);
                    break;
                case 1:
                    realization = distribution.Gaussian(meanParam, sigmaParam);
                    break;
                case 2:
                    realization = distribution.Triangle(aParam, bParam);
                    break;
                case 3:
                    realization = distribution.Simpson(aParam, bParam);
                    break;
                case 4:
                    realization = distribution.Gamma(lambdaParam, etaParam);
                    break;
                case 5:
                    realization = distribution.Exponential(lambdaParam);
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid selection index");
            }
            UpdateStatisticsResult(realization);
            DrawHistogram(realization);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="realization"></param>
        private void UpdateStatisticsResult(ICollection<Double> realization)
        {
            StatisticsResults.Calculate(realization);
            this.StatisticsDeviationValue.Text = StatisticsResults.Deviation.ToString(CultureInfo.InvariantCulture);
            this.StatisticsMeanValue.Text = StatisticsResults.ExpectedValue.ToString(CultureInfo.InvariantCulture);
            this.StatisticsSigmaValue.Text = StatisticsResults.Variance.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetInputValues()
        {
            try
            {
                this.aParam = Double.Parse(AValue.Text);
                this.bParam = Double.Parse(BValue.Text);
                this.lambdaParam = Double.Parse(LambdaValue.Text);
                this.meanParam = Double.Parse(MeanValue.Text);
                this.sigmaParam = Double.Parse(SigmaValue.Text);
                this.etaParam = (int) UInt32.Parse(EtaValue.Text);
            }
            catch (InvalidCastException e)
            {
                MessageBox.Show(this, e.Message);    
            }
            catch (FormatException e)
            {
                MessageBox.Show(this, e.Message);    
            }
            catch (OverflowException e)
            {
                MessageBox.Show(this, e.Message);    
            }
            
        }


        private void DrawHistogram(ICollection<Double> realization )
        {
            IEnumerable<double> rows = CalculateHistogramData(realization);

            const double targetProbabilityValue = (double)1 / 20;
            
            PlotModel tempPlotModel = new PlotModel("Histogram")
            {
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.RightTop,
                LegendOrientation = LegendOrientation.Vertical
            };

            ColumnSeries columnSeries = (ColumnSeries)CreateHistogramSeries(rows);
            tempPlotModel.Axes.Add(new LinearAxis(AxisPosition.Left, 0.0));
            tempPlotModel.Axes.Add(new CategoryAxis
            {
                LabelField = "Value",
                IntervalLength = targetProbabilityValue,
                ItemsSource = columnSeries.ItemsSource,
                GapWidth = 0.0
            });

            tempPlotModel.Series.Add(columnSeries);
            HistogramPlot.Model = tempPlotModel; // OxyPlot refreshing hack

        }

        
        private Series CreateHistogramSeries(IEnumerable<Double> rows)
        {
            ColumnSeries columnSeries = new ColumnSeries
            {
                IsStacked = false,
                BaseValue = 0,
                ColumnWidth = 0.05,
                ValueField = "Value",
                ItemsSource = rows.ToList().ConvertAll(x => new ColumnItem
                {
                    Value = x
                })
            };

            return columnSeries;
        }


        private static IEnumerable<double> CalculateHistogramData(ICollection<Double> realization )
        {
            ICollection<Double> result = new List<double>();

            const Int32 intervalCount = 20;
            Double maxVlaue = realization.Max();
            Double minValue = realization.Min();
            Double intervalLength = (maxVlaue - minValue) / intervalCount;

            Double from = minValue;
            Double to = minValue + intervalLength;

            for (int i = 0; i < 20; ++i)
            {
                Int32 hits = realization.Count(r => r > from && r <= to );
                Double probability = (Double)hits / realization.Count;

                result.Add(probability);

                from += intervalLength;
                to += intervalLength;
            }

            return result;
        }
    }
}
