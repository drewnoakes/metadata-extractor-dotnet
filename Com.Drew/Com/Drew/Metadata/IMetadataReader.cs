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

using Com.Drew.Lang;
using JetBrains.Annotations;

namespace Com.Drew.Metadata
{
    /// <summary>
    /// Defines an object capable of processing a particular type of metadata from a <see cref="Com.Drew.Lang.RandomAccessReader"/>.
    /// <para>
    /// Instances of this interface must be thread-safe and reusable.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public interface IMetadataReader
    {
        /// <summary>
        /// Extracts metadata from <c>reader</c> and merges it into the specified
        /// <see cref="Metadata"/>
        /// object.
        /// </summary>
        /// <param name="reader">
        /// The
        /// <see cref="Com.Drew.Lang.RandomAccessReader"/>
        /// from which the metadata should be extracted.
        /// </param>
        /// <param name="metadata">
        /// The
        /// <see cref="Metadata"/>
        /// object into which extracted values should be merged.
        /// </param>
        void Extract([NotNull] RandomAccessReader reader, [NotNull] Metadata metadata);
    }
}
