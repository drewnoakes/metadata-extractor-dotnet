using System.Collections;

namespace Sharpen
{
    public class ListIterator : Iterator<object>
    {
        private readonly IList _list;
        private int _pos;

        public ListIterator(IList list, int n = -1)
        {
            this._list = list;
            this._pos = n;
        }

        public override object Next()
        {
            _pos++;
            return _list[_pos];
        }

        public override void Remove()
        {
            _list.RemoveAt(_pos);
        }

        public override bool HasNext()
        {
            return (this._pos < _list.Count - 1);
        }

        public void Set(object val)
        {
            _list[_pos] = val;
        }
    }
}
