using Window = System.Windows.Window;

namespace WpfApplication1
{
    public partial class MainWindow : Window
    {
        public IntegralTemperatureVM IntegralTemperatureVM { get; }
        public DetectorsVM Detectors { get; }

        public MainWindow()
        {
            InitializeComponent();

            IntegralTemperatureVM = new IntegralTemperatureVM();
            Detectors             = new DetectorsVM();

          //  cb.SelectedIndex = 0;
            //this.NumbersDTOs = new Dictionary<string, List<NumbersDTO>>();
            //this.DataContext = this;
            //VisibilityCondition = false;
        }
    }
}
