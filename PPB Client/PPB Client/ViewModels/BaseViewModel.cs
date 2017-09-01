using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PPB_Client.ViewModels
{
    /// <summary>
    /// Base class for all view models. Implements INotifyPropertyChanged which allows the UI to be notified of property changes.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies the UI that this property needs to be updated. 
        /// </summary>
        /// <param name="propertyName">Property which has been updated.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
