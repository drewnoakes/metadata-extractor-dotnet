// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MetadataExtractor.Tools.FileProcessor
{
    internal abstract class FileHandlerBase : IFileHandler
    {
        private static readonly ICollection<string> _supportedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "jpg", "jpeg", "png", "gif", "bmp", "heic", "heif", "ico", "webp", "pcx", "ai", "eps",
            "nef", "crw", "cr2", "orf", "arw", "raf", "srw", "x3f", "rw2", "rwl", "dcr",
            "tif", "tiff", "psd", "dng",
            "mp3",
            "j2c", "jp2", "jpf", "jpm", "mj2",
            "3g2", "3gp", "m4v", "mov", "mp4", "m2v", "mts",
            "pbm", "pnm", "pgm", "ppm"
        };

        private int _processedFileCount;
        private int _exceptionCount;
        private int _errorCount;
        private long _totalFileByteCount;
        private long _totalReadByteCount;

        public virtual void OnStartingDirectory(string directoryPath)
        {}

        public bool ShouldProcess(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            return extension.Length > 1 && _supportedExtensions.Contains(extension.Substring(1));
        }

        public virtual void OnBeforeExtraction(string filePath, string relativePath, TextWriter log)
        {
            _processedFileCount++;
            _totalFileByteCount += new FileInfo(filePath).Length;
        }

        public virtual void OnExtractionError(string filePath, Exception exception, TextWriter log, long streamPosition)
        {
            _exceptionCount++;
            _totalReadByteCount += streamPosition;
            log.Write($"\t[{exception.GetType().Name}] {filePath}\n");
        }

        public virtual void OnExtractionSuccess(string filePath, IList<Directory> directories, string relativePath, TextWriter log, long streamPosition)
        {
            _totalReadByteCount += streamPosition;

            if (!directories.Any(d => d.HasError))
                return;

            // write out any errors
            log.WriteLine(filePath);
            foreach (var directory in directories)
            {
                if (!directory.HasError)
                    continue;
                foreach (var error in directory.Errors)
                {
                    log.Write($"\t[{directory.Name}] {error}\n");
                    _errorCount++;
                }
            }
        }

        public virtual void OnScanCompleted(TextWriter log)
        {
            if (_processedFileCount <= 0)
                return;

            log.WriteLine(
                $"Processed {_processedFileCount:#,##0} files (read {_totalReadByteCount:#,##0} of {_totalFileByteCount:#,##0} bytes) with {_exceptionCount:#,##0} exceptions and {_errorCount:#,##0} file errors\n");
        }
    }
}
