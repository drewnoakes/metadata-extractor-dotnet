using System.Collections.Generic;
using System.Linq;
using Sharpen;

namespace Com.Drew.Lang
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class Iterables
    {
        public static IList<TE> ToList<TE>(IEnumerable<TE> iterable)
        {
            AList<TE> list = new AList<TE>();
            list.AddRange(iterable);
            return iterable.ToList();
        }
    }
}
