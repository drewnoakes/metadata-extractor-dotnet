using System;
using System.Text;

namespace Sharpen
{
    public abstract class CharSequence
    {
        public static implicit operator CharSequence (string str)
        {
            return new StringCharSequence (str);
        }

        public static implicit operator CharSequence (StringBuilder str)
        {
            return new StringCharSequence (str.ToString ());
        }

        public virtual int this[int index]
        {
            get { throw new NotImplementedException(); }
        }

        public virtual int Length
        {
            get { throw new NotImplementedException(); }
        }
    }

    class StringCharSequence: CharSequence
    {
        readonly string _str;

        public StringCharSequence (string str)
        {
            this._str = str;
        }

        public override string ToString ()
        {
            return _str;
        }

        public override int this[int index]
        {
            get { return _str[index]; }
        }

        public override int Length
        {
            get { return _str.Length; }
        }
    }
}
