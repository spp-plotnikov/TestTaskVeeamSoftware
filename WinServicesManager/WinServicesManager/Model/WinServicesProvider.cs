using System;
using System.Collections.Generic;
using System.Management;

namespace WinServicesManager
{
    /// <summary>
    /// Provides services using WMI
    /// </summary>
    class WinServicesProvider : IWinServicesProvider
    {
        private const string Scope = "root\\cimv2";
        private const string AllServicesQuery = "select * from Win32_Service";

        public IEnumerable<WindowsService> ListAllWindowsServices()
        {
            ManagementObjectSearcher windowsServicesSearcher = new ManagementObjectSearcher(Scope, AllServicesQuery);
            ManagementObjectCollection objectCollection = windowsServicesSearcher.Get();

            // https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-baseservice#members
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
