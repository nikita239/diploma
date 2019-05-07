using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApplication1
{
    partial class IntegralTemperature: TabItem
    {
        public IntegralTemperature()
        {
            InitializeComponent();
        }

        public void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Focus();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }
    }
}
