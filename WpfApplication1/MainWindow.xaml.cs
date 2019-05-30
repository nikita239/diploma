using WpfApplication1.VMs;
using Window = System.Windows.Window;

namespace WpfApplication1.Views
{
    public partial class MainWindow : Window
    {
        public IntegralTemperatureVM IntegralTemperatureVM { get; }
        public DetectorsVM Detectors { get; }
        public LevelingParametersVM LevelingParametersVM { get; }

        public MainWindow()
        {
            InitializeComponent();

            IntegralTemperatureVM = new IntegralTemperatureVM();
            Detectors             = new DetectorsVM();
            LevelingParametersVM  = new LevelingParametersVM();
        }
    }
}
