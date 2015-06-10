/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System.Text;
using Com.Drew.Lang;
using Com.Drew.Metadata.Exif;
using NUnit.Framework;
using Sharpen;

namespace Com.Drew.Metadata
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class DirectoryTest
    {
        private Directory _directory;

        // TODO write tests to validate type conversions from all underlying types
        [SetUp]
        public void Setup()
        {
            _directory = new MockDirectory();
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestSetAndGetMultipleTagsInSingleDirectory()
        {
            _directory.SetString(ExifDirectoryBase.TagAperture, "TAG_APERTURE");
            _directory.SetString(ExifDirectoryBase.TagBatteryLevel, "TAG_BATTERY_LEVEL");
            Assert.AreEqual("TAG_APERTURE", _directory.GetString(ExifDirectoryBase.TagAperture));
            Assert.AreEqual("TAG_BATTERY_LEVEL", _directory.GetString(ExifDirectoryBase.TagBatteryLevel));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestSetSameTagMultipleTimesOverwritesValue()
        {
            _directory.SetInt(ExifDirectoryBase.TagAperture, 1);
            _directory.SetInt(ExifDirectoryBase.TagAperture, 2);
            Assert.AreEqual(2, _directory.GetInt(ExifDirectoryBase.TagAperture));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestUnderlyingInt()
        {
            var value = 123;
            var tagType = 321;
            _directory.SetInt(tagType, value);
            Assert.AreEqual(value, _directory.GetInt(tagType));
            Assert.AreEqual(value, _directory.GetInteger(tagType));
            Assert.AreEqual(value, _directory.GetFloat(tagType), 0.00001);
            Assert.AreEqual(value, _directory.GetDouble(tagType), 0.00001);
            Assert.AreEqual((long)value, (object)_directory.GetLong(tagType));
            Assert.AreEqual(value.ToString(), _directory.GetString(tagType));
            Assert.AreEqual(new Rational(value, 1), _directory.GetRational(tagType));
            CollectionAssert.AreEqual(new[] { value }, _directory.GetIntArray(tagType));
            CollectionAssert.AreEqual(new[] { unchecked((byte)value) }, _directory.GetByteArray(tagType));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestSetAndGetIntArray()
        {
            var inputValues = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var tagType = 123;
            _directory.SetIntArray(tagType, inputValues);
            var outputValues = _directory.GetIntArray(tagType);
            Assert.IsNotNull(outputValues);
            Assert.AreEqual(inputValues.Length, outputValues.Length);
            for (var i = 0; i < inputValues.Length; i++)
            {
                var inputValue = inputValues[i];
                var outputValue = outputValues[i];
                Assert.AreEqual(inputValue, outputValue);
            }
            CollectionAssert.AreEqual(inputValues, _directory.GetIntArray(tagType));
            var outputString = new StringBuilder();
            for (var i1 = 0; i1 < inputValues.Length; i1++)
            {
                var inputValue = inputValues[i1];
                if (i1 > 0)
                {
                    outputString.Append(' ');
                }
                outputString.Append(inputValue);
            }
            Assert.AreEqual(outputString.ToString(), _directory.GetString(tagType));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestSetStringAndGetDate()
        {
            var date1 = "2002:01:30 24:59:59";
            var date2 = "2002:01:30 24:59";
            var date3 = "2002-01-30 24:59:59";
            var date4 = "2002-01-30 24:59";
            _directory.SetString(1, date1);
            _directory.SetString(2, date2);
            _directory.SetString(3, date3);
            _directory.SetString(4, date4);
            Assert.AreEqual(date1, _directory.GetString(1));
            Assert.AreEqual(new GregorianCalendar(2002, GregorianCalendar.January, 30, 24, 59, 59).GetTime(), _directory.GetDate(1));
            Assert.AreEqual(new GregorianCalendar(2002, GregorianCalendar.January, 30, 24, 59, 0).GetTime(), _directory.GetDate(2));
            Assert.AreEqual(new GregorianCalendar(2002, GregorianCalendar.January, 30, 24, 59, 59).GetTime(), _directory.GetDate(3));
            Assert.AreEqual(new GregorianCalendar(2002, GregorianCalendar.January, 30, 24, 59, 0).GetTime(), _directory.GetDate(4));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestSetIntArrayGetByteArray()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            _directory.SetIntArray(1, ints);
            var bytes = _directory.GetByteArray(1);
            Assert.IsNotNull(bytes);
            Assert.AreEqual(ints.Length, bytes.Length);
            Assert.AreEqual(1, bytes[0]);
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestSetStringGetInt()
        {
            var bytes = new byte[] { unchecked(0x01), unchecked(0x02), unchecked(0x03) };
            _directory.SetString(1, Encoding.UTF8.GetString(bytes));
            Assert.AreEqual(unchecked(0x010203), _directory.GetInt(1));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestContainsTag()
        {
            Assert.IsFalse(_directory.ContainsTag(ExifDirectoryBase.TagAperture));
            _directory.SetString(ExifDirectoryBase.TagAperture, "Tag Value");
            Assert.IsTrue(_directory.ContainsTag(ExifDirectoryBase.TagAperture));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public void TestGetNonExistentTagIsNullForAllTypes()
        {
            Assert.IsNull(_directory.GetString(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetInteger(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetDoubleObject(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetFloatObject(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetByteArray(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetDate(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetIntArray(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetLongObject(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetObject(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetRational(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetRationalArray(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetStringArray(ExifDirectoryBase.TagAperture));
        }

        [Test]
        public void TestToString()
        {
            Directory directory = new ExifIfd0Directory();
            Assert.AreEqual("Exif IFD0 Directory (0 tags)", directory.ToString());
            directory.SetString(1, "Tag 1");
            Assert.AreEqual("Exif IFD0 Directory (1 tag)", directory.ToString());
            directory.SetString(2, "Tag 2");
            Assert.AreEqual("Exif IFD0 Directory (2 tags)", directory.ToString());
        }
    }
}
