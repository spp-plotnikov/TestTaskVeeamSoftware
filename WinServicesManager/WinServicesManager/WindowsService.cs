using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
