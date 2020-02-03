using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Threading;

namespace WinServicesManager
{
    class ServicesModel : IDisposable
    {
        private bool shouldContinueUpdating = true;
        private Thread backgroundUpdater = null;
        private static readonly object locker = new object();
        private List<WindowsService> windowsServicesUpdated = new List<WindowsService>();

        public List<WindowsService> WindowsServices 
        { 
            get
            {
                lock (locker)
                {
                    return windowsServicesUpdated;
                }
            }
        }

        public ServicesModel()
        {
            backgroundUpdater = new Thread(UpdateServices)
            {
                IsBackground = true
            };
            backgroundUpdater.Start();
        }

        public void StopService(string name)
        {
            try
            {
                var service = new ServiceController(name);
                if (service.CanStop)
                {
                    service.Stop(); // fast operation, so no need run in separate thread
                }
            }
            catch
            {
                // some logging here
            }
        }

        public void StartService(string name)
        {
            try
            {
                var service = new ServiceController(name);
                service.Start(); // fast operation, so no need run in separate thread
            }
            catch
            {
                // some logging here
            }
        }

        private void UpdateServices()
        {
            while (shouldContinueUpdating)
            {
                var updatedServices = ListAllWindowsServices().ToList(); // possible long operation

                lock (locker)
                {
                    windowsServicesUpdated = updatedServices;
                }

                Thread.Sleep(300);
            }
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
