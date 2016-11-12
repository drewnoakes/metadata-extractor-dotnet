#region License
//
// Copyright 2002-2016 Drew Noakes
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

using System.Linq;
using System.Management.Automation;
using JetBrains.Annotations;

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

            if (Raw)
            {
                var obj = from dir in directories
                    where !dir.IsEmpty
                    from tag in dir.Tags
                    select new
                    {
                        Directory = dir.Name,
                        Tag = tag.Name,
                        RawValue = dir.GetObject(tag.Type)
                    };

                WriteObject(obj.ToList());
            }
            else
            {
                var obj = from dir in directories
                    where !dir.IsEmpty
                    from tag in dir.Tags
                    select new
                    {
                        Directory = dir.Name,
                        Tag = tag.Name,
                        Description = tag.Description
                    };

                WriteObject(obj.ToList());
            }
        }
    }
}
