using System;
using System.Collections;
using System.Collections.Generic;

namespace Sharpen
{
    public interface IIterator
    {
        bool HasNext ();
        object Next ();
        void Remove ();
    }

    public abstract class Iterator<T> : IEnumerator, IDisposable, IEnumerator<T>, IIterator
    {
        private T _lastValue;

        object IIterator.Next ()
        {
            return Next ();
        }

        public abstract bool HasNext ();
        public abstract T Next ();
        public abstract void Remove ();

        bool IEnumerator.MoveNext ()
        {
            if (HasNext ()) {
                _lastValue = Next ();
                return true;
            }
            return false;
        }

        void IEnumerator.Reset ()
        {
            throw new NotImplementedException ();
        }

        void IDisposable.Dispose ()
        {
        }

        T IEnumerator<T>.Current {
            get { return _lastValue; }
        }

        object IEnumerator.Current {
            get { return _lastValue; }
        }
    }
}
