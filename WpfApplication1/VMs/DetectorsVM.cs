using ExcelDataReader;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Input;
using WpfApplication1.Models;
using WpfApplication1.Properties;

namespace WpfApplication1.VMs
{
    public class DetectorsVM : BaseViewModel
    {
        private List<string>                               _models;
        private bool                          _visibilityCondition;
        private bool                         _visibilityCondition2;
        private PlotModel                               _plotModel;
        private string                              _selectedModel;
        private Dictionary<string, List<NumbersDTO>> _pointersDict;
        private double                                         _ΔH;

        public DetectorsVM()
        {
            Models = new List<string>();
            VisibilityCondition = false;
            VisibilityCondition2 = false;
            _pointersDict = new Dictionary<string, List<NumbersDTO>>();
        }

        public bool VisibilityCondition
        {
            get { return _visibilityCondition; }
            set
            {
                if (_visibilityCondition != value)
                {
                    _visibilityCondition = value;
                    OnPropertyChanged("VisibilityCondition");
                }
            }
        }

        public bool VisibilityCondition2
        {
            get { return _visibilityCondition2; }
            set
            {
                if (_visibilityCondition2 != value)
                {
                    _visibilityCondition2 = value;
                    OnPropertyChanged("VisibilityCondition2");
                }
            }
        }

        public double ΔH
        {
            get { return _ΔH; }
            set
            {
                if (_ΔH != value)
                {
                    _ΔH = value;
                    OnPropertyChanged("ΔH");
                }
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

        public List<string> Models
        {
            get { return _models; }
            set
            {
                if (_models == value) return;
                _models = value;
                OnPropertyChanged("Models");
            }
        }

        public string SelectedModel
        {
            get { return _selectedModel; }
            set
            {
                if (_selectedModel == value) return;
                _selectedModel = value;
                VisibilityCondition2 = true;
                OnPropertyChanged("SelectedModel");
                PlotModel = CreateModel();
            }
        }

        public ICommand ButtonClickCommand
        {
            get { return new DelegateCommand(LoadFromXml, true); }
        }

        private void LoadFromXml()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm",
                RestoreDirectory = true
            };

            var headers = new List<string>();
            _pointersDict = new Dictionary<string, List<NumbersDTO>>();

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
                            _pointersDict.Add(currentNumber, list);
                            list = new List<NumbersDTO>();
                        }
                        currentNumber = row.First().ToString();
                    }
                    NumbersDTO dto = new NumbersDTO
                    {
                        H = Convert.ToDouble(row[1]),
                        T1 = Convert.ToDouble(row[2]),
                        T2 = Convert.ToDouble(row[3])
                    };

                    list.Add(dto);

                    if (i == drows.Count - 1)
                    {
                        _pointersDict.Add(currentNumber, list);
                    }
                }
                Models.AddRange(_pointersDict.Select(x => x.Key).ToList());

                VisibilityCondition = true;

                Settings.Default.Save();
            }
        }

        private PlotModel CreateModel()
        {
            var plotModel = new PlotModel();

            var function = new LineSeries() { Color = OxyColors.Black };

            double sum = 0;
            double h_prev = 0.5;
            for(int i = 0; i < _pointersDict[SelectedModel].Count; i++)
            {
                var d = _pointersDict[SelectedModel][i];
                function.Points.Add(new DataPoint(d.H, d.T2 - d.T1));
                sum += (d.T1 - d.T2) * (d.H * h_prev);
                h_prev = d.H - h_prev;
            }

            ΔH = sum * 12 * Math.Pow(10, -6) * Math.Pow(10, 3);

            var linearAxis = new LinearAxis { Position = AxisPosition.Bottom };
            linearAxis.IsZoomEnabled = false;
            linearAxis.IsPanEnabled = false;
            linearAxis.MajorStep = 1;

            plotModel.Axes.Add(linearAxis);
            plotModel.Series.Add(function);

            return plotModel;
        }
    }
}
