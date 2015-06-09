using System.Linq;

namespace Sharpen
{
    public static class Arrays
    {
        /// <summary>
        /// Counts array hash code
        /// </summary>
        /// <remarks>Implementation ported from openjdk source</remarks>
        /// <param name="a"></param>
        /// <returns></returns>
        public static int HashCode(sbyte[] a)
        {
            if (a == null)
                return 0;

            return a.Aggregate(1, (current, element) => 31*current + element);
        }
    }
}
