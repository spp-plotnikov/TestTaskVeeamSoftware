# WinServicesManager

This WPF-app shows a list of Windows services. You can see the following properties of each service: Name, DisplayName, Status and Account name under which a service runs. List of services is automatically updated from the OS. You can stop and resume the updating. Also you can stop or start each service.

## Ideas for Further Improvements

* Using `System.ComponentModel.BackgroundWorker` instead of `Thread` and getting rid of two timers (in Model and in ViewModel).
* Temporarily disabling pressed buttons to prevent errors due to repeated click (waiting until a service called to be stopped will be really stopped).