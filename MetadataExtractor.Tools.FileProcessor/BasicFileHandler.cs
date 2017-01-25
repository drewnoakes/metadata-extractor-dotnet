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

using System.Collections.Generic;
using System.IO;

// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace MetadataExtractor.Tools.FileProcessor
{
    /// <summary>
    /// Does nothing with the output except enumerate it in memory and format descriptions. This is useful in order to
    /// flush out any potential exceptions raised during the formatting of extracted value descriptions.
    /// </summary>
    internal class BasicFileHandler : FileHandlerBase
    {
        public override void OnExtractionSuccess(string filePath, IList<Directory> directories, string relativePath, TextWriter log)
        {
            base.OnExtractionSuccess(filePath, directories, relativePath, log);

            // Iterate through all values, calling toString to flush out any formatting exceptions
            foreach (var directory in directories)
            {
                directory.Name.ToString();

                foreach (var tag in directory.Tags)
                {
                    tag.Name.ToString();
                    (tag.Description ?? "").ToString();
                }
            }
        }
    }
}