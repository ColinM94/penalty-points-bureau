using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PPB_Client.Helpers
{
    /// <summary>
    /// Implements INotifyPropertyChanged.
    /// </summary>
    public class NotifyPropertyChange : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
