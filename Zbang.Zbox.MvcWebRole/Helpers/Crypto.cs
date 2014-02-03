using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Text;

namespace Zbang.Zbox.MvcWebRole.Helpers
{
    internal static class Crypto
    {
        internal static string Encrypt(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                return MachineKey.Encode(Encoding.Unicode.GetBytes(text),
                                        MachineKeyProtection.All);
            }
            else
            {
                return null;
            }
        }

        internal static string Decrypt(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                return Encoding.Unicode.GetString(MachineKey.Decode(text,
                                            MachineKeyProtection.All));
            }
            else
            {
                return null;
            }
        }
    }
}