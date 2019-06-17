using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfApplication1.Models;

namespace WpfApplication1.VMs
{
    public class LevelingParametersVM : BaseViewModel
    {
        private List<DataStructure> _parameters;
        private DataStructure _selectedParameter;
        private List<DataStructure> _cells;
        private List<DataStructure> _allValues;
        private string _displayString;
        private string _displayString2;

        public LevelingParametersVM()
        {
            _allValues = new List<DataStructure>() {
                new DataStructure{
                    TextBlock = LevelingParameters.L,
                    Description = "Протяженность линии нивелирования",
                    Units = "(км)"
                },
                new DataStructure{
                    TextBlock = LevelingParameters.T,
                    Description = "Периодичность нивелирования",
                    Units = "(год)"
                },
                new DataStructure{
                    TextBlock = LevelingParameters.M,
                    Description = "Средняя квадратичная ошибка нивелирования на 1 км хода",
                    Units = "(мм/км)"
                },
                new DataStructure{
                    TextBlock = LevelingParameters.V,
                    TextBlock2 = "прогн",
                    Description = "Прогнозная скорость",
                    Units = "(мм в год)"
                }
            };
            Parameters = _allValues.ToList();
            Cells = new List<DataStructure>();
        }

        public List<DataStructure> Parameters
        {
            get { return _parameters; }
            set
            {
                if (_parameters == value) return;
                _parameters = value;
                OnPropertyChanged("Parameters");
            }
        }

        public DataStructure SelectedParameter
        {
            get { return _selectedParameter; }
            set
            {
                if (_selectedParameter == value) return;
                _selectedParameter = value;
                List<DataStructure> item = _allValues.Where(x => x.TextBlock == value.TextBlock).ToList();
                if ((LevelingParameters)value.TextBlock != LevelingParameters.M)
                {
                    item.AddRange(_allValues.Where(x => (LevelingParameters)x.TextBlock == LevelingParameters.M));
                }
                Cells = _allValues.Except(item).ToList();
                DisplayString = String.Empty;
                DisplayString2 = String.Empty;
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
        public string DisplayString2
        {
            get { return _displayString2; }
            set
            {
                if (_displayString2 != value)
                {
                    _displayString2 = value;
                    OnPropertyChanged("DisplayString2");
                }
            }
        }

        public ICommand ButtonClickCommand
        {
            get { return new DelegateCommand(FormatDisplayString, true); }
        }

        private void FormatDisplayString(LevelingParameters parameter, Func<double, double> func, string prefix, string postfix, ref string _displayString, ref string _displayString2)
        {
            if (parameter != LevelingParameters.M)
            {
                DisplayString = String.Concat("Нивелирование I класса (M = 1 мм/км): ", prefix, Math.Round(func(1), 1), postfix);
                DisplayString2 = String.Concat("Нивелирование II класса (M = 2 мм/км): ", prefix, Math.Round(func(2), 1), postfix);
            }
        }

        private void FormatDisplayString()
        {
            try {
                switch (_selectedParameter.TextBlock)
                {
                    case LevelingParameters.L:
                        {
                            var v = _cells.First(x => (LevelingParameters)x.TextBlock == LevelingParameters.V).Value.Value;
                            var t = _cells.First(x => (LevelingParameters)x.TextBlock == LevelingParameters.T).Value.Value;

                            Func<double, double> func = m => t * t * v * v / (32 * m * m);

                            FormatDisplayString(LevelingParameters.L, func, "L ≤ ", "км", ref _displayString, ref _displayString2);
                            break;
                        }
                    case LevelingParameters.M:
                        {
                            var v = _cells.First(x => (LevelingParameters)x.TextBlock == LevelingParameters.V).Value.Value;
                            var t = _cells.First(x => (LevelingParameters)x.TextBlock == LevelingParameters.T).Value.Value;
                            var l = _cells.First(x => (LevelingParameters)x.TextBlock == LevelingParameters.L).Value.Value;
                            var value = t * (v / Math.Sqrt(32 * l));
                            DisplayString = String.Concat("M ≤ ", String.Empty, Math.Round(value, 1));
                            break;
                        }
                    case LevelingParameters.V:
                        {
                            var l = _cells.First(x => (LevelingParameters)x.TextBlock == LevelingParameters.L).Value.Value;
                            var t = _cells.First(x => (LevelingParameters)x.TextBlock == LevelingParameters.T).Value.Value;

                            Func<double, double> func = m => m * Math.Sqrt(32 * l) / t;

                            FormatDisplayString(LevelingParameters.V, func, "V ≥ ", String.Empty, ref _displayString, ref _displayString2);
                            break;
                        }
                    case LevelingParameters.T:
                        {
                            var l = _cells.First(x => (LevelingParameters)x.TextBlock == LevelingParameters.L).Value.Value;
                            var v = _cells.First(x => (LevelingParameters)x.TextBlock == LevelingParameters.V).Value.Value;

                            Func<double, double> func = m => m * Math.Sqrt(32 * l) / v;

                            FormatDisplayString(LevelingParameters.T, func, "T ≥ ", String.Empty, ref _displayString, ref _displayString2);
                            break;
                        }
                }
            }
            catch
            {
                MessageBox.Show("Некорректные аргуметы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }
    }
}
