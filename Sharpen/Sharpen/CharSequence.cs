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
        readonly string str;

        public StringCharSequence (string str)
        {
            this.str = str;
        }

        public override string ToString ()
        {
            return str;
        }

        public override int this[int index]
        {
            get { return str[index]; }
        }

        public override int Length
        {
            get { return str.Length; }
        }
    }
}
