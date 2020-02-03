using System;
using System.Collections.Generic;

namespace WinServicesManager
{
    public interface IWinServicesProvider
    {
        IEnumerable<WindowsService> ListAllWindowsServices();
    }
}
