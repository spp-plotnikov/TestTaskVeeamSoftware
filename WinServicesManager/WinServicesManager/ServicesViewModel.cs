using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
            Services = new ObservableCollection<WindowsService>
            {
                new WindowsService { Name="Test", DisplayName="Test" }
            };
        }
    }
}
