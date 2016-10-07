#region License
//
// Copyright 2002-2016 Drew Noakes
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MetadataExtractor.Tests
{
    /// <summary>Unit tests for <see cref="DirectoryExtensions"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class DirectoryExtensionsTest
    {
        [Fact]
        public void Int32Tests()
        {
            Action<Directory, int> assertPresentInt32 = (dictionary, i) =>
            {
                Assert.Equal(i, dictionary.GetInt32(i));
                int value;
                Assert.True(dictionary.TryGetInt32(i, out value));
                Assert.Equal(i, dictionary.GetInt32(i));
            };

            Action<Directory, int> assertMissingInt32 = (dictionary, i) =>
            {
                int value;
                Assert.False(dictionary.TryGetInt32(i, out value));
                Assert.Throws<MetadataException>(() => dictionary.GetInt32(i));
            };

            Test(BuildDirectory(_singleValues), assertPresentInt32, assertMissingInt32);
            Test(BuildDirectory(_arraysOfSingleValues), assertPresentInt32, assertMissingInt32);
        }

        [Fact]
        public void Int64Tests()
        {
            Action<Directory, int> assertPresentInt64 = (dictionary, i) =>
            {
                Assert.Equal(i, dictionary.GetInt64(i));
                long value;
                Assert.True(dictionary.TryGetInt64(i, out value));
                Assert.Equal(i, dictionary.GetInt64(i));
            };

            Action<Directory, int> assertMissingInt64 = (dictionary, i) =>
            {
                long value;
                Assert.False(dictionary.TryGetInt64(i, out value));
                Assert.Throws<MetadataException>(() => dictionary.GetInt64(i));
            };

            Test(BuildDirectory(_singleValues), assertPresentInt64, assertMissingInt64);
            Test(BuildDirectory(_arraysOfSingleValues), assertPresentInt64, assertMissingInt64);
        }

        [Fact]
        public void SingleTests()
        {
            Action<Directory, int> assertPresentSingle = (dictionary, i) =>
            {
                Assert.Equal(i, dictionary.GetSingle(i));
                float value;
                Assert.True(dictionary.TryGetSingle(i, out value));
                Assert.Equal(i, dictionary.GetSingle(i));
            };

            Action<Directory, int> assertMissingSingle = (dictionary, i) =>
            {
                float value;
                Assert.False(dictionary.TryGetSingle(i, out value));
                Assert.Throws<MetadataException>(() => dictionary.GetSingle(i));
            };

            Test(BuildDirectory(_singleValues), assertPresentSingle, assertMissingSingle);
            Test(BuildDirectory(_arraysOfSingleValues), assertPresentSingle, assertMissingSingle);
        }

        [Fact]
        public void DoubleTests()
        {
            Action<Directory, int> assertPresentDouble = (dictionary, i) =>
            {
                Assert.Equal(i, dictionary.GetDouble(i));
                double value;
                Assert.True(dictionary.TryGetDouble(i, out value));
                Assert.Equal(i, dictionary.GetDouble(i));
            };

            Action<Directory, int> assertMissingDouble = (dictionary, i) =>
            {
                double value;
                Assert.False(dictionary.TryGetDouble(i, out value));
                Assert.Throws<MetadataException>(() => dictionary.GetDouble(i));
            };

            Test(BuildDirectory(_singleValues), assertPresentDouble, assertMissingDouble);
            Test(BuildDirectory(_arraysOfSingleValues), assertPresentDouble, assertMissingDouble);
        }

        [Fact]
        public void BooleanTests()
        {
            Action<Directory, int> assertPresentTrueBoolean = (dictionary, i) =>
            {
                Assert.True(dictionary.GetBoolean(i));
                bool value;
                Assert.True(dictionary.TryGetBoolean(i, out value));
                Assert.True(dictionary.GetBoolean(i));
            };

            Action<Directory, int> assertPresentFalseBoolean = (dictionary, i) =>
            {
                Assert.False(dictionary.GetBoolean(i));
                bool value;
                Assert.True(dictionary.TryGetBoolean(i, out value));
                Assert.False(dictionary.GetBoolean(i));
            };

            Action<Directory, int> assertMissingBoolean = (dictionary, i) =>
            {
                bool value;
                Assert.False(dictionary.TryGetBoolean(i, out value));
                Assert.Throws<MetadataException>(() => dictionary.GetBoolean(i));
            };

            // NOTE string is not convertible to boolean other than for "true" and "false"

            Test(BuildDirectory(_singleValues.Where(v => !(v is string))), assertPresentTrueBoolean, assertMissingBoolean);
            Test(BuildDirectory(_singleZeroValues.Where(v => !(v is string))), assertPresentFalseBoolean, assertMissingBoolean);
            Test(BuildDirectory(_arraysOfSingleValues.Where(v => !(v is string[]))), assertPresentTrueBoolean, assertMissingBoolean);
            Test(BuildDirectory(_arraysOfSingleZeroValues.Where(v => !(v is string[]))), assertPresentFalseBoolean, assertMissingBoolean);

            var directory = new MockDirectory();

            directory.Set(1, "True");
            directory.Set(2, "true");
            directory.Set(3, "False");
            directory.Set(4, "false");

            Assert.True(directory.GetBoolean(1));
            Assert.True(directory.GetBoolean(2));
            Assert.False(directory.GetBoolean(3));
            Assert.False(directory.GetBoolean(4));
        }

        #region Test support

        private static void Test(Directory directory, Action<Directory, int> presentAssertion, Action<Directory, int> missingAssertion)
        {
            foreach (var tag in directory.Tags)
                presentAssertion(directory, tag.Type);

            missingAssertion(directory, directory.Tags.Max(t => t.Type) + 1);
        }

        private static Directory BuildDirectory(IEnumerable<object> values)
        {
            var directory = new MockDirectory();

            foreach (var pair in Enumerable.Range(1, int.MaxValue).Zip(values, Tuple.Create))
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

        private static readonly IEnumerable<object> _singleZeroValues = new object[]
        {
            (byte)0,
            (sbyte)0,
            (short)0,
            (ushort)0,
            (int)0,
            (uint)0,
            (long)0,
            (ulong)0,
            (decimal)0,
            (float)0,
            (double)0,
            new Rational(0, 0),
            "0"
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

        private static readonly IEnumerable<object> _arraysOfSingleZeroValues = new object[]
        {
            new byte[] { 0 },
            new sbyte[] { 0 },
            new short[] { 0 },
            new ushort[] { 0 },
            new int[] { 0 },
            new uint[] { 0 },
            new long[] { 0 },
            new ulong[] { 0 },
            new decimal[] { 0 },
            new float[] { 0 },
            new double[] { 0 },
            new[] { new Rational(0, 0) },
            new[] { "0" }
        };

        #endregion
    }
}