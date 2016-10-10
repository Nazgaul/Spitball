﻿using System;
using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Url
{
    /// <summary>
    /// Struct whose numbering system consists of 0..9 a..z A..Z.  For example, "10" == 62.
    /// </summary>
    public struct Base62
    {
        private readonly long m_Value;
        private string m_StringValue;

        public Base62(string value)
        {
            m_Value = 0;
            int count = 0;

            for (int i = value.Length - 1; i >= 0; i--)
            {
                int pos = (int)Math.Pow(62, count++);
                int part = 0;
                char c = value[i];

                if (c >= 48 && c <= 57)
                {
                    part += c - 48;
                }
                else if (c >= 97 && c <= 122)
                {
                    part += c - 87;
                }
                else if (c >= 65 && c <= 90)
                {
                    part += c - 29;
                }
                else
                {
                    throw new Exception(
                        $"The character '{c}' is not legal; only the characters 1-9, a-z and A-Z are legal.");
                }

                m_Value += part * pos;
            }

            m_StringValue = value;
        }

        public Base62(long value)
        {
            m_Value = value;
            m_StringValue = null;
        }



        private string ConvertToString(long value)
        {
            long mod = value % 62;
            char val;

            if (mod >= 0 && mod <= 9)
            {
                val = Convert.ToChar(mod + 48);
            }
            else if (mod >= 10 && mod <= 35)
            {
                val = Convert.ToChar(mod + 87);
            }
            else if (mod >= 36 && mod <= 62)
            {
                val = Convert.ToChar(mod + 29);
            }
            else
            {
                throw new Exception();
            }

            if (value < 62)
            {
                return val.ToString(CultureInfo.InvariantCulture);
            }
            return ConvertToString(value / 62) + val;
        }

        public long Value => m_Value;

        public override string ToString()
        {
            return m_StringValue ?? (m_StringValue = ConvertToString(m_Value));
        }

        public override bool Equals(object obj)
        {
            return (obj as Base62?)?.Value == Value;
        }

        public override int GetHashCode()
        {
            return m_Value.GetHashCode();
        }

        #region Base 62 Math

        public static Base62 operator +(Base62 a, Base62 b)
        {
            return new Base62(a.Value + b.Value);
        }

        public static Base62 operator -(Base62 a, Base62 b)
        {
            return new Base62(a.Value - b.Value);
        }

        public static Base62 operator *(Base62 a, Base62 b)
        {
            return new Base62(a.Value * b.Value);
        }

        public static Base62 operator /(Base62 a, Base62 b)
        {
            return new Base62(a.Value / b.Value);
        }

        #endregion

        #region Comparison

        public static bool operator <=(Base62 a, Base62 b)
        {
            return (a == b) || (a.Value < b.Value);
        }

        public static bool operator >=(Base62 a, Base62 b)
        {
            return (a == b) || (a.Value > b.Value);
        }

        public static bool operator <(Base62 a, Base62 b)
        {
            return a.Value < b.Value;
        }

        public static bool operator >(Base62 a, Base62 b)
        {
            return a.Value > b.Value;
        }

        #endregion

        #region Implicit conversions to Base62

        public static implicit operator Base62(string a)
        {
            return new Base62(a);
        }

        public static implicit operator Base62(int a)
        {
            return new Base62(a);
        }

        #endregion

        #region Implicit conversions from Base62

        public static implicit operator string(Base62 a)
        {
            return a.ToString();
        }

        public static implicit operator long(Base62 a)
        {
            return a.Value;
        }

        #endregion

        #region Equality and Inequality (Base62, string, int)

        public static bool operator ==(Base62 a, Base62 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Base62 a, Base62 b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(Base62 a, int b)
        {
            return a.Value == b;
        }

        public static bool operator !=(Base62 a, int b)
        {
            return a.Value != b;
        }

        public static bool operator ==(Base62 a, string b)
        {
            return a.Value == new Base62(b).Value;
        }

        public static bool operator !=(Base62 a, string b)
        {
            return a.Value != new Base62(b).Value;
        }

        #endregion
    }
}
