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

using JetBrains.Annotations;

namespace MetadataExtractor
{
    public interface ITagDescriptor
    {
        /// <summary>Decodes the raw value stored for <paramref name="tagType"/>.</summary>
        /// <remarks>
        /// Where possible, known values will be substituted here in place of the raw
        /// tokens actually kept in the metadata segment.  If no substitution is
        /// available, the value provided by <see cref="DirectoryExtensions.GetString(MetadataExtractor.Directory,int)"/> will be returned.
        /// </remarks>
        /// <param name="tagType">The tag to find a description for.</param>
        /// <returns>
        /// A description of the image's value for the specified tag, or
        /// <c>null</c> if the tag hasn't been defined.
        /// </returns>
        [CanBeNull]
        string GetDescription(int tagType);
    }
}