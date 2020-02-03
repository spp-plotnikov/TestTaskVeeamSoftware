using System;
using System.Collections.Generic;

namespace WinServicesManager
{
    /// <summary>
    /// Provides the ability to get a list of Windows services
    /// </summary>
    public interface IWinServicesProvider
    {
        IEnumerable<WindowsService> ListAllWindowsServices();
    }
}
