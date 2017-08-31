using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPB_Client.Helpers
{
    /// <summary>
    /// Class keeps track of currently selected main view. 
    /// </summary>
    public static class CurrentView
    {
        // Event notifies of currently selected window.
        public static event EventHandler <string>ViewChanged;

        private static void OnViewChanged(string view)
        {
            ViewChanged?.Invoke(null, view);
        }

        private static string view = "LoginView";
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
