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

using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;

namespace MetadataExtractor.Tools.FileProcessor
{
    internal interface IFileHandler
    {
        /// <summary>
        /// Called when the scan is about to start processing files in directory <c>path</c>.
        /// </summary>
        void OnStartingDirectory([NotNull] string directoryPath);

        /// <summary>
        /// Called to determine whether the implementation should process <paramref name="filePath"/>.
        /// </summary>
        bool ShouldProcess([NotNull] string filePath);

        /// <summary>
        /// Called before extraction is performed on <paramref name="filePath"/>.
        /// </summary>
        void OnBeforeExtraction([NotNull] string filePath, string relativePath, TextWriter log);

        /// <summary>
        /// Called when extraction on <paramref name="filePath"/> completed without an exception.
        /// </summary>
        void OnExtractionSuccess([NotNull] string filePath, [NotNull] IList<Directory> directories, [NotNull] string relativePath, [NotNull] TextWriter log);

        /// <summary>
        /// Called when extraction on <paramref name="filePath"/> resulted in an exception.
        /// </summary>
        void OnExtractionError([NotNull] string filePath, [NotNull] Exception exception, [NotNull] TextWriter log);

        /// <summary>
        /// Called when all files have been processed.
        /// </summary>
        void OnScanCompleted([NotNull] TextWriter log);
    }
}