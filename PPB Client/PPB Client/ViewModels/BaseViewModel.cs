using PPB_Client.Helpers;

namespace PPB_Client.ViewModels
{
    public abstract class BaseViewModel : NotifyPropertyChange
    {
        // Creates instance of the server class.
        protected static Server server = new Server();

    }
}
