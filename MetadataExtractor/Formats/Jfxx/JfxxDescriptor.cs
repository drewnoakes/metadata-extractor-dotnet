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

namespace MetadataExtractor.Formats.Jfxx
{
    /// <summary>Provides human-readable string versions of the tags stored in a <see cref="JfxxDirectory"/>.</summary>
    /// <remarks>
    /// Provides human-readable string versions of the tags stored in a <see cref="JfxxDirectory"/>.
    /// <list type="bullet">
    ///   <item>http://en.wikipedia.org/wiki/JPEG_File_Interchange_Format</item>
    ///   <item>http://www.w3.org/Graphics/JPEG/jfif3.pdf</item>
    /// </list>
    /// </remarks>
    /// <author>Drew Noakes</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class JfxxDescriptor : TagDescriptor<JfxxDirectory>
    {
        public JfxxDescriptor([NotNull] JfxxDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case JfxxDirectory.TagExtensionCode:
                    return GetExtensionCodeDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetExtensionCodeDescription()
        {
            if (!Directory.TryGetInt32(JfxxDirectory.TagExtensionCode, out int value))
                return null;

            switch (value)
            {
                case 0x10: return "Thumbnail coded using JPEG";
                case 0x11: return "Thumbnail stored using 1 byte/pixel";
                case 0x13: return "Thumbnail stored using 3 bytes/pixel";
                default: return "Unknown extension code " + value;
            }
        }
    }
}
