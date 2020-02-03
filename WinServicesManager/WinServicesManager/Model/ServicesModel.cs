using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading;

namespace WinServicesManager
{
    public class ServicesModel : IDisposable
    {
        IWinServicesProvider provider = null;
        private Thread backgroundUpdater = null;
        private static readonly object locker = new object();
        private List<WindowsService> windowsServicesUpdated = new List<WindowsService>();

        public bool IsUpdating { get; private set; } = true;

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

        public ServicesModel(IWinServicesProvider winServicesProvider)
        {
            provider = winServicesProvider ?? throw new ArgumentNullException(nameof(winServicesProvider));
            StartUpdating();
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
            while (IsUpdating)
            {
                var updatedServices = provider.ListAllWindowsServices().ToList(); // possible long operation

                lock (locker)
                {
                    windowsServicesUpdated = updatedServices;
                }

                Thread.Sleep(300);
            }
        }
        
        public void StartUpdating()
        {
            IsUpdating = true;
            if (backgroundUpdater != null) return;

            backgroundUpdater = new Thread(UpdateServices)
            {
                IsBackground = true
            };
            backgroundUpdater.Start();
        }

        public void StopUpdating()
        {
            IsUpdating = false;
            backgroundUpdater?.Join();
            backgroundUpdater = null;
        }

        public void Dispose()
        {
            StopUpdating();
        }
    }
}
