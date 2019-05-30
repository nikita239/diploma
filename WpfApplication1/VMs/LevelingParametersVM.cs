using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace WpfApplication1.VMs
{
    public class DataStructure
    {
        public string TextBlock { get; set; }
        public string TextBlock2 { get; set; }
        public double TextBox { get; set; }
        public string Description { get; set; }
    }

    public class LevelingParametersVM : BaseViewModel
    {
        private List<string> _parameters;
        private string _selectedParameter;
        private bool _visibilityCondition;
        private List<DataStructure> _cells;
        private List<DataStructure> _allValues;
        private string _displayString;

        public LevelingParametersVM()
        {
            Parameters = new List<string> { "L", "T", "M", "V" };
            _allValues = new List<DataStructure>() {
                new DataStructure{
                    TextBlock = "L",
                    Description = "Протяженность линии нивелирования"
                },
                new DataStructure{
                    TextBlock = "T",
                    Description = "Периодичность нивелирования"
                },
                new DataStructure{
                    TextBlock = "M",
                    Description = "Средняя квадратичная ошибка нивелирования на 1 км хода"
                },
                new DataStructure{
                    TextBlock = "V",
                    TextBlock2 = "прогн",
                    Description = "Прогнозная скорость"
                }
            };
            Cells = new List<DataStructure>();
        }

        public List<string> Parameters
        {
            get { return _parameters; }
            set
            {
                if (_parameters == value) return;
                _parameters = value;
                OnPropertyChanged("Parameters");
            }
        }

        public string SelectedParameter
        {
            get { return _selectedParameter; }
            set
            {
                if (_selectedParameter == value) return;
                _selectedParameter = value;
                var item = _allValues.Where(x => x.TextBlock == value);
               // if(item !)
                Cells = _allValues.Except(item).ToList();
                OnPropertyChanged("SelectedParameter");
            }
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

        public string DisplayString
        {
            get { return _displayString; }
            set
            {
                if (_displayString != value)
                {
                    _displayString = value;
                    OnPropertyChanged("DisplayString");
                }
            }
        }


        public ICommand ButtonClickCommand
        {
            get { return new DelegateCommand(FormatDisplayString, true); }
        }



        private void FormatDisplayString()
        {
            var l = _cells.FirstOrDefault(x => x.TextBlock == "L")?.TextBox;
            var m = _cells.FirstOrDefault(x => x.TextBlock == "M")?.TextBox;
            var v = _cells.FirstOrDefault(x => x.TextBlock == "V")?.TextBox;
            var t = _cells.FirstOrDefault(x => x.TextBlock == "T")?.TextBox;
            double? value = 0;
            switch (_selectedParameter)
            {
                case "L":
                    {
                        value = t * t * v * v / (32 * m * m);
                        DisplayString = String.Concat("L ≤ ", Math.Round(value.Value, 1));
                        break;
                    }
                case "M":
                    {
                        value = t * (v / Math.Sqrt(32 * l.Value));
                        DisplayString = String.Concat("M ≤ ", Math.Round(value.Value, 1));
                        break;
                    }
                case "V":
                    {
                        value = m * Math.Sqrt(32 * l.Value) / t;
                        DisplayString = String.Concat("V ≥ ", Math.Round(value.Value, 1));
                        break;
                    }
                case "T":
                    {
                        value = m * Math.Sqrt(32 * l.Value) / v;
                        DisplayString = String.Concat("T ≥ ", Math.Round(value.Value, 1 ));
                        break;
                    }
            }
        }
    }
}
