using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Management;

namespace WinServicesManager
{
    class ServicesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<WindowsService> Services { get; set; }

        public ServicesViewModel()
        {
            var services = ListAllWindowsServices();
            Services = new ObservableCollection<WindowsService>(services);
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
    }
}
