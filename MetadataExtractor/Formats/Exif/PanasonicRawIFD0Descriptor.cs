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

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="PanasonicRawIfd0Directory"/>.
    /// </summary>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class PanasonicRawIfd0Descriptor : TagDescriptor<PanasonicRawIfd0Directory>
    {
        public PanasonicRawIfd0Descriptor([NotNull] PanasonicRawIfd0Directory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case PanasonicRawIfd0Directory.TagPanasonicRawVersion:
                    return GetVersionBytesDescription(PanasonicRawIfd0Directory.TagPanasonicRawVersion, 2);
                case PanasonicRawIfd0Directory.TagOrientation:
                    return GetOrientationDescription(PanasonicRawIfd0Directory.TagOrientation);
                default:
                    return base.GetDescription(tagType);
            }
        }
    }
}
