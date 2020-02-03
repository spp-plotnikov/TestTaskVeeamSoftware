using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;

namespace WinServicesManager
{
    class ServicesViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<WindowsService> Services { get; set; } = new ObservableCollection<WindowsService>();

        private readonly ServicesModel model = new ServicesModel(new WinServicesProvider());
        private readonly DispatcherTimer timer = new DispatcherTimer();

        public ServicesViewModel()
        {
            StopService = new DelegateCommand<string>((name) => StopServiceInternal(name));
            StartService = new DelegateCommand<string>((name) => StartServiceInternal(name));
            StopUpdating = new DelegateCommand(() => StopUpdatingInternal());
            StartUpdating = new DelegateCommand(() => StartUpdatingInternal());

            timer.Tick += new EventHandler(UpdateCollectionOfServices);
            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer.Start();
        }

        private void UpdateCollectionOfServices(object sender, EventArgs e)
        {
            var updated = model.WindowsServices;
            if (Services.Count == updated.Count)
            {
                for (int i = 0; i < updated.Count; i++)
                {
                    Services[i] = updated[i];
                }
            }
            else
            {
                Services.Clear();
                updated.ForEach(s => Services.Add(s));
            }
        }

        public DelegateCommand<string> StopService { get; }
        public DelegateCommand<string> StartService { get; }
        public DelegateCommand StopUpdating { get; }
        public DelegateCommand StartUpdating { get; }

        public bool IsUpdating => model.IsUpdating;


        private void StopServiceInternal(string name)
        {
            model.StopService(name);
        }

        private void StartServiceInternal(string name)
        {
            model.StartService(name);
        }

        private void StopUpdatingInternal()
        {
            model.StopUpdating();
            timer.Stop();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUpdating)));
        }

        private void StartUpdatingInternal()
        {
            model.StartUpdating();
            timer.Start();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUpdating)));
        }

        public void Dispose()
        {
            model.Dispose();
        }
    }
}
