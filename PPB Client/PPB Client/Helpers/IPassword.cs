using System.Security;

namespace PPB_Client.Helpers
{ 
    /// <summary>
    /// Secure password interface.
    /// </summary>
    public interface IPassword
    {
        /// <summary>
        /// SecureString version of password entered by user.
        /// </summary>
        SecureString Password { get; }
    }
}
