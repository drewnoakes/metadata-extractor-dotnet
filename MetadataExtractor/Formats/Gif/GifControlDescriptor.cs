#region License
//
// Copyright 2002-2019 Drew Noakes
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

using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Gif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public class GifControlDescriptor : TagDescriptor<GifControlDirectory>
    {
        public GifControlDescriptor([NotNull] GifControlDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case GifControlDirectory.TagDisposalMethod:
                    return GetDisposalMethodDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        public string GetDisposalMethodDescription()
        {
            if (!Directory.TryGetInt32(GifControlDirectory.TagDisposalMethod, out int value))
                return null;

            switch (value)
            {
                case 0:
                    return "Not Specified";
                case 1:
                    return "Don't Dispose";
                case 2:
                    return "Restore to Background Color";
                case 3:
                    return "Restore to Previous";
                case 4:
                case 5:
                case 6:
                case 7:
                    return "To Be Defined";
                default:
                    return $"Invalid value ({value})";
            }
        }
    }
}
