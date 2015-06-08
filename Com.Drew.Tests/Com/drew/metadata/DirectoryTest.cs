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
    public class DirectoryTest
    {
        private Directory _directory;

        // TODO write tests to validate type conversions from all underlying types
        [SetUp]
        public virtual void Setup()
        {
            _directory = new MockDirectory();
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestSetAndGetMultipleTagsInSingleDirectory()
        {
            _directory.SetString(ExifSubIFDDirectory.TagAperture, "TAG_APERTURE");
            _directory.SetString(ExifSubIFDDirectory.TagBatteryLevel, "TAG_BATTERY_LEVEL");
            Assert.AreEqual("TAG_APERTURE", _directory.GetString(ExifSubIFDDirectory.TagAperture));
            Assert.AreEqual("TAG_BATTERY_LEVEL", _directory.GetString(ExifSubIFDDirectory.TagBatteryLevel));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestSetSameTagMultipleTimesOverwritesValue()
        {
            _directory.SetInt(ExifSubIFDDirectory.TagAperture, 1);
            _directory.SetInt(ExifSubIFDDirectory.TagAperture, 2);
            Assert.AreEqual(2, _directory.GetInt(ExifSubIFDDirectory.TagAperture));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestUnderlyingInt()
        {
            int value = 123;
            int tagType = 321;
            _directory.SetInt(tagType, value);
            Assert.AreEqual(value, _directory.GetInt(tagType));
            Assert.AreEqual(Extensions.ValueOf(value), _directory.GetInteger(tagType));
            Assert.AreEqual((float)value, _directory.GetFloat(tagType), 0.00001);
            Assert.AreEqual((double)value, _directory.GetDouble(tagType), 0.00001);
            Assert.AreEqual((long)value, (object)_directory.GetLong(tagType));
            Assert.AreEqual(Extensions.ConvertToString(value), _directory.GetString(tagType));
            Assert.AreEqual(new Rational(value, 1), _directory.GetRational(tagType));
            CollectionAssert.AreEqual(new int[] { value }, _directory.GetIntArray(tagType));
            CollectionAssert.AreEqual(new sbyte[] { unchecked((sbyte)value) }, _directory.GetByteArray(tagType));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestSetAndGetIntArray()
        {
            int[] inputValues = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            int tagType = 123;
            _directory.SetIntArray(tagType, inputValues);
            int[] outputValues = _directory.GetIntArray(tagType);
            Assert.IsNotNull(outputValues);
            Assert.AreEqual(inputValues.Length, outputValues.Length);
            for (int i = 0; i < inputValues.Length; i++)
            {
                int inputValue = inputValues[i];
                int outputValue = outputValues[i];
                Assert.AreEqual(inputValue, outputValue);
            }
            CollectionAssert.AreEqual(inputValues, _directory.GetIntArray(tagType));
            StringBuilder outputString = new StringBuilder();
            for (int i_1 = 0; i_1 < inputValues.Length; i_1++)
            {
                int inputValue = inputValues[i_1];
                if (i_1 > 0)
                {
                    outputString.Append(' ');
                }
                outputString.Append(inputValue);
            }
            Assert.AreEqual(Extensions.ConvertToString(outputString), _directory.GetString(tagType));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestSetStringAndGetDate()
        {
            string date1 = "2002:01:30 24:59:59";
            string date2 = "2002:01:30 24:59";
            string date3 = "2002-01-30 24:59:59";
            string date4 = "2002-01-30 24:59";
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
        public virtual void TestSetIntArrayGetByteArray()
        {
            int[] ints = new int[] { 1, 2, 3, 4, 5 };
            _directory.SetIntArray(1, ints);
            sbyte[] bytes = _directory.GetByteArray(1);
            Assert.IsNotNull(bytes);
            Assert.AreEqual(ints.Length, bytes.Length);
            Assert.AreEqual(1, bytes[0]);
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestSetStringGetInt()
        {
            sbyte[] bytes = new sbyte[] { unchecked((int)(0x01)), unchecked((int)(0x02)), unchecked((int)(0x03)) };
            _directory.SetString(1, Runtime.GetStringForBytes(bytes));
            Assert.AreEqual(unchecked((int)(0x010203)), _directory.GetInt(1));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestContainsTag()
        {
            Assert.IsFalse(_directory.ContainsTag(ExifSubIFDDirectory.TagAperture));
            _directory.SetString(ExifSubIFDDirectory.TagAperture, "Tag Value");
            Assert.IsTrue(_directory.ContainsTag(ExifSubIFDDirectory.TagAperture));
        }

        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetNonExistentTagIsNullForAllTypes()
        {
            Assert.IsNull(_directory.GetString(ExifSubIFDDirectory.TagAperture));
            Assert.IsNull(_directory.GetInteger(ExifSubIFDDirectory.TagAperture));
            Assert.IsNull(_directory.GetDoubleObject(ExifSubIFDDirectory.TagAperture));
            Assert.IsNull(_directory.GetFloatObject(ExifSubIFDDirectory.TagAperture));
            Assert.IsNull(_directory.GetByteArray(ExifSubIFDDirectory.TagAperture));
            Assert.IsNull(_directory.GetDate(ExifSubIFDDirectory.TagAperture));
            Assert.IsNull(_directory.GetIntArray(ExifSubIFDDirectory.TagAperture));
            Assert.IsNull(_directory.GetLongObject(ExifSubIFDDirectory.TagAperture));
            Assert.IsNull(_directory.GetObject(ExifSubIFDDirectory.TagAperture));
            Assert.IsNull(_directory.GetRational(ExifSubIFDDirectory.TagAperture));
            Assert.IsNull(_directory.GetRationalArray(ExifSubIFDDirectory.TagAperture));
            Assert.IsNull(_directory.GetStringArray(ExifSubIFDDirectory.TagAperture));
        }

        [Test]
        public virtual void TestToString()
        {
            Directory directory = new ExifIFD0Directory();
            Assert.AreEqual("Exif IFD0 Directory (0 tags)", Extensions.ConvertToString(directory));
            directory.SetString(1, "Tag 1");
            Assert.AreEqual("Exif IFD0 Directory (1 tag)", Extensions.ConvertToString(directory));
            directory.SetString(2, "Tag 2");
            Assert.AreEqual("Exif IFD0 Directory (2 tags)", Extensions.ConvertToString(directory));
        }
    }
}
