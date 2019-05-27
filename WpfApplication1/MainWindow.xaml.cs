using Window = System.Windows.Window;

namespace WpfApplication1
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
          //  cb.SelectedIndex = 0;
            //this.NumbersDTOs = new Dictionary<string, List<NumbersDTO>>();
            //this.DataContext = this;
            //VisibilityCondition = false;
        }
    }
}
