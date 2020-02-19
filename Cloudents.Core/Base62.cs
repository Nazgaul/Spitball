using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cloudents.Core
{
    /// <summary>
    /// Struct whose numbering system consists of 0..9 a..z A..Z.  For example, "10" == 62.
    /// </summary>
    /// 
    [SuppressMessage("Microsoft.Performance", "CA2225", Justification = "Online code")]
    public struct Base62
    {
        private string _stringValue;

        public Base62(string value)
        {
            Value = 0;
            if (value == null)
            {
                _stringValue = null;
                return;

            }

            var count = 0;

            for (var i = value.Length - 1; i >= 0; i--)
            {
                var pos = (int)Math.Pow(62, count++);
                var part = 0;
                var c = value[i];

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

                Value += part * pos;
            }

            _stringValue = value;
        }

        public static bool TryParse(string s, out Base62 val)
        {
            try
            {
                val = new Base62(s);
                return true;
            }
            catch
            {
                val = null;
                return false;
            }
        }

        public Base62(long value)
        {
            Value = value;
            _stringValue = null;
        }

        private string ConvertToString(long value)
        {
            var mod = value % 62;
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

        public long Value { get; }

        public override string ToString()
        {
            return _stringValue ?? (_stringValue = ConvertToString(Value));
        }

        public override bool Equals(object obj)
        {
            return (obj as Base62?)?.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
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

        public static bool operator <=(Base62 left, Base62 right)
        {
            return (left == right) || (left.Value < right.Value);
        }

        public static bool operator >=(Base62 left, Base62 right)
        {
            return (left == right) || (left.Value > right.Value);
        }

        public static bool operator <(Base62 left, Base62 right)
        {
            return left.Value < right.Value;
        }

        public static bool operator >(Base62 left, Base62 right)
        {
            return left.Value > right.Value;
        }

        #endregion

        #region Implicit conversions to Base62

        public static implicit operator Base62(string number)
        {
            return new Base62(number);
        }

        public static implicit operator Base62(int number)
        {
            return new Base62(number);
        }

        #endregion

        #region Implicit conversions from Base62

        public static implicit operator string(Base62 number)
        {
            return number.ToString();
        }

        public static implicit operator long(Base62 number)
        {
            return number.Value;
        }

        #endregion

        #region Equality and Inequality (Base62, string, int)

        public static bool operator ==(Base62 left, Base62 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Base62 left, Base62 right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(Base62 left, int right)
        {
            return left.Value == right;
        }

        public static bool operator !=(Base62 left, int right)
        {
            return left.Value != right;
        }

        public static bool operator ==(Base62 left, string right)
        {
            return left.Value == new Base62(right).Value;
        }

        public static bool operator !=(Base62 left, string right)
        {
            return left.Value != new Base62(right).Value;
        }

        #endregion
    }
}
