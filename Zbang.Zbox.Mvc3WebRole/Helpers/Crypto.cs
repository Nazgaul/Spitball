using System.Text;
using System.Web.Security;

namespace Zbang.Zbox.Mvc3WebRole.Helpers
{
    internal static class Crypto
    {
        private static readonly Encoding encoder = Encoding.Unicode;

        internal static string Encrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
                
            }
            return MachineKey.Encode(encoder.GetBytes(text),
                                        MachineKeyProtection.All);
        }

        internal static string Decrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
               
            }
            return encoder.GetString(MachineKey.Decode(text,
                                           MachineKeyProtection.All));
        }
    }

   
}
