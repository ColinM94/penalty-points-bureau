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

        private static string view;

        /// <summary>
        /// Changing this value will change the current main view. e.g View="LoginView".
        /// </summary>
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
