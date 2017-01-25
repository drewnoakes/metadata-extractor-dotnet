#region License
//
// Copyright 2002-2017 Drew Noakes
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif
{
    /// <remarks>These tags can be found in Panasonic/Leica RAW, RW2 and RWL images. The index values are 'fake' but
    /// chosen specifically to make processing easier</remarks>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class PanasonicRawWbInfo2Directory : Directory
    {
        public const int TagNumWbEntries = 0;

        public const int TagWbType1 = 1;
        public const int TagWbRgbLevels1 = 2;

        public const int TagWbType2 = 5;
        public const int TagWbRgbLevels2 = 6;

        public const int TagWbType3 = 9;
        public const int TagWbRgbLevels3 = 10;

        public const int TagWbType4 = 13;
        public const int TagWbRgbLevels4 = 14;

        public const int TagWbType5 = 17;
        public const int TagWbRgbLevels5 = 18;

        public const int TagWbType6 = 21;
        public const int TagWbRgbLevels6 = 22;

        public const int TagWbType7 = 25;
        public const int TagWbRgbLevels7 = 26;


        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagNumWbEntries, "Num WB Entries" },
            { TagWbType1, "WB Type 1" },
            { TagWbRgbLevels1, "WB RGB Levels 1" },
            { TagWbType2, "WB Type 2" },
            { TagWbRgbLevels2, "WB RGB Levels 2" },
            { TagWbType3, "WB Type 3" },
            { TagWbRgbLevels3, "WB RGB Levels 3" },
            { TagWbType4, "WB Type 4" },
            { TagWbRgbLevels4, "WB RGB Levels 4" },
            { TagWbType5, "WB Type 5" },
            { TagWbRgbLevels5, "WB RGB Levels 5" },
            { TagWbType6, "WB Type 6" },
            { TagWbRgbLevels6, "WB RGB Levels 6" },
            { TagWbType7, "WB Type 7" },
            { TagWbRgbLevels7, "WB RGB Levels 7" }
        };

        public PanasonicRawWbInfo2Directory()
        {
            SetDescriptor(new PanasonicRawWbInfo2Descriptor(this));
        }

        public override string Name => "PanasonicRaw WbInfo2";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
