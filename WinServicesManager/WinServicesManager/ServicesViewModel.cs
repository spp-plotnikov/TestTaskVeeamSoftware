using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Threading;
using System.Windows.Threading;

namespace WinServicesManager
{
    class ServicesViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<WindowsService> Services { get; set; }

        List<WindowsService> WindowsServicesUpdated = null;

        static readonly object locker = new object();
        bool shouldContinueUpdate = true;
        Thread backgroundUpdater = null;

        SynchronizationContext synchronizationContext = SynchronizationContext.Current;

        public ServicesViewModel()
        {
            StopService = new DelegateCommand<string>((name) => StopServiceInternal(name));
            //var services = ListAllWindowsServices();
            WindowsServicesUpdated = new List<WindowsService>();
            Services = new ObservableCollection<WindowsService>();
            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(300);
            dispatcherTimer.Start();

            backgroundUpdater = new Thread(UpdateCollection)
            {
                IsBackground = true
            };
            backgroundUpdater.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            lock (locker)
            {
                Services.Clear();
                WindowsServicesUpdated.ForEach(s => Services.Add(s));
            }
        }

        private void UpdateCollection()
        {
            while (shouldContinueUpdate)
            {
                var updatedServices = ListAllWindowsServices().ToList();

                lock (locker)
                {
                    WindowsServicesUpdated = updatedServices;
                }

                Thread.Sleep(300);
            }
        }

        public DelegateCommand<string> StopService { get; }

        public void StopServiceInternal(string name)
        {
            var a = Thread.CurrentThread;
        }

        private static IEnumerable<WindowsService> ListAllWindowsServices()
        {
            ManagementObjectSearcher windowsServicesSearcher = new ManagementObjectSearcher("root\\cimv2", "select * from Win32_Service");
            ManagementObjectCollection objectCollection = windowsServicesSearcher.Get();


            foreach (ManagementObject windowsService in objectCollection)
            {
                PropertyDataCollection serviceProperties = windowsService.Properties;
                var newService = new WindowsService();
                foreach (PropertyData serviceProperty in serviceProperties)
                {
                    if (serviceProperty.Name == "Name")
                    {
                        newService.Name = serviceProperty.Value?.ToString();
                    }

                    if (serviceProperty.Name == "DisplayName")
                    {
                        newService.DisplayName = serviceProperty.Value?.ToString();
                    }

                    if (serviceProperty.Name == "State")
                    {
                        newService.Status = serviceProperty.Value?.ToString();
                    }

                    if (serviceProperty.Name == "StartName")
                    {
                        newService.Account = serviceProperty.Value?.ToString();
                    }
                }
                yield return newService;
            }
        }

        public void Dispose()
        {
            shouldContinueUpdate = false;
            backgroundUpdater.Join();
        }
    }
}
