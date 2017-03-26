#region License
//
// Copyright 2002-2017 Drew Noakes
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
    public class GifAnimationDescriptor : TagDescriptor<GifAnimationDirectory>
    {
        public GifAnimationDescriptor([NotNull] GifAnimationDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case GifAnimationDirectory.TagIterationCount:
                    return GetIterationCountDescription();
                default:
                    return null;
            }
        }

        private string GetIterationCountDescription()
        {
            if (!Directory.TryGetUInt16(GifAnimationDirectory.TagIterationCount, out ushort count))
                return null;
            return count == 0 ? "Infinite" : count == 1 ? "Once" : count == 2 ? "Twice" : $"{count} times";
        }
    }
}
