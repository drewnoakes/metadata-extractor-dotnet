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

using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Jfif
{
    /// <summary>Provides human-readable string versions of the tags stored in a JfifDirectory.</summary>
    /// <remarks>
    /// Provides human-readable string versions of the tags stored in a JfifDirectory.
    /// <p>
    /// More info at: http://en.wikipedia.org/wiki/JPEG_File_Interchange_Format
    /// </remarks>
    /// <author>Yuri Binev, Drew Noakes</author>
    public class JfifDescriptor : TagDescriptor<JfifDirectory>
    {
        public JfifDescriptor([NotNull] JfifDirectory directory)
            : base(directory)
        {
        }

        [CanBeNull]
        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case JfifDirectory.TagResx:
                {
                    return GetImageResXDescription();
                }

                case JfifDirectory.TagResy:
                {
                    return GetImageResYDescription();
                }

                case JfifDirectory.TagVersion:
                {
                    return GetImageVersionDescription();
                }

                case JfifDirectory.TagUnits:
                {
                    return GetImageResUnitsDescription();
                }

                default:
                {
                    return base.GetDescription(tagType);
                }
            }
        }

        [CanBeNull]
        public virtual string GetImageVersionDescription()
        {
            int? value = _directory.GetInteger(JfifDirectory.TagVersion);
            if (value == null)
            {
                return null;
            }
            return Extensions.StringFormat("%d.%d", ((int)value & unchecked((int)(0xFF00))) >> 8, (int)value & unchecked((int)(0xFF)));
        }

        [CanBeNull]
        public virtual string GetImageResYDescription()
        {
            int? value = _directory.GetInteger(JfifDirectory.TagResy);
            if (value == null)
            {
                return null;
            }
            return Extensions.StringFormat("%d dot%s", value, value == 1 ? string.Empty : "s");
        }

        [CanBeNull]
        public virtual string GetImageResXDescription()
        {
            int? value = _directory.GetInteger(JfifDirectory.TagResx);
            if (value == null)
            {
                return null;
            }
            return Extensions.StringFormat("%d dot%s", value, value == 1 ? string.Empty : "s");
        }

        [CanBeNull]
        public virtual string GetImageResUnitsDescription()
        {
            int? value = _directory.GetInteger(JfifDirectory.TagUnits);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case 0:
                {
                    return "none";
                }

                case 1:
                {
                    return "inch";
                }

                case 2:
                {
                    return "centimetre";
                }

                default:
                {
                    return "unit";
                }
            }
        }
    }
}
