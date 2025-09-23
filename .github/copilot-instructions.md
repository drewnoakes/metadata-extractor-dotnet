# MetadataExtractor .NET Library

MetadataExtractor is a .NET library for reading metadata from image, movie and audio files. It extracts Exif, IPTC, XMP, ICC, Photoshop, WebP, PNG, BMP, GIF, ICO, PCX metadata from JPEG, TIFF WebP, PSD, PNG, BMP, GIF, ICO, PCX and camera RAW files.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Prerequisites and Environment Setup
- Ensure .NET 8.0 SDK is installed (`dotnet --version` should show 8.0.x)
- The repository is a standard .NET solution with multiple projects
- Use absolute paths when working with the repository in `/home/runner/work/metadata-extractor-dotnet/metadata-extractor-dotnet`

### Bootstrap and Build Process
- **Package restore**: `dotnet restore MetadataExtractor.sln` -- takes ~3 seconds
- **Build Release mode**: `dotnet build MetadataExtractor.sln --configuration Release` -- takes ~7 seconds when cached. NEVER CANCEL. Set timeout to 45+ minutes.
- **Build Debug mode**: `dotnet build MetadataExtractor.sln --configuration Debug` -- takes ~7 seconds when cached. NEVER CANCEL. Set timeout to 30+ minutes.
- **Run unit tests**: `dotnet test --verbosity normal --configuration Release --no-build -f net8.0 MetadataExtractor.Tests/MetadataExtractor.Tests.csproj` -- takes ~2 seconds, expect 483+ passing tests. NEVER CANCEL. Set timeout to 15+ minutes.

### Known Issues and Workarounds
- **.NET Framework targets**: Cannot run net462/net472/net48 projects on Linux without Mono. Use net8.0 targets instead
- **Benchmark execution**: Benchmarks require test data files and may fail if paths are incorrect

### Testing and Validation Scenarios
Always run these validation steps after making changes:
1. **Build validation**: `dotnet build MetadataExtractor.sln --configuration Release && dotnet build MetadataExtractor.sln --configuration Debug`
2. **Unit test validation**: `dotnet test --verbosity normal --configuration Release --no-build -f net8.0 MetadataExtractor.Tests/MetadataExtractor.Tests.csproj`
3. **Tool functionality test**: 
   ```bash
   dotnet run --project MetadataExtractor.Tools.FileProcessor/MetadataExtractor.Tools.FileProcessor.csproj --configuration Release --framework net8.0 -- MetadataExtractor.Tests/Data/withIptc.jpg
   ```
   Expect output showing extracted metadata including JPEG, JFIF, Photoshop, IPTC, Adobe JPEG sections
4. **NativeAOT publishing test**: 
   ```bash
   dotnet publish --verbosity normal --configuration Release -f net8.0 MetadataExtractor.Tools.FileProcessor/MetadataExtractor.Tools.FileProcessor.csproj
   ./MetadataExtractor.Tools.FileProcessor/bin/Release/net8.0/linux-x64/publish/MetadataExtractor.Tools.FileProcessor MetadataExtractor.Tests/Data/withIptc.jpg
   ```
   Takes ~22 seconds. NEVER CANCEL. Set timeout to 45+ minutes.

### Development and Debugging
- **Code style**: Follow `.editorconfig` rules - 4 spaces, PascalCase for public members, camelCase with `_` prefix for fields
- **File header**: All C# files must start with the copyright header as defined in `.editorconfig`
- **Target frameworks**: Library targets net8.0, netstandard2.0, netstandard2.1. Tests target net8.0 and net472
- **Core library structure**: Main code in `MetadataExtractor/` with format-specific readers in `MetadataExtractor/Formats/` (Adobe, Exif, IPTC, JPEG, PNG, TIFF, XMP, etc.)

## Project Structure and Key Locations

### Main Projects
- **MetadataExtractor**: Core library with metadata readers for 30+ file formats
- **MetadataExtractor.Tests**: Comprehensive test suite with test data in `Data/` folder
- **MetadataExtractor.Tools.FileProcessor**: CLI tool for metadata extraction
- **MetadataExtractor.Tools.JpegSegmentExtractor**: CLI tool for JPEG segment extraction
- **MetadataExtractor.Benchmarks**: Performance benchmarking (requires test data setup)
- **MetadataExtractor.PowerShell**: PowerShell module
- **MetadataExtractor.Samples**: Sample usage code (targets .NET Framework only)

### Important Directories
- **MetadataExtractor/Formats/**: Format-specific metadata readers (Exif, IPTC, XMP, JPEG, PNG, TIFF, etc.)
- **MetadataExtractor.Tests/Data/**: Test images and data files for validation
- **wiki/**: Extensive documentation and specifications
- **.github/workflows/**: CI/CD pipeline definitions

### Common Commands Reference
```bash
# Complete build and test cycle
dotnet restore MetadataExtractor.sln
dotnet build MetadataExtractor.sln --configuration Release
dotnet test --verbosity normal --configuration Release --no-build -f net8.0 MetadataExtractor.Tests/MetadataExtractor.Tests.csproj

# Extract metadata from an image
dotnet run --project MetadataExtractor.Tools.FileProcessor --configuration Release --framework net8.0 -- [image_file] [--markdown] [--hex]

# Run benchmarks (if test data is available)
cd MetadataExtractor.Benchmarks && dotnet run -c Release

# Publish NativeAOT executable
dotnet publish --configuration Release -f net8.0 MetadataExtractor.Tools.FileProcessor/MetadataExtractor.Tools.FileProcessor.csproj
```

### Expected Timing and Performance
- **Restore**: ~3 seconds
- **Build Release**: ~7 seconds (when cached), ~20 seconds (first time)
- **Build Debug**: ~7 seconds (when cached), ~14 seconds (first time)
- **Unit tests**: ~2 seconds (483+ tests)
- **NativeAOT publish**: ~22 seconds
- **Metadata extraction**: <1 second per typical image file

### Manual Testing Scenarios
After making changes, always verify:
1. **Library functionality**: Extract metadata from various image formats in test data
2. **Tool usability**: Use FileProcessor tool with different options (--markdown, --hex)
3. **Performance**: Ensure metadata extraction completes in reasonable time
4. **Cross-format support**: Test with JPEG, PNG, TIFF, and RAW files if available

Always run the complete validation cycle before committing changes. The library should extract comprehensive metadata including camera settings, GPS coordinates, color profiles, and embedded thumbnails from supported image files.