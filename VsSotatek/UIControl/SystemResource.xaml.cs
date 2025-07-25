using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Management;

namespace VsSotatek.UIControl
{
    /// <summary>
    /// Interaction logic for SystemResource.xaml
    /// </summary>
    public partial class SystemResource : UserControl, INotifyPropertyChanged
    {
        private PerformanceInfo performanceInfo;
        private DispatcherTimer timer;

        public SystemResource()
        {
            InitializeComponent();
            DataContext = this;
        }


        private float diskCPercentage;
        public float DiskCPercentage
        {
            get { return diskCPercentage; }
            set
            {
                if (diskCPercentage != value)
                {
                    diskCPercentage = value;
                    OnPropertyChanged(nameof(diskCPercentage));
                }
            }
        }


        private float cpuPercentage;
        public float CpuPercentage
        {
            get { return cpuPercentage; }
            set
            {
                if (cpuPercentage != value)
                {
                    cpuPercentage = value;
                    OnPropertyChanged(nameof(cpuPercentage));
                }
            }
        }

        private float ramPercentage;
        public float RamPercentage
        {
            get { return ramPercentage; }
            set
            {
                if (ramPercentage != value)
                {
                    ramPercentage = value;
                    OnPropertyChanged(nameof(ramPercentage));
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            performanceInfo = new PerformanceInfo();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            RefreshPerformanceInfo();
        }

        private async void RefreshPerformanceInfo()
        {
            await Task.Run(() =>
            {
                performanceInfo.Refresh();
            });

            Dispatcher.Invoke(() =>
            {
                DiskCPercentage = performanceInfo.DiskCUsagePercentage;
                CpuPercentage = performanceInfo.CPUUsagePercentage;
                RamPercentage = performanceInfo.RAMUsagePercentage;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private class PerformanceInfo
        {
            public float DiskCUsagePercentage { get; private set; }
            public float CPUUsagePercentage { get; private set; }
            public float RAMUsagePercentage { get; private set; }

            public void Refresh()
            {
                DriveInfo driveCInfo = new DriveInfo("C");
                float usedDiskCSpace = driveCInfo.TotalSize - driveCInfo.TotalFreeSpace;
                DiskCUsagePercentage = (usedDiskCSpace / driveCInfo.TotalSize) * 100;

                // CPU 
                using (PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
                {
                    cpuCounter.NextValue();
                    System.Threading.Thread.Sleep(1000);
                    CPUUsagePercentage = cpuCounter.NextValue();
                }

                // RAM
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        float totalPhysicalMemory = Convert.ToSingle(obj["TotalVisibleMemorySize"]);
                        float freePhysicalMemory = Convert.ToSingle(obj["FreePhysicalMemory"]);
                        RAMUsagePercentage = ((totalPhysicalMemory - freePhysicalMemory) / totalPhysicalMemory) * 100;
                        break;
                    }
                }
            }
        }

       
    }
}
