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
        public override void OnExtractionSuccess(string filePath, IReadOnlyList<Directory> directories, string relativePath, TextWriter log)
        {
            base.OnExtractionSuccess(filePath, directories, relativePath, log);

            // Iterate through all values, calling toString to flush out any formatting exceptions
            foreach (var directory in directories)
            {
                directory.Name.ToString();

                foreach (var tag in directory.Tags)
                {
                    tag.TagName.ToString();
                    (tag.Description ?? "").ToString();
                }
            }
        }
    }
}