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
        public PlotModel MyModel { get; private set; }
        public string FileName { get; set; }
        private bool IsInitialized;
        public event PropertyChangedEventHandler PropertyChanged;

        private void btnOpenFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            openFileDialog.RestoreDirectory = true;

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
                        Numbers2.Add(currentNumber);
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
                        NumbersDTOs.Add( currentNumber, list);
                    }
                }

                Vis =  true;

                Settings.Default.Save();
            }
        }
        public class NumbersDTO
        {
            public double H { get; set; }
            public double T1 { get; set; }
            public double T2 { get; set; }
        }
        public static bool IsEnabled2 { get; set; } = false;//=  new Prop<bool> { Value = false };

        public MainWindow()
        {
            InitializeComponent();
            cb.SelectedIndex = 0;
            this.NumbersDTOs = new Dictionary<string, List<NumbersDTO>>();
            this.DataContext = this;
            this.Numbers2 = new ObservableCollection<string>() { };
            this.plot.Model = this.CreateModel();

            Vis = false;
        }

        public Dictionary<string, List<NumbersDTO>> NumbersDTOs { get; set; }

        public ObservableCollection<string> Numbers2 { get; set; }

        private PlotModel CreateModel()
        {
            Dictionary<double, string> dd = new Dictionary<double, string>();
            var a = new List<string> { "янв", "фев", "мар", "апр", "май", "июн", "июл", "авг", "сен", "окт", "ноя", "дек" };
            var tmp = new PlotModel();
            double i = -Math.PI / 2;
            int y = 0;
            while (y < 12)
            {
                LineAnnotation Line = new LineAnnotation
                {
                    Color = OxyColors.Black,
                    Type = LineAnnotationType.Vertical,
                    X = i,
                    ToolTip = i.ToString(),
                    LineStyle = LineStyle.LongDash
                };
                dd.Add(Math.Round(i, 4), a[y]);
                i += Math.PI / 6;

                tmp.Annotations.Add(Line);
                y++;
            }

            var linearAxis = new LinearAxis { Position = AxisPosition.Bottom, AbsoluteMinimum = -Math.PI / 2, AbsoluteMaximum = 3 * Math.PI / 2, TicklineColor = OxyColors.White };
            linearAxis.IsZoomEnabled = false;
            linearAxis.IsPanEnabled = false;
            linearAxis.MajorStep = Math.PI / 6;
            linearAxis.LabelFormatter = (d) =>
            {
                return dd.TryGetValue(Math.Round(d, 4), out string s) ? s : "янв";
            };

            tmp.Axes.Add(linearAxis);


            var function = new FunctionSeries(x => Math.Sin(x), -Math.PI / 2, 3 * Math.PI / 2, 0.1, "");
            function.Color = OxyColors.Black;
            tmp.Series.Add(function);

            return tmp;

        }


        private PlotModel CreateModel2()
        {

            var tmp = new PlotModel();
            var a = combobox22.SelectedItem.ToString();
            var data = NumbersDTOs[a];
            var function = new LineSeries();
            foreach (var d in data)
            {
                function.Points.Add(new DataPoint(d.H, d.T2 - d.T1));

            }
            function.Color = OxyColors.Black;
            var linearAxis = new LinearAxis { Position = AxisPosition.Bottom };
            linearAxis.IsZoomEnabled = false;
            linearAxis.IsPanEnabled = false;
            linearAxis.MajorStep = 1;

            tmp.Axes.Add(linearAxis);
            tmp.Series.Add(function);

            return tmp;

        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            plot2.Model = CreateModel2();
        }

        private bool vis;
        public bool Vis
        {
            get { return vis; }
            set
            {
                if (vis != value)
                {
                    vis = value;
                    OnPropertyChanged("Vis");  // To notify when the property is changed
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
