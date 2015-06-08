using System.Collections.Generic;
using Sharpen;

namespace Com.Drew.Lang
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class Iterables
    {
        public static IList<TE> ToList<TE>(Iterable<TE> iterable)
        {
            AList<TE> list = new AList<TE>();
            list.AddRange(iterable);
            return list;
        }
    }
}
