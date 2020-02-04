# WinServicesManager

This WPF-app shows a list of the Windows services. You can see the following properties of each service: Name, DisplayName, Status and Account name under which a service runs. List of services is automatically updated from the OS. You can stop and resume the updating. Also you can stop or start each service.

## Ideas for Further Improvements

Using `System.ComponentModel.BackgroundWorker` instead of `Thread` and getting rid of two timers (in Model and in ViewModel).