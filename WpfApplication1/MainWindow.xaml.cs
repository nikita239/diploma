using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ExcelDataReader;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using WpfApplication1.Properties;
using Window = System.Windows.Window;

namespace WpfApplication1
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public IntegralTemperatureVM IntegralTemperatureVM { get; }
        public DetectorsVM Detectors { get; }


        public event PropertyChangedEventHandler PropertyChanged;


        public MainWindow()
        {
            InitializeComponent();
            IntegralTemperatureVM = new IntegralTemperatureVM();
            Detectors = new DetectorsVM();
          //  cb.SelectedIndex = 0;
            //this.NumbersDTOs = new Dictionary<string, List<NumbersDTO>>();
            //this.DataContext = this;
            //VisibilityCondition = false;
        }

    }
}
