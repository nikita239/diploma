using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using WpfApplication1.Models;

namespace WpfApplication1.VMs
{


    class PermafrostVM : BaseViewModel
    {
        private List<DataStructure> _cells;
        private double _z;
        private bool _visibilityCondition;

        public PermafrostVM()
        {
            _cells = new List<DataStructure>() {
                new DataStructure{
                    TextBlock = PermafrostParameters.f,
                    Description = "Глубина промерзания",
                    Units = "(м)",
                },
                new DataStructure{
                    TextBlock = PermafrostParameters.P,
                    TextBlock2 = "уп",
                    Description = "Коэффициент удельного пучения горной породы",
                    Units = "(Н/см²)"
                },
                new DataStructure{
                    TextBlock = PermafrostParameters.Р,
                    TextBlock2 = "усм",
                    Description = "Коэффициент удельного смерзания горной породы",
                    Units = "(Н/см²)"
                }
            };
            VisibilityCondition = false;
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

        public double Z
        {
            get { return _z; }
            set
            {
                if (_z != value)
                {
                    _z = value;
                    OnPropertyChanged("Z");
                }
            }
        }

        public ICommand ButtonClickCommand
        {
            get { return new DelegateCommand(Calculate, true); }
        }

        public void Calculate()
        {
            var f = _cells.First(x => (PermafrostParameters)x.TextBlock == PermafrostParameters.f).Value.Value;
            var P_уп = _cells.First(x => (PermafrostParameters)x.TextBlock == PermafrostParameters.P).Value.Value;
            var P_усм = _cells.First(x => (PermafrostParameters)x.TextBlock == PermafrostParameters.Р).Value.Value;

            Z = 1.4 * f * P_уп / P_усм;

            VisibilityCondition = true;
        }
    }
}
