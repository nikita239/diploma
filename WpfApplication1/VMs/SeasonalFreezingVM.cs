using MathNet.Numerics;
using MathNet.Numerics.RootFinding;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Input;
using WpfApplication1.Models;

namespace WpfApplication1.VMs
{
    class SeasonalFreezingVM: BaseViewModel
    {
        private List<DataStructure> _cells;

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
                    TextBlock = SeasonalFreezingParameters.z,
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

        public void Calculate()
        {
            double P_up = 11;
            double f = 1.2;
            double r_tr = 0.1;
            double gamma = 2750;
            double z = 5;
            double R_i = 0.147;

            var x_3 = -2 * Math.PI * P_up / (3 * f);
            var x_2 = ((Math.PI * P_up) / f) * (-r_tr + f) + Math.PI * gamma;
            var x_1 = (Math.PI * P_up / f) * r_tr * r_tr;
            var x_0 = -2.4 * Math.PI * P_up * r_tr * f - Math.PI * P_up * (Math.Pow(r_tr, 2) + Math.Pow(r_tr, 3)/(3*f) + Math.Pow(r_tr, 2) / 2) 
                - Math.PI * z * Math.Pow(r_tr, 2) * gamma + Math.Pow(r_tr, 2) * Math.PI * z*gamma + 2* Math.PI*r_tr*R_i*(z-f);

            var roots = SolveCubic(x_3, x_2, x_1, x_0);

            //Complex root1 = roots.Item1;
            //Complex root2 = roots.Item2;
            //Complex root3 = roots.Item3;

            //var aii = root1.IsReal();
        }
        public List<Complex> SolveCubic(double a, double b, double c, double d)
        {
            const int NRoots = 3;
            double SquareRootof3 = Math.Sqrt(3);
            // the 3 cubic roots of 1
            List<Complex> CubicUnity = new List<Complex>(NRoots)
                        { new Complex(1, 0), new Complex(-0.5, -SquareRootof3 / 2.0), new Complex(-0.5, SquareRootof3 / 2.0) };
            // intermediate calculations
            double DELTA = 18 * a * b * c * d - 4 * b * b * b * d + b * b * c * c - 4 * a * c * c * c - 27 * a * a * d * d;
            double DELTA0 = b * b - 3 * a * c;
            double DELTA1 = 2 * b * b * b - 9 * a * b * c + 27 * a * a * d;
            Complex DELTA2 = -27 * a * a * DELTA;
            Complex C = Complex.Pow((DELTA1 + Complex.Pow(DELTA2, 0.5)) / 2, 1 / 3.0); //Phew...
            List<Complex> R = new List<Complex>(NRoots);
            for (int i = 0; i < NRoots; i++)
            {
                Complex M = CubicUnity[i] * C;
                Complex Root = -1.0 / (3 * a) * (b + M + DELTA0 / M);
                R.Add(Root);
            }
            return R;
        }
    }
}
