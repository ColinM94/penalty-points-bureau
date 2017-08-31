using PPB_Client.Helpers;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PPB_Client.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Function is called when a property changes and the UI needs to be notified.
        /// </summary>
        /// <param name="propertyName">Property which has been updated.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
