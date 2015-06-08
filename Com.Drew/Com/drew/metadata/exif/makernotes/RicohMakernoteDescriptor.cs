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
using Com.Drew.Metadata;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a
    /// <see cref="RicohMakernoteDescriptor"/>
    /// .
    /// <p>
    /// Some information about this makernote taken from here:
    /// http://www.ozhiker.com/electronics/pjmt/jpeg_info/ricoh_mn.html
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class RicohMakernoteDescriptor : TagDescriptor<RicohMakernoteDirectory>
    {
        public RicohMakernoteDescriptor([NotNull] RicohMakernoteDirectory directory)
            : base(directory)
        {
        }

        [CanBeNull]
        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                default:
                {
                    //            case TAG_PRINT_IMAGE_MATCHING_INFO:
                    //                return getPrintImageMatchingInfoDescription();
                    //            case TAG_PROPRIETARY_THUMBNAIL:
                    //                return getProprietaryThumbnailDataDescription();
                    return base.GetDescription(tagType);
                }
            }
        }
        //    @Nullable
        //    public String getPrintImageMatchingInfoDescription()
        //    {
        //        return getByteLengthDescription(TAG_PRINT_IMAGE_MATCHING_INFO);
        //    }
        //
        //    @Nullable
        //    public String getProprietaryThumbnailDataDescription()
        //    {
        //        return getByteLengthDescription(TAG_PROPRIETARY_THUMBNAIL);
        //    }
    }
}
