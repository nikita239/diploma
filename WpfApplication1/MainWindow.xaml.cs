using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ExcelDataReader;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using WpfApplication1.Properties;
using Window = System.Windows.Window;

namespace WpfApplication1
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public class NumbersDTO
        {
            public double H { get; set; }
            public double T1 { get; set; }
            public double T2 { get; set; }
        }
        public MainWindow()
        {
            InitializeComponent();
            cb.SelectedIndex = 0;
            this.NumbersDTOs = new Dictionary<string, List<NumbersDTO>>();
            this.DataContext = this;
            this.plot.Model = this.CreateModel();

            VisibilityCondition = false;
        }

        public Dictionary<string, List<NumbersDTO>> NumbersDTOs { get; set; }

        public List<string> Titles { get; set; } = new List<string>();

        private PlotModel CreateModel()
        {
            Dictionary<double, string> monthValueMap = new Dictionary<double, string>();
            var months = new List<string> { "янв", "фев", "мар", "апр", "май", "июн", "июл", "авг", "сен", "окт", "ноя", "дек" };
            var plotModel = new PlotModel();
            double i = 0;
            int month = 0;
            while (month < 12)
            {
                LineAnnotation Line = new LineAnnotation
                {
                    Color = OxyColors.Black,
                    Type = LineAnnotationType.Vertical,
                    X = i,
                    ToolTip = i.ToString(),
                    LineStyle = LineStyle.LongDash
                };
                monthValueMap.Add(Math.Round(i, 4), months[month]);
                i += 31556926 / 12;

                plotModel.Annotations.Add(Line);
                month++;
            }

            var linearAxis = new LinearAxis { Position = AxisPosition.Bottom, AbsoluteMinimum = 0, AbsoluteMaximum = 31556926, TicklineColor = OxyColors.White };
            linearAxis.IsZoomEnabled = false;
            linearAxis.IsPanEnabled = false;
            linearAxis.MajorStep = 31556926 / 12;
            linearAxis.LabelFormatter = (d) =>
            {
                return monthValueMap.TryGetValue(Math.Round(d, 4), out string s) ? s : "янв";
            };

            plotModel.Axes.Add(linearAxis);
            var function = new FunctionSeries(MakeFunction(), 0, 31556926, 500, "");
            function.Color = OxyColors.Black;
            plotModel.Series.Add(function);

            return plotModel;
        }


        private Func<double, double> MakeFunction()
        {
            double alpha_st = 12 * Math.Pow(10, -6);
            double alpha_gp = 28 * Math.Pow(10, -6);
            double a = 26 * Math.Pow(10, -7);
            double z = 20;
            double delta_T = 31556926;
            double delta_t0 = 50;
            return t => delta_t0 * 100 * Math.Sqrt(a * delta_T/  2 * Math.PI) * ((alpha_st - alpha_gp) * Math.Exp(-z * Math.Sqrt(Math.PI / ( delta_T* a)))
            * Math.Sin(2*Math.PI * t/delta_T  - z * Math.Sqrt(Math.PI / (delta_T * a)) - Math.PI / 4) - alpha_st * Math.Sin(2 * Math.PI * t / delta_T - Math.PI / 4));
        }
        private PlotModel CreateModel2()
        {
            var plotModel = new PlotModel();
            var selectedItem = combobox22.SelectedItem.ToString();
            var data = NumbersDTOs[selectedItem];
            var function = new LineSeries() { Color = OxyColors.Black };

            foreach (var d in data)
            {
                function.Points.Add(new DataPoint(d.H, d.T2 - d.T1));
            }

            var linearAxis = new LinearAxis { Position = AxisPosition.Bottom };
            linearAxis.IsZoomEnabled = false;
            linearAxis.IsPanEnabled = false;
            linearAxis.MajorStep = 1;

            plotModel.Axes.Add(linearAxis);
            plotModel.Series.Add(function);

            return plotModel;

        }

        private bool visibilityCondition;
        public bool VisibilityCondition
        {
            get { return visibilityCondition; }
            set
            {
                if (visibilityCondition != value)
                {
                    visibilityCondition = value;
                    OnPropertyChanged("VisibilityCondition");
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void btnOpenFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm",
                RestoreDirectory = true
            };

            var headers = new List<string>();

            ExcelDataSetConfiguration c = new ExcelDataSetConfiguration
            {
                ConfigureDataTable = (s) =>
                {
                    return new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true,

                        ReadHeaderRow = rowReader =>
                        {
                            for (var i = 0; i < rowReader.FieldCount; i++)
                                headers.Add(Convert.ToString(rowReader.GetValue(i)));
                        },
                    };
                }
            };

            List<DataRow> drows;

            if (openFileDialog.ShowDialog() == true)
            {
                using (var stream = File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        drows = reader.AsDataSet(c).Tables[0].AsEnumerable().ToList();
                    }
                }

                List<NumbersDTO> list = new List<NumbersDTO>();

                string currentNumber = String.Empty;

                for (int i = 1; i < drows.Count; i++)
                {
                    var row = drows[i].ItemArray;
                    if (row.First() != DBNull.Value)
                    {
                        if (i != 1)
                        {
                            NumbersDTOs.Add(currentNumber, list);
                            list = new List<NumbersDTO>();
                        }
                        currentNumber = row.First().ToString();
                        Titles.Add(currentNumber);
                    }
                    NumbersDTO dto = new NumbersDTO
                    {
                        H = Convert.ToSingle(row[1]),
                        T1 = Convert.ToSingle(row[2]),
                        T2 = Convert.ToSingle(row[3])
                    };

                    list.Add(dto);

                    if (i == drows.Count - 1)
                    {
                        NumbersDTOs.Add(currentNumber, list);
                    }
                }

                VisibilityCondition = true;

                Settings.Default.Save();
            }
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            plot2.Model = CreateModel2();
        }
    }
}
