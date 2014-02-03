using System;

namespace Zbang.Zbox.Infrastructure.Exceptions
{
    public static class Throw
    {
        public static void OnNull(object param, string name)
        {
            if (param == null)
            {
                throw new ArgumentNullException(name, name + " cannot be null");
            }
        }
        public static void OnNull(string param, string name, bool whiteSpaceAllow = true)
        {
            if (whiteSpaceAllow)
            {
                if (string.IsNullOrEmpty(param))
                {
                    throw new ArgumentNullException(name, name + " cannot be null");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(param))
                {
                    throw new ArgumentNullException(name, name + " cannot be null");
                }
            }
        }

        public static void OnEmpty(Guid param, string name)
        {
            if (param == Guid.Empty)
            {
                throw new ArgumentNullException(name, name + " cannot be empty guid");
            }
        }

        public static void OnNegative(int param, string name)
        {
            if (param < 0)
            {
                throw new ArgumentException(name + " cannot be negative", name);
            }
        }
    }
}
