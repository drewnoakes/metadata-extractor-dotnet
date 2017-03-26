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

namespace MetadataExtractor.Formats.Jfif
{
    /// <summary>Provides human-readable string versions of the tags stored in a <see cref="JfifDirectory"/>.</summary>
    /// <remarks>
    /// Provides human-readable string versions of the tags stored in a <see cref="JfifDirectory"/>.
    /// <list type="bullet">
    ///   <item>http://en.wikipedia.org/wiki/JPEG_File_Interchange_Format</item>
    ///   <item>http://www.w3.org/Graphics/JPEG/jfif3.pdf</item>
    /// </list>
    /// </remarks>
    /// <author>Yuri Binev, Drew Noakes</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class JfifDescriptor : TagDescriptor<JfifDirectory>
    {
        public JfifDescriptor([NotNull] JfifDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case JfifDirectory.TagResX:
                    return GetImageResXDescription();
                case JfifDirectory.TagResY:
                    return GetImageResYDescription();
                case JfifDirectory.TagVersion:
                    return GetImageVersionDescription();
                case JfifDirectory.TagUnits:
                    return GetImageResUnitsDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetImageVersionDescription()
        {
            if (!Directory.TryGetInt32(JfifDirectory.TagVersion, out int value))
                return null;
            return $"{(value & 0xFF00) >> 8}.{value & 0x00FF}";
        }

        [CanBeNull]
        public string GetImageResYDescription()
        {
            if (!Directory.TryGetInt32(JfifDirectory.TagResY, out int value))
                return null;
            return $"{value} dot{(value == 1 ? string.Empty : "s")}";
        }

        [CanBeNull]
        public string GetImageResXDescription()
        {
            if (!Directory.TryGetInt32(JfifDirectory.TagResX, out int value))
                return null;
            return $"{value} dot{(value == 1 ? string.Empty : "s")}";
        }

        [CanBeNull]
        public string GetImageResUnitsDescription()
        {
            if (!Directory.TryGetInt32(JfifDirectory.TagUnits, out int value))
                return null;
            switch (value)
            {
                case 0:
                    return "none";
                case 1:
                    return "inch";
                case 2:
                    return "centimetre";
                default:
                    return "unit";
            }
        }
    }
}
