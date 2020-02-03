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

        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        static readonly object locker = new object();

        private bool shouldContinueUpdating = true;
        readonly Thread backgroundUpdater = null;

        public ServicesViewModel()
        {
            StopService = new DelegateCommand<string>((name) => StopServiceInternal(name));
            var services = ListAllWindowsServices();
            Services = new ObservableCollection<WindowsService>(services);
            backgroundUpdater = new Thread(UpdateCollection);
            backgroundUpdater.IsBackground = true;
            backgroundUpdater.Start();
        }

        private void UpdateCollection()
        {
            while (shouldContinueUpdating)
            {
                var updatedServices = ListAllWindowsServices().ToList();
                dispatcher.Invoke(() =>
                {
                    lock (locker)
                    {
                        Services.Clear();
                        updatedServices.ForEach(s => Services.Add(s));
                    }
                });

                Thread.Sleep(500);
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
            shouldContinueUpdating = false;
            backgroundUpdater.Join();
        }
    }
}
