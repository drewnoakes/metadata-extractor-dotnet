using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace MetadataExtractor.Tests
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class DirectoryExtensionsTest
    {
        [Test]
        public void Int32Tests()
        {
            Action<Directory, int> assertPresentInt32 = (dictionary, i) =>
            {
                Assert.AreEqual(i, dictionary.GetInt32(i));
                int value;
                Assert.IsTrue(dictionary.TryGetInt32(i, out value));
                Assert.IsNotNull(dictionary.GetInt32Nullable(i));
                Assert.AreEqual(i, dictionary.GetInt32Nullable(i));
            };

            Action<Directory, int> assertMissingInt32 = (dictionary, i) =>
            {
                int value;
                Assert.IsFalse(dictionary.TryGetInt32(i, out value));
                Assert.Null(dictionary.GetInt32Nullable(i));
                try
                {
                    Assert.AreEqual(i, dictionary.GetInt32(i));
                    Assert.Fail("Should throw MetadataException");
                }
                catch (MetadataException) { }
            };

            Test(BuildDirectory(_singleValues), assertPresentInt32, assertMissingInt32);
            Test(BuildDirectory(_arraysOfSingleValues), assertPresentInt32, assertMissingInt32);
        }

        #region Test support

        private static void Test(Directory directory, Action<Directory, int> presentAssertion, Action<Directory, int> missingAssertion)
        {
            foreach (var tag in directory.Tags)
                presentAssertion(directory, tag.TagType);

            missingAssertion(directory, directory.Tags.Max(t => t.TagType) + 1);
        }

        private static Directory BuildDirectory(IEnumerable<object> values)
        {
            var directory = new MockDirectory();

            foreach (var pair in Enumerable.Range(1, int.MaxValue).Zip<int, object, Tuple<int, object>>(values, Tuple.Create))
                directory.Set(pair.Item1, pair.Item2);

            return directory;
        }

        private static readonly IEnumerable<object> _singleValues = new object[]
        {
            (byte)1,
            (sbyte)2,
            (short)3,
            (ushort)4,
            (int)5,
            (uint)6,
            (long)7,
            (ulong)8,
            (decimal)9,
            (float)10,
            (double)11,
            new Rational(12, 1),
            "13"
        };

        private static readonly IEnumerable<object> _arraysOfSingleValues = new object[]
        {
            new byte[] { 1 },
            new sbyte[] { 2 },
            new short[] { 3 },
            new ushort[] { 4 },
            new int[] { 5 },
            new uint[] { 6 },
            new long[] { 7 },
            new ulong[] { 8 },
            new decimal[] { 9 },
            new float[] { 10 },
            new double[] { 11 },
            new[] { new Rational(12, 1) },
            new[] { "13" }
        };

        #endregion
    }
}