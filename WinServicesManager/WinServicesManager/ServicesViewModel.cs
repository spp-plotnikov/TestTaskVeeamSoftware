using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WinServicesManager
{
    class ServicesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<WindowsService> Services { get; set; }

        public ServicesViewModel()
        {
            var services = ServiceController.GetServices().Select(s => new WindowsService 
            { 
                Name = s.ServiceName, 
                DisplayName = s.DisplayName, 
                Status = s.Status.ToString()
            });
            Services = new ObservableCollection<WindowsService>(services);
        }
    }
}
