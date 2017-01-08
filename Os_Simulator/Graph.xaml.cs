using Os_Simulator.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Threading;
using Windows.UI.Xaml;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Os_Simulator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Graph : Page
    {
        private OperatingSystem _os;

        private int _memoryUsage;
        private DispatcherTimer dispatcherTimer;
        private List<Tuple<int, int>> _avgList = new List<Tuple<int, int>>();
        private IEnumerable<Tuple<int, int>> avgList;

        public Graph()
        {
            this.InitializeComponent();
            this.Loaded += Graph_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this._os = (OperatingSystem)e.Parameter;
          
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.dispatcherTimer.Stop();
        }

        private void Graph_Loaded(object sender, RoutedEventArgs e)
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler<object>(RefreshGraph);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();



            List<Tuple<string, int>> myList = new List<Tuple<string, int>>()
            {
                new Tuple<string, int>("Current Memory Usage", this._os.Scheduler.Memory.GetCurrentMemoryUsage()),
                new Tuple<string, int>("Unused Memory", this._os.Settings.MemorySize - this._os.Scheduler.Memory.GetCurrentMemoryUsage())

            };
            
            (MemoryChart.Series[0] as BarSeries).ItemsSource = myList;
        }

        private void RefreshGraph(object sender, object e)
        {

            List<Tuple<string, int>> memList = new List<Tuple<string, int>>()
            {
                new Tuple<string, int>("Current Memory Usage", this._os.Scheduler.Memory.GetCurrentMemoryUsage()),
                new Tuple<string, int>("Unused Memory", this._os.Settings.MemorySize - this._os.Scheduler.Memory.GetCurrentMemoryUsage())

            };

            (MemoryChart.Series[0] as BarSeries).ItemsSource = memList;

            //for (var i = 0; i < 10; i++)
            //{
            //    _avgList.Add(new Tuple<int, int>(i, i));
            //}

            _avgList.Add(new Tuple<int, int>(this._os.totalCycles, (int)this._os.GetAvgWaitTimes()));

            _avgList = _avgList.Distinct().ToList();

            




            avgList = _avgList.AsEnumerable();

            (AverageJobChart.Series[0] as LineSeries).ItemsSource = avgList;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            List<Tuple<string, int>> myList = new List<Tuple<string, int>>()
            {
                new Tuple<string, int>("Current Memory Usage", this._os.Scheduler.Memory.GetCurrentMemoryUsage()),
                new Tuple<string, int>("Unused Memory", this._os.Settings.MemorySize - this._os.Scheduler.Memory.GetCurrentMemoryUsage())

            };

            (MemoryChart.Series[0] as BarSeries).ItemsSource = myList;
        }
    }
}
