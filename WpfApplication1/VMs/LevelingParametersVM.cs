using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WpfApplication1
{
    public class LevelingParametersVM: BaseViewModel
    {
        private List<string> _parameters;
        private string       _selectedParameter;
        private bool         _visibilityCondition;


        public LevelingParametersVM()
        {
            Parameters = new List<string> { "L", "T", "M", "V" };
            //VisibilityCondition = _selectedParameter
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
                OnPropertyChanged("SelectedParameter");
            }
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

      
    }

    public class IsVisibleToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            try
            {
                GridUnitType t = GridUnitType.Star;
                if (parameter != null)
                {
                    Enum.TryParse<GridUnitType>((string)parameter, true, out t);
                }

                if (value != null)
                {
                    bool d = (bool)value;
                    return d == false ? new GridLength(0, GridUnitType.Star) : new GridLength(1, t);
                }
                return null;
            }
            catch (Exception exp)
            {
                return null;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            return null;
        }
    }
}
