// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;

namespace MetadataExtractor.Tools.FileProcessor
{
    internal interface IFileHandler
    {
        /// <summary>
        /// Called when the scan is about to start processing files in directory <c>path</c>.
        /// </summary>
        void OnStartingDirectory(string directoryPath);

        /// <summary>
        /// Called to determine whether the implementation should process <paramref name="filePath"/>.
        /// </summary>
        bool ShouldProcess(string filePath);

        /// <summary>
        /// Called before extraction is performed on <paramref name="filePath"/>.
        /// </summary>
        void OnBeforeExtraction(string filePath, string relativePath, TextWriter log);

        /// <summary>
        /// Called when extraction on <paramref name="filePath"/> completed without an exception.
        /// </summary>
        void OnExtractionSuccess(string filePath, IList<Directory> directories, string relativePath, TextWriter log, long streamPosition);

        /// <summary>
        /// Called when extraction on <paramref name="filePath"/> resulted in an exception.
        /// </summary>
        void OnExtractionError(string filePath, Exception exception, TextWriter log, long streamPosition);

        /// <summary>
        /// Called when all files have been processed.
        /// </summary>
        void OnScanCompleted(TextWriter log);
    }
}
