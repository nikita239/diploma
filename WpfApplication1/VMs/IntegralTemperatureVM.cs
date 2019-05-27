using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WpfApplication1
{
    public class ModelItem
    {
        public int    Id   { get; set; }
        public string Name { get; set; }
    }

    public class IntegralTemperatureVM : BaseViewModel
    {
        #region Private Members

        private PlotModel _plotModel;
        private string    _solidParameter;
        private double    _alpha;
        private double    _a;
        private double    _delta;

        #endregion

        public IntegralTemperatureVM()
        {
            A = 26;
            Delta = 50;
           // Alpha = 24;
            _plotModel = CreateModel();
            SolidParameter = SolidParameters.First().Name;
        }

        #region Properties

        public ObservableCollection<ModelItem> SolidParameters => new ObservableCollection<ModelItem>(new List<ModelItem> {
                new ModelItem { Id = 1, Name = "Песок" },
                new ModelItem { Id = 1, Name = "Глина" },
                new ModelItem { Id = 1, Name = "Суглинок" }
            });

        public double Alpha
        {
            get { return _alpha; }
            set
            {
                if (_alpha == value) return;
                _alpha = value;
                OnPropertyChanged("Alpha");
                PlotModel = CreateModel();
            }
        }

        public double A
        {
            get { return _a; }
            set
            {
                if (_a == value) return;
                _a = value;
                OnPropertyChanged("A");
                PlotModel = CreateModel();
            }
        }

        public double Delta
        {
            get { return _delta; }
            set
            {
                if (_delta == value) return;
                _delta = value;
                OnPropertyChanged("Delta");
                PlotModel = CreateModel();
            }
        }

        public string SolidParameter
        {
            get { return _solidParameter; }
            set
            {
                if (_solidParameter == value) return;
                _solidParameter = value;
                OnPropertyChanged("SolidParameter");
                PlotModel.InvalidatePlot(true);
            }
        }

        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set
            {
                if (_plotModel == value) return;
                _plotModel = value;
                OnPropertyChanged("PlotModel");
            }
        }

        #endregion


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
            double a = A * Math.Pow(10, -7);
            double alpha_st = 12 * Math.Pow(10, -6); //const
            double z = 20;
            double delta_T = 31556926;
            return t => Delta * 100 * Math.Sqrt(a * delta_T / 2 * Math.PI) * ((alpha_st - Alpha) * Math.Exp(-z * Math.Sqrt(Math.PI / (delta_T * a)))
            * Math.Sin(2 * Math.PI * t / delta_T - z * Math.Sqrt(Math.PI / (delta_T * a)) - Math.PI / 4) - alpha_st * Math.Sin(2 * Math.PI * t / delta_T - Math.PI / 4));
        }
    }
}
