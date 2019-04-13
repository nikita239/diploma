using System;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using WpfApplication1.Properties;
using Excel = Microsoft.Office.Interop.Excel;
using Window = System.Windows.Window;

namespace WpfApplication1
{
    public partial class MainWindow : Window
    {
        public PlotModel MyModel { get; private set; }
        public string FileName { get; set; }
        private void btnOpenFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            openFileDialog.RestoreDirectory = true;


            if (openFileDialog.ShowDialog() == true)
            {
                TableList.Clear();
                Excel.Application xlApp = new Excel.Application();
                Workbook xlWorkbook = xlApp.Workbooks.Open(openFileDialog.FileName);
                _Worksheet xlWorksheet = xlWorkbook.Sheets[1];
                Range xlRange = xlWorksheet.UsedRange;
                fileNameLabel.Content = openFileDialog.FileName;
                int rowCount = xlRange.Rows.Count;
                int colCount = xlRange.Columns.Count;

                DataObject model = new DataObject();
                Range cell;

                for (int i = 1; i <= rowCount; i++)
                {
                    for (int j = 1; j <= colCount; j++)
                    {
                        cell = (Range)xlRange.Cells[i, j];
                        //new line
                        if (j == 1)
                        {
                            if (xlRange.Cells[i, j] != null)
                            {
                                if (cell.Value != null)
                                {
                                    if (i != 1)
                                    {
                                        TableList.Add(model);
                                        model = new DataObject();
                                    }
                                    model.Letter = Convert.ToString(cell.Value);
                                }
                            }
                        }
                        else
                        {
                            cell = (Range)xlRange.Cells[i, j];
                            model.Sum += Convert.ToInt32(cell.Value);
                        }
                    }
                    if (i == rowCount)
                    {
                        TableList.Add(model);
                    }
                }

                Settings.Default.Save();
            }
        }
        public class DataObject
        {
            public string Letter { get; set; }
            public int Sum { get; set; }
        }


        private ObservableCollection<DataObject> TableList = new ObservableCollection<DataObject>();

        public MainWindow()
        {
            InitializeComponent();
            cb.SelectedIndex = 0;

            MyModel = new PlotModel { Title = "Your Equation", LegendTitle = "Equations" };

            this.DataContext = this;

            this.plot.Model = this.CreateModel();

            this.dataGrid1.ItemsSource = TableList;

        }

        private PlotModel CreateModel()
        {
            var tmp = new PlotModel { Title = "DateTime axis (PlotModel)" };
            tmp.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = -Math.PI / 2, Maximum = 3 * Math.PI / 2, TextColor = OxyColors.White, TicklineColor = OxyColors.White });
            double i = -Math.PI / 2;
            string[] months = { "янв", "фев", "мар", "апр", "май", "июн", "июл", "авг", "сен", "окт", "ноя", "дек", "окт", "ноя", "дек" };
            int y = 0;
            while (i < 3 * Math.PI / 2)
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

            tmp.Series.Add(new FunctionSeries(x => Math.Sin(2 * x), -Math.PI / 2, 3 * Math.PI / 2, 0.1, "sin(x)"));

            return tmp;

        }
    }
}
