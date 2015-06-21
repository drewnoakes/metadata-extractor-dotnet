#region License
//
// Copyright 2002-2015 Drew Noakes
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

using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Adobe
{
    /// <summary>Provides human-readable string versions of the tags stored in an AdobeJpegDirectory.</summary>
    public class AdobeJpegDescriptor : TagDescriptor<AdobeJpegDirectory>
    {
        public AdobeJpegDescriptor(AdobeJpegDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case AdobeJpegDirectory.TagColorTransform:
                    return GetColorTransformDescription();
                case AdobeJpegDirectory.TagDctEncodeVersion:
                    return GetDctEncodeVersionDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        private string GetDctEncodeVersionDescription()
        {
            int value;
            if (!Directory.TryGetInt32(AdobeJpegDirectory.TagDctEncodeVersion, out value))
                return null;

            return value == 0x64 ? "100" : value.ToString();
        }

        [CanBeNull]
        private string GetColorTransformDescription()
        {
            return GetIndexedDescription(AdobeJpegDirectory.TagColorTransform,
                "Unknown (RGB or CMYK)",
                "YCbCr",
                "YCCK");
        }
    }
}
