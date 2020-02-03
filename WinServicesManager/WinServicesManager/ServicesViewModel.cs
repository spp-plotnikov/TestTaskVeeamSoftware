﻿using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;

namespace WinServicesManager
{
    class ServicesViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<WindowsService> Services { get; set; } = new ObservableCollection<WindowsService>();

        private readonly ServicesModel model = new ServicesModel();

        public ServicesViewModel()
        {
            StopService = new DelegateCommand<string>((name) => StopServiceInternal(name));
            StartService = new DelegateCommand<string>((name) => StartServiceInternal(name));
            StopUpdating = new DelegateCommand(() => StopUpdatingInternal());
            StartUpdating = new DelegateCommand(() => StartUpdatingInternal());

            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(UpdateCollectionOfServices);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(300);
            dispatcherTimer.Start();
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUpdating)));
        }

        private void StartUpdatingInternal()
        {
            model.StartUpdating();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUpdating)));
        }

        public void Dispose()
        {
            model.Dispose();
        }
    }
}
