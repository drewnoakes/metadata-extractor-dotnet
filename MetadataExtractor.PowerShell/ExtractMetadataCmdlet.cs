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
