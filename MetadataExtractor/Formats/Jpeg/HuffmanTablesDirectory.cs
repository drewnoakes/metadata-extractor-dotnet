#region License
//
// Copyright 2002-2019 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
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
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Directory of tables for the DHT (Define Huffman Table(s)) segment.</summary>
    /// <author>Nadahar</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class HuffmanTablesDirectory : Directory
    {
        public const int TagNumberOfTables = 1;

        internal static byte[] TypicalLuminanceDcLengths = {
            0x00, 0x01, 0x05, 0x01, 0x01, 0x01, 0x01, 0x01,
            0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        internal static byte[] TypicalLuminanceDcValues = {
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
            0x08, 0x09, 0x0A, 0x0B
        };

        internal static byte[] TypicalChrominanceDcLengths = {
            0x00, 0x03, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
            0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        internal static byte[] TypicalChrominanceDcValues = {
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
            0x08, 0x09, 0x0A, 0x0B
        };

        internal static byte[] TypicalLuminanceAcLengths = {
            0x00, 0x02, 0x01, 0x03, 0x03, 0x02, 0x04, 0x03,
            0x05, 0x05, 0x04, 0x04, 0x00, 0x00, 0x01, 0x7D
        };

        internal static byte[] TypicalLuminanceAcValues = {
            0x01, 0x02, 0x03, 0x00, 0x04, 0x11, 0x05, 0x12,
            0x21, 0x31, 0x41, 0x06, 0x13, 0x51, 0x61, 0x07,
            0x22, 0x71, 0x14, 0x32, 0x81, 0x91, 0xA1, 0x08,
            0x23, 0x42, 0xB1, 0xC1, 0x15, 0x52, 0xD1, 0xF0,
            0x24, 0x33, 0x62, 0x72, 0x82, 0x09, 0x0A, 0x16,
            0x17, 0x18, 0x19, 0x1A, 0x25, 0x26, 0x27, 0x28,
            0x29, 0x2A, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39,
            0x3A, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49,
            0x4A, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59,
            0x5A, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69,
            0x6A, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79,
            0x7A, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89,
            0x8A, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98,
            0x99, 0x9A, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7,
            0xA8, 0xA9, 0xAA, 0xB2, 0xB3, 0xB4, 0xB5, 0xB6,
            0xB7, 0xB8, 0xB9, 0xBA, 0xC2, 0xC3, 0xC4, 0xC5,
            0xC6, 0xC7, 0xC8, 0xC9, 0xCA, 0xD2, 0xD3, 0xD4,
            0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xE1, 0xE2,
            0xE3, 0xE4, 0xE5, 0xE6, 0xE7, 0xE8, 0xE9, 0xEA,
            0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8,
            0xF9, 0xFA
        };

        internal static byte[] TypicalChrominanceAcLengths = {
            0x00, 0x02, 0x01, 0x02, 0x04, 0x04, 0x03, 0x04,
            0x07, 0x05, 0x04, 0x04, 0x00, 0x01, 0x02, 0x77
        };

        internal static byte[] TypicalChrominanceAcValues = {
            0x00, 0x01, 0x02, 0x03, 0x11, 0x04, 0x05, 0x21,
            0x31, 0x06, 0x12, 0x41, 0x51, 0x07, 0x61, 0x71,
            0x13, 0x22, 0x32, 0x81, 0x08, 0x14, 0x42, 0x91,
            0xA1, 0xB1, 0xC1, 0x09, 0x23, 0x33, 0x52, 0xF0,
            0x15, 0x62, 0x72, 0xD1, 0x0A, 0x16, 0x24, 0x34,
            0xE1, 0x25, 0xF1, 0x17, 0x18, 0x19, 0x1A, 0x26,
            0x27, 0x28, 0x29, 0x2A, 0x35, 0x36, 0x37, 0x38,
            0x39, 0x3A, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48,
            0x49, 0x4A, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58,
            0x59, 0x5A, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68,
            0x69, 0x6A, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78,
            0x79, 0x7A, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87,
            0x88, 0x89, 0x8A, 0x92, 0x93, 0x94, 0x95, 0x96,
            0x97, 0x98, 0x99, 0x9A, 0xA2, 0xA3, 0xA4, 0xA5,
            0xA6, 0xA7, 0xA8, 0xA9, 0xAA, 0xB2, 0xB3, 0xB4,
            0xB5, 0xB6, 0xB7, 0xB8, 0xB9, 0xBA, 0xC2, 0xC3,
            0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9, 0xCA, 0xD2,
            0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA,
            0xE2, 0xE3, 0xE4, 0xE5, 0xE6, 0xE7, 0xE8, 0xE9,
            0xEA, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8,
            0xF9, 0xFA
        };
        
        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagNumberOfTables, "Number of Tables" }
        };

        public HuffmanTablesDirectory()
        {
            SetDescriptor(new HuffmanTablesDescriptor(this));
        }

        public override string Name => "Huffman";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }

        /// <remarks>Use GetNumberOfTables for bounds-checking.</remarks>
        /// <param name="tableNumber">The zero-based index of the table. This number is normally between 0 and 3.</param>
        /// <returns>The HuffmanTable having the specified number.</returns>
        [NotNull]
        public HuffmanTable GetTable(int tableNumber)
        {
            return Tables[tableNumber];
        }

        /// <returns>The number of Huffman tables held by this HuffmanTablesDirectory instance.</returns>
        public int GetNumberOfTables()
        {
            return this.GetInt32(TagNumberOfTables);
        }

        /// <returns>The List of HuffmanTables in this Directory.</returns>
        [NotNull]
        public List<HuffmanTable> Tables { get; set; } = new List<HuffmanTable>(4);

        /// <summary>Evaluates whether all the tables in this HuffmanTablesDirectory are "typical" Huffman tables.</summary>
        /// <remarks>
        /// "Typical" has a special meaning in this context as the JPEG standard
        /// (ISO/IEC 10918 or ITU-T T.81) defines 4 Huffman tables that has been
        /// developed from the average statistics of a large set of images with 8-bit
        /// precision. Using these instead of calculating the optimal Huffman tables
        /// for a given image is faster, and is preferred by many hardware encoders
        /// and some hardware decoders.
        /// <para/>
        /// Even though the JPEG standard doesn't define these as "standard tables"
        /// and requires a decoder to be able to read any valid Huffman tables, some
        /// are in reality limited decoding images using these "typical" tables.
        /// Standards like DCF(Design rule for Camera File system) and DLNA(Digital
        /// Living Network Alliance) actually requires any compliant JPEG to use only
        /// the "typical" Huffman tables.
        /// <para/>
        /// This is also related to the term "optimized" JPEG. An "optimized" JPEG is
        /// a JPEG that doesn't use the "typical" Huffman tables.
        /// </remarks>
        public bool IsTypical()
        {
            if (Tables.Count == 0)
            {
                return false;
            }
            foreach (HuffmanTable table in Tables)
            {
                if (!table.IsTypical())
                {
                    return false;
                }
            }
            return true;
        }

        /// <remarks>The opposite of IsTypical().</remarks>
        /// <returns>
        /// Whether or not the tables in this HuffmanTablesDirectory
        /// are "optimized" - which means that at least one of them aren't
        /// one of the "typical" Huffman tables.
        /// </returns>
        public bool IsOptimized()
        {
            return !IsTypical();
        }

    }

    /// <summary>An instance of this class holds a JPEG Huffman table.</summary>
    public class HuffmanTable
    {
        private byte[] _lengthBytes;
        private byte[] _valueBytes;

        public HuffmanTable([NotNull] HuffmanTableClass tableClass, int tableDestinationId, [NotNull] byte[] lengthBytes, [NotNull] byte[] valueBytes)
        {
            _lengthBytes = lengthBytes ?? throw new ArgumentNullException("lengthBytes cannot be null.");
            _valueBytes = valueBytes ?? throw new ArgumentNullException("valueBytes cannot be null.");

            TableClass = tableClass;
            TableDestinationId = tableDestinationId;
            TableLength = _valueBytes.Length + 17;
        }

        /// <returns>The table length in bytes.</returns>
        public int TableLength { get; private set; }

        /// <returns>The HuffmanTableClass of this table.</returns>
        public HuffmanTableClass TableClass { get; private set; }

        /// <returns>The destination identifier for this table.</returns>
        public int TableDestinationId { get; private set; }

        /// <returns>A byte array with the L values for this table.</returns>
        [NotNull]
        public byte[] LengthBytes
        {
            get
            {
                byte[] result = new byte[_lengthBytes.Length];
                Array.Copy(_lengthBytes, 0, result, 0, _lengthBytes.Length);
                return result;
            }
        }

        /// <returns>A byte array with the V values for this table.</returns>
        [NotNull]
        public byte[] ValueBytes
        {
            get
            {
                byte[] result = new byte[_valueBytes.Length];
                Array.Copy(_valueBytes, 0, result, 0, _valueBytes.Length);
                return result;
            }
        }

        /// <summary>Evaluates whether this table is a "typical" Huffman table.</summary>
        /// <remarks>
        /// "Typical" has a special meaning in this context as the JPEG standard
        /// (ISO/IEC 10918 or ITU-T T.81) defines 4 Huffman tables that has been
        /// developed from the average statistics of a large set of images with 8-bit
        /// precision. Using these instead of calculating the optimal Huffman tables
        /// for a given image is faster, and is preferred by many hardware encoders
        /// and some hardware decoders.
        /// <para/>
        /// Even though the JPEG standard doesn't define these as "standard tables"
        /// and requires a decoder to be able to read any valid Huffman tables, some
        /// are in reality limited decoding images using these "typical" tables.
        /// Standards like DCF(Design rule for Camera File system) and DLNA(Digital
        /// Living Network Alliance) actually requires any compliant JPEG to use only
        /// the "typical" Huffman tables.
        /// <para/>
        /// This is also related to the term "optimized" JPEG. An "optimized" JPEG is
        /// a JPEG that doesn't use the "typical" Huffman tables.
        /// </remarks>
        /// <returns>Whether or not this table is one of the predefined "typical" Huffman tables.</returns>
        public bool IsTypical()
        {
            if (TableClass == HuffmanTableClass.DC)
            {
                return
                    Equals(_lengthBytes, HuffmanTablesDirectory.TypicalLuminanceDcLengths) &&
                    Equals(_valueBytes, HuffmanTablesDirectory.TypicalLuminanceDcValues) ||
                    Equals(_lengthBytes, HuffmanTablesDirectory.TypicalChrominanceDcLengths) &&
                    Equals(_valueBytes, HuffmanTablesDirectory.TypicalChrominanceDcValues);
            }
            else if (TableClass == HuffmanTableClass.AC)
            {
                return
                    Equals(_lengthBytes, HuffmanTablesDirectory.TypicalLuminanceAcLengths) &&
                    Equals(_valueBytes, HuffmanTablesDirectory.TypicalLuminanceAcValues) ||
                    Equals(_lengthBytes, HuffmanTablesDirectory.TypicalChrominanceAcLengths) &&
                    Equals(_valueBytes, HuffmanTablesDirectory.TypicalChrominanceAcValues);
            }
            return false;
        }

        /// <remarks>The opposite of IsTypical().</remarks>
        /// <returns>
        /// Whether or not this table is "optimized" - which means that
        /// it isn't one of the "typical" Huffman tables.
        /// </returns>
        public bool IsOptimized()
        {
            return !IsTypical();
        }

        public static HuffmanTableClass TypeOf(int value)
        {
            switch (value)
            {
                case 0:
                    return HuffmanTableClass.DC;
                case 1:
                    return HuffmanTableClass.AC;
                default:
                    return HuffmanTableClass.UNKNOWN;
            }
        }
    }

    public enum HuffmanTableClass
    {
        DC,
        AC,
        UNKNOWN
    }

}
