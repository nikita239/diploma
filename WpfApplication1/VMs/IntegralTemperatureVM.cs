using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using WpfApplication1.Models;

namespace WpfApplication1.VMs
{
    public class IntegralTemperatureVM : BaseViewModel
    {
        #region Private Members

        private PlotModel _plotModel;
        private MapValues _solidParameter;
        private List<MapValues> _solidParameters;
        private List<DataStructure> _cells;

        #endregion

        public IntegralTemperatureVM()
        {
            _cells = new List<DataStructure>() {
                new DataStructure{
                    TextBlock = IntergralTemperatureParameters.α,
                    TextBlock2 = "-6",
                    Description = "Коэффициент теплового линейного расширения горной породы",
                    Units = "(1/℃)",
                },
                new DataStructure{
                    TextBlock =IntergralTemperatureParameters.a,
                    TextBlock2 = "-7",
                    Description = "Температуропроводность горной породы",
                    Units = "(м²/c)"
                },
                new DataStructure{
                    TextBlock = IntergralTemperatureParameters.Δt,
                    Description = "Амплитуда колебания температуры воздуха на поверхности",
                    Units = "(℃)"
                },
                new DataStructure{
                    TextBlock = IntergralTemperatureParameters.z,
                    Description = "Глубина заложения",
                    Units = "(м)"
                }
            };
            SolidParameters = new List<MapValues>
            {
                new MapValues
                {
                    Solid = "Песок",
                    Values = new List<Map>
                    {
                        new Map
                        {
                            Name = IntergralTemperatureParameters.a,
                            Value = 3
                        },
                        new Map
                        {
                            Name = IntergralTemperatureParameters.z,
                            Value = 4
                        },
                        new Map
                        {
                            Name = IntergralTemperatureParameters.α,
                            Value = 5
                        },
                        new Map
                        {
                            Name = IntergralTemperatureParameters.Δt,
                            Value = 6
                        }
                    }
                },
                new MapValues
                {
                    Solid = "Глина",
                    Values = new List<Map>
                    {
                        new Map
                        {
                            Name = IntergralTemperatureParameters.a,
                            Value = 13
                        },
                        new Map
                        {
                            Name = IntergralTemperatureParameters.z,
                            Value = 14
                        },
                        new Map
                        {
                            Name = IntergralTemperatureParameters.α,
                            Value = 15
                        },
                        new Map
                        {
                            Name = IntergralTemperatureParameters.Δt,
                            Value = 16
                        }
                    }
                },
                new MapValues
                {
                    Solid = "Суглинок",
                    Values = new List<Map>
                    {
                        new Map
                        {
                            Name = IntergralTemperatureParameters.a,
                            Value = 23
                        },
                        new Map
                        {
                            Name = IntergralTemperatureParameters.z,
                            Value = 24
                        },
                        new Map
                        {
                            Name = IntergralTemperatureParameters.α,
                            Value = 25
                        },
                        new Map
                        {
                            Name = IntergralTemperatureParameters.Δt,
                            Value = 26
                        }
                    }
                },
                new MapValues
                {
                    Solid = "Ручной ввод",
                    Values = new List<Map>()
                }
            };
            SolidParameter = SolidParameters.First();
            PlotModel = CreateModel();
        }

        #region Properties
        public List<MapValues> SolidParameters
        {
            get { return _solidParameters; }
            set
            {
                if (_solidParameters == value) return;
                _solidParameters = value;
               // PlotModel = CreateModel();
                OnPropertyChanged("SolidParameters");
            }
        }

        public ICommand ButtonClickCommand
        {
            get { return new DelegateCommand(()=> PlotModel = CreateModel(), true); }
        }

        public MapValues SolidParameter
        {
            get { return _solidParameter; }
            set
            {
                if (_solidParameter == value) return;
                _solidParameter = value;
                if (value.Solid == "Ручной ввод")
                {
                    Cells = Cells.Select(c => { c.Value = null; return c; }).ToList();
                }
                else
                {
                    Cells = Cells.Select(c => { c.Value = value.Values.First(x => x.Name == (IntergralTemperatureParameters)c.TextBlock).Value; return c; }).ToList();
                }
                PlotModel = CreateModel();
                OnPropertyChanged("SolidParameter");
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
            var plotModel = new PlotModel()
            {
                Title = "Деформация реперной трубы",
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.TopCenter,
                LegendOrientation = LegendOrientation.Horizontal
            };

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

            var linearAxis = new LinearAxis { Position = AxisPosition.Bottom, AbsoluteMinimum = 0, AbsoluteMaximum = 31556926, Title = "ererer", TitlePosition = 4 };
            linearAxis.IsZoomEnabled = false;
            linearAxis.IsPanEnabled = false;
            linearAxis.MajorStep = 31556926 / 12;
            linearAxis.LabelFormatter = (d) =>
            {
                return monthValueMap.TryGetValue(Math.Round(d, 4), out string s) ? s : "янв";
            };
            var arrowAnnotation = new ArrowAnnotation
            {
                StartPoint = new DataPoint(0, 0),
                EndPoint = new DataPoint(10, 10)
            };
            plotModel.Axes.Add(linearAxis);
            var function = new FunctionSeries(MakeFunction(), 0, 31556926, 500, "");
            function.Color = OxyColors.Black;
            plotModel.Series.Add(function);

            return plotModel;
        }

        public List<DataStructure> Cells
        {
            get { return _cells; }
            set
            {
                if (_cells != value)
                {
                    _cells = value;
                    OnPropertyChanged("Cells");
                }
            }
        }

        private Func<double, double> MakeFunction()
        {
            var selectedParameter = _cells;
            double a = selectedParameter.First(x => (IntergralTemperatureParameters)x.TextBlock== IntergralTemperatureParameters.a).Value.Value * Math.Pow(10, -7);
            double alpha_st = 12 * Math.Pow(10, -6); //const
            double z = selectedParameter.First(x => (IntergralTemperatureParameters)x.TextBlock == IntergralTemperatureParameters.z).Value.Value;
            double delta = selectedParameter.First(x => (IntergralTemperatureParameters)x.TextBlock == IntergralTemperatureParameters.Δt).Value.Value;
            double alpha = selectedParameter.First(x => (IntergralTemperatureParameters)x.TextBlock == IntergralTemperatureParameters.α).Value.Value;
            double delta_T = 31556926;
            return t => delta * 100 * Math.Sqrt(a * delta_T / 2 * Math.PI) * ((alpha_st - alpha) * Math.Exp(-z * Math.Sqrt(Math.PI / (delta_T * a)))
            * Math.Sin(2 * Math.PI * t / delta_T - z * Math.Sqrt(Math.PI / (delta_T * a)) - Math.PI / 4) - alpha_st * Math.Sin(2 * Math.PI * t / delta_T - Math.PI / 4));
        }
    }
}
