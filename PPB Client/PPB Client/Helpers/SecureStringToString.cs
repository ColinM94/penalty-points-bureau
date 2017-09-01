using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace PPB_Client.Helpers
{
    /// <summary>
    /// Converts a SecureString to a standard string.
    /// </summary>
    public static class SecureStringToString
    {
        /// <summary>
        /// Takes a SecureString and returns a standard string.
        /// </summary>
        public static string Convert(SecureString secureString)
        {
            if (secureString == null)
            {
                return string.Empty;
            }

            // Creates null pointer.
            IntPtr unmanagedString = IntPtr.Zero;

            try
            {
                // Moves secureString to unmanaged memory and assigns unmanagedString pointer to it.
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);

                // Converts the unmanaged string to a managed string and returns it 
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                // Frees up memory used my unmanagedString pointer
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }  
}
