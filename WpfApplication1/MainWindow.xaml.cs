using System;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public PlotModel MyModel { get; private set; }

        private void btnOpenFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Excel files (*.xls)|(*.xlsx)|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                //foreach (string filename in openFileDialog.FileNames)
                   // lbFiles.Items.Add(Path.GetFileName(filename));
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            cb.SelectedIndex = 0;


            MyModel = new PlotModel { Title = "Your Equation", LegendTitle = "Equations" };
            this.DataContext = this;

            this.plot.Model = this.CreateModel(new DateTime(2010, 01, 01), new DateTime(2011, 01, 01), 1);

            int year = DateTime.Now.Year;

        }

        public Collection<DateValue> Data { get; set; }


        private PlotModel CreateModel(DateTime start, DateTime end, int increment)
        {
            var tmp = new PlotModel { Title = "DateTime axis (PlotModel)" };
            tmp.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = -Math.PI / 2, Maximum = 3 * Math.PI / 2, TextColor = OxyColors.White, TicklineColor = OxyColors.White});
            var r = new Random(13);
            var date = start;
            double i = -Math.PI / 2;
            string[] months = { "янв", "фев", "мар", "апр", "май", "июн", "июл", "авг", "сен", "окт", "ноя", "дек", "окт", "ноя", "дек" };
            int y = 0;
            while (i < 3 * Math.PI / 2 )
            {
                LineAnnotation Line = new LineAnnotation
                {
                    
                    Color = OxyColors.Black,
                    Type = LineAnnotationType.Vertical,

                    MaximumY = 1,
                    Text = months[y],
                    TextColor = OxyColors.Black,
                    X = i,
                    ToolTip = i.ToString(),
                    TextOrientation = AnnotationTextOrientation.Horizontal,
                     TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                     TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                };

                tmp.Annotations.Add(Line);

                i += Math.PI / 6;
                y++;
            }


            var categoryAxis = new CategoryAxis()
            {
                Position = AxisPosition.Bottom
           };
            tmp.Axes.Add(categoryAxis);

            tmp.Series.Add(new FunctionSeries(x => Math.Sin(2*x), -Math.PI / 2, 3 * Math.PI / 2, 0.1, "sin(x)"));

            return tmp;

        }
    }

    public class DateValue
    {
        public DateTime Date { get; set; }
        public double Value { get; set; }
    }

}
