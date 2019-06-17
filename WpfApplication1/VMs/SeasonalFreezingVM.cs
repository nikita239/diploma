using MathNet.Numerics;
using MathNet.Numerics.RootFinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using WpfApplication1.Models;

namespace WpfApplication1.VMs
{
    class SeasonalFreezingVM: BaseViewModel
    {
        private List<DataStructure> _cells;
        private bool _visibilityCondition;
        private double _z;

        public SeasonalFreezingVM()
        {
            _cells = new List<DataStructure>() {
                new DataStructure{
                    TextBlock = SeasonalFreezingParameters.γ,
                    Description = "Удальный вес грунта",
                    Units = "(кг/м³)"
                },
                new DataStructure{
                    TextBlock = SeasonalFreezingParameters.P,
                    TextBlock2 = "уп",
                    Description = "Коэффициент удельного пучения",
                    Units = "(Н/см²)"
                },
                new DataStructure{
                    TextBlock = SeasonalFreezingParameters.R,
                    TextBlock2 = "i",
                    Description = "Расчетное сопротивление грунта сдвигу",
                    Units = "(Н/см²)",
                },
                new DataStructure{
                    TextBlock = SeasonalFreezingParameters.f,
                    Description = "Глубина промерзания",
                    Units = "(м)",
                },
                new DataStructure{
                    TextBlock = SeasonalFreezingParameters.r,
                    TextBlock2 = "як",
                    Description = "глубина закладки",
                    Units = "(м)",
                }
            };
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

        public ICommand ButtonClickCommand
        {
            get { return new DelegateCommand(Calculate, true); }
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

        public double Z
        {
            get { return _z; }
            set
            {
                if (_z != value)
                {
                    _z = Math.Round(value, 2);
                    OnPropertyChanged("Z");
                }
            }
        }

        public void Calculate()
        {
            var gamma = _cells.First(x => (SeasonalFreezingParameters)x.TextBlock == SeasonalFreezingParameters.γ).Value.Value;
            var P_уп = _cells.First(x => (SeasonalFreezingParameters)x.TextBlock == SeasonalFreezingParameters.P).Value.Value;
            var R = _cells.First(x => (SeasonalFreezingParameters)x.TextBlock == SeasonalFreezingParameters.R).Value.Value;
            var f = _cells.First(x => (SeasonalFreezingParameters)x.TextBlock == SeasonalFreezingParameters.f).Value.Value;
            var r_тр = _cells.First(x => (SeasonalFreezingParameters)x.TextBlock == SeasonalFreezingParameters.r).Value.Value;
            var r_як = 0.1;

            Z = (2.4 * Math.PI * P_уп * f - (Math.PI * P_уп / f)*(r_як * r_як * (r_як / 3 - r_тр) + (f - r_як) * (r_як * r_як - r_тр * r_тр) 
                - Math.Pow(r_тр, 3) / 3 - r_тр*r_тр * f / 2) + Math.PI * gamma*(r_як * r_як - r_тр * r_тр) - 2 * Math.PI * r_тр * R * f) 
                / (r_тр * r_тр * Math.PI * gamma + 2 * Math.PI * r_тр * R);

            VisibilityCondition = true;
        }
    }
}
