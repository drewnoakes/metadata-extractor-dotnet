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

using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Xmp;

namespace MetadataExtractor.PowerShell
{
    [Cmdlet("Extract", "Metadata")]
    [UsedImplicitly]
    public sealed class ExtractMetadata : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "Path to the file to process")]
        [ValidateNotNullOrEmpty]
        public string FilePath { get; set; }

        [Parameter(HelpMessage = "Show raw value")]
        public SwitchParameter Raw { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            WriteVerbose($"Extracting metadata from file: {FilePath}");

            var directories = ImageMetadataReader.ReadMetadata(FilePath);

            WriteObject(directories.SelectMany(GetDirectoryItems).ToList());
        }

        private IEnumerable<object> GetDirectoryItems(Directory directory)
        {
            // XmpDirectory gets special treatment -- we use the XmpMeta object to list properties
            var xmp = directory as XmpDirectory;

            if (xmp?.XmpMeta != null)
            {
                if (Raw)
                {
                    return xmp.XmpMeta.Properties.Select(prop => new
                    {
                        Directory = directory.Name,
                        Tag = prop.Path,
                        RawValue = (object)prop.Value
                    });
                }
                else
                {
                    return xmp.XmpMeta.Properties.Select(prop => new
                    {
                        Directory = directory.Name,
                        Tag = prop.Path,
                        Description = prop.Value
                    });
                }
            }

            if (Raw)
            {
                return directory.Tags.Select(tag => new
                {
                    Directory = directory.Name,
                    Tag = tag.Name,
                    RawValue = directory.GetObject(tag.Type)
                });
            }
            else
            {
                return directory.Tags.Select(tag => new
                {
                    Directory = directory.Name,
                    Tag = tag.Name,
                    Description = tag.Description
                });
            }
        }
    }
}
