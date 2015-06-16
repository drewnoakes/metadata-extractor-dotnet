using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Directory = MetadataExtractor.Directory;

namespace FileLabeller
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
        void OnExtractionSuccess([NotNull] string filePath, [NotNull] IReadOnlyList<Directory> directories, [NotNull] string relativePath, [NotNull] TextWriter log);

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