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

using System.Collections.Generic;
using JetBrains.Annotations;

namespace Com.Drew.Metadata.Exif.Makernotes
{
    /// <summary>Describes tags specific to Ricoh cameras.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class RicohMakernoteDirectory : Directory
    {
        public const int TagMakernoteDataType = unchecked(0x0001);

        public const int TagVersion = unchecked(0x0002);

        public const int TagPrintImageMatchingInfo = unchecked(0x0E00);

        public const int TagRicohCameraInfoMakernoteSubIfdPointer = unchecked(0x2001);

        [NotNull]
        protected static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static RicohMakernoteDirectory()
        {
            TagNameMap[TagMakernoteDataType] = "Makernote Data Type";
            TagNameMap[TagVersion] = "Version";
            TagNameMap[TagPrintImageMatchingInfo] = "Print Image Matching (PIM) Info";
            TagNameMap[TagRicohCameraInfoMakernoteSubIfdPointer] = "Ricoh Camera Info Makernote Sub-IFD";
        }

        public RicohMakernoteDirectory()
        {
            SetDescriptor(new RicohMakernoteDescriptor(this));
        }

        public override string GetName()
        {
            return "Ricoh Makernote";
        }

        protected override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }
    }
}
