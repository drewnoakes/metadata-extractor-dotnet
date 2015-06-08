using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Com.Drew.Imaging;
using Com.Drew.Metadata;
using JetBrains.Annotations;
using Sharpen;
using Directory = System.IO.Directory;

namespace FileLabeller
{
    // TODO port MarkdownTableOutputHandler
    // TODO port UnknownTagHandler

    internal static class Program
    {
        private static int Main(string[] args)
        {
            var directories = new List<string>();

            var fileHandler = (IFileHandler)null;
            var log = Console.Out;

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg.Equals("--text", StringComparison.OrdinalIgnoreCase))
                {
                    // If "--text" is specified, write the discovered metadata into a sub-folder relative to the image
                    fileHandler = new TextFileOutputHandler();
                }
//                else if (arg.Equals("--markdown", StringComparison.OrdinalIgnoreCase))
//                {
//                    // If "--markdown" is specified, write a summary table in markdown format to standard out
//                    fileHandler = new MarkdownTableOutputHandler();
//                }
//                else if (arg.Equals("--unknown", StringComparison.OrdinalIgnoreCase))
//                {
//                    // If "--unknown" is specified, write CSV tallying unknown tag counts
//                    fileHandler = new UnknownTagHandler();
//                }
                else if (arg.Equals("--log-file"))
                {
                    if (i == args.Length - 1)
                    {
                        PrintUsage();
                        return 1;
                    }
                    log = new FileWriter(args[++i], append: false);
                }
                else
                {
                    // Treat this argument as a directory
                    directories.Add(arg);
                }
            }

            if (directories.Count == 0)
            {
                Console.Error.WriteLine("Expects one or more directories as arguments.");
                PrintUsage();
                return 1;
            }

            if (fileHandler == null)
                fileHandler = new BasicFileHandler();

            var stopwatch = Stopwatch.StartNew();

            foreach (var directory in directories)
                ProcessDirectory(directory, fileHandler, "", log);

            fileHandler.OnScanCompleted(log);

            Console.Out.WriteLine("Completed in {0:0.##} ms", stopwatch.Elapsed.TotalMilliseconds);

            if (!ReferenceEquals(Console.Out, log))
                log.Dispose();

            if (Debugger.IsAttached)
                Console.ReadLine();

            return 0;
        }

        private static void PrintUsage()
        {
            Console.Out.WriteLine("Usage:");
            Console.Out.WriteLine();
            Console.Out.WriteLine("  {0}.exe [--text|--markdown|--unknown] [--log-file <file-name>]",
                Assembly.GetExecutingAssembly().GetName().Name);
        }

        private static void ProcessDirectory([NotNull] string path, [NotNull] IFileHandler handler, [NotNull] string relativePath, [NotNull] TextWriter log)
        {
            var entries = Directory.GetFileSystemEntries(path);

            // Order alphabetically so that output is stable across invocations
            Arrays.Sort(entries);

            foreach (var entry in entries)
            {
                var file = Path.Combine(path, entry);

                if (Directory.Exists(file))
                {
                    ProcessDirectory(file, handler, relativePath.Length == 0 ? entry : relativePath + "/" + entry, log);
                }
                else if (handler.ShouldProcess(file))
                {
                    handler.OnBeforeExtraction(file, relativePath, log);

                    // Read metadata
                    Metadata metadata;
                    try
                    {
                        metadata = ImageMetadataReader.ReadMetadata(file);
                    }
                    catch (Exception e)
                    {
                        handler.OnExtractionError(file, e, log);
                        continue;
                    }

                    handler.OnExtractionSuccess(file, metadata, relativePath, log);
                }
            }
        }
    }
}