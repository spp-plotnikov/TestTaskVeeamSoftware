using System;

namespace WinServicesManager
{
    public class WindowsService
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Status { get; set; }
        public string Account { get; set; }
        public bool IsStopped => Status == "Stopped";
        public bool CanBeManaged => IsStopped || Status == "Running";
    }
}
