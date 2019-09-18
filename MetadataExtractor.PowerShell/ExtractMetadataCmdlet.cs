// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
        public string FilePath { get; set; } = default!;

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
