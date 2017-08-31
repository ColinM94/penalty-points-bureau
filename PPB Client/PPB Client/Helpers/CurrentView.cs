using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PPB_Client.Helpers
{
    /// <summary>
    /// Class keeps track and allows changing of currently selected main view. 
    /// </summary>
    public static class CurrentView
    {
        // Event notifies of currently selected window.
        public static event EventHandler <string>ViewChanged;

        private static void OnViewChanged(string view)
        {
            ViewChanged?.Invoke(null, view);
        }

        // Changing this value will change the current view. 
        private static string view;
        public static string View
        {
            get
            {
                return view;
            }
            set
            {
                view = value;
                OnViewChanged(view);
            }
        }
    }
}
