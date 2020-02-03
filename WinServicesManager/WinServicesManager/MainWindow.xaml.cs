using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WinServicesManager
{
    public partial class MainWindow : Window
    {
        private IDisposable disposableViewModel = null;
        public MainWindow()
        {
            InitializeComponent();
            disposableViewModel = new ServicesViewModel();
            DataContext = disposableViewModel;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            disposableViewModel.Dispose();
            disposableViewModel = null;
        }
    }
}
