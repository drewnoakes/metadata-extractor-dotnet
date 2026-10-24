# MetadataExtractor .NET Library

MetadataExtractor is a .NET library for reading metadata from image, movie and audio files. It extracts Exif, IPTC, XMP, ICC, Photoshop, WebP, PNG, BMP, GIF, ICO, PCX metadata from JPEG, TIFF WebP, PSD, PNG, BMP, GIF, ICO, PCX and camera RAW files.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Prerequisites and Environment Setup
- Ensure .NET 8.0 SDK is installed (`dotnet --version` should show 8.0.x)
- The repository is a standard .NET solution with multiple projects
- Use absolute paths when working with the repository in `/home/runner/work/metadata-extractor-dotnet/metadata-extractor-dotnet`

### Bootstrap and Build Process
- **Package restore**: `dotnet restore MetadataExtractor.slnx` -- takes ~3 seconds
- **Build Release mode**: `dotnet build MetadataExtractor.slnx --configuration Release` -- takes ~7 seconds when cached. NEVER CANCEL. Set timeout to 45+ minutes.
- **Build Debug mode**: `dotnet build MetadataExtractor.slnx --configuration Debug` -- takes ~7 seconds when cached. NEVER CANCEL. Set timeout to 30+ minutes.
- **Run unit tests**: `dotnet test --verbosity normal --configuration Release --no-build -f net8.0 MetadataExtractor.Tests/MetadataExtractor.Tests.csproj` -- takes ~2 seconds, expect 483+ passing tests. NEVER CANCEL. Set timeout to 15+ minutes.

### Known Issues and Workarounds
- **.NET Framework targets**: Cannot run net462/net472/net48 projects on Linux without Mono. Use net8.0 targets instead
- **Benchmark execution**: Benchmarks require test data files and may fail if paths are incorrect

### Testing and Validation Scenarios
Always run these validation steps after making changes:
1. **Build validation**: `dotnet build MetadataExtractor.slnx --configuration Release && dotnet build MetadataExtractor.slnx --configuration Debug`
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

### Writing Unit Tests
- Prefer `[Theory]` with `[CombinatorialData]` for independent test inputs. Boolean parameters automatically cover both values; do not enumerate them with `[InlineData(false)]` and `[InlineData(true)]`.
- Use `[CombinatorialValues(...)]` or `[CombinatorialRange(...)]` on parameters to select other input values.
- Keep `[InlineData]` for correlated input/expected-output rows where a Cartesian product would introduce invalid combinations.

### Descriptor Class Design Pattern
When implementing descriptor classes for metadata directories, follow this established pattern:

1. **Static Import**: Use static import of the directory class to reduce repetition and improve readability:
   ```csharp
   using static MetadataExtractor.Formats.Exif.Makernotes.YourMakernoteDirectory;
   ```

2. **Individual Get Methods**: Create individual `Get*Description()` methods for each tag that return `string?`. These methods should have semantically meaningful names based on the tag purpose. Always use `TryGet*` methods to handle missing tags:
   ```csharp
   public string? GetContrastDescription()
   {
       return Directory.TryGetUInt16(TagContrast, out var value) ? value.ToString() : null;
   }
   
   public string? GetFlashDescription()
   {
       return GetIndexedDescription(TagFlash, "Off", "On");
   }
   ```

3. **Main GetDescription Method**: The overridden `GetDescription(int tagType)` method should use a switch expression to delegate to the individual methods:
   ```csharp
   public override string? GetDescription(int tagType)
   {
       return tagType switch
       {
           TagContrast => GetContrastDescription(),
           TagFlash => GetFlashDescription(),
           _ => base.GetDescription(tagType)
       };
   }
   ```

4. **Method Benefits**: This pattern provides both semantic access (`descriptor.GetContrastDescription()`) and general access (`descriptor.GetDescription(tagId)`) for users of the library.

5. **Return Types**: All descriptor methods must return `string?` to maintain API consistency.

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
dotnet restore MetadataExtractor.slnx
dotnet build MetadataExtractor.slnx --configuration Release
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

### Related repositories and shared guidance

The language implementations and sample corpus normally live in sibling checkouts,
not as submodules of one another:

| Checkout | Repository | Agent guidance |
| --- | --- | --- |
| `metadata-extractor-dotnet` | [C#/.NET implementation](https://github.com/drewnoakes/metadata-extractor-dotnet) | `.github/copilot-instructions.md` |
| `metadata-extractor` | [Java implementation](https://github.com/drewnoakes/metadata-extractor) | `.github/copilot-instructions.md` |
| `metadata-extractor-images` | [Shared sample corpus](https://github.com/drewnoakes/metadata-extractor-images) | `AGENTS.md` |

- Choose checkout/worktree paths explicitly. A sibling's default checkout is not
  necessarily the revision being investigated. Check its status and guidance before
  using it; do not switch, reset, clean, or overwrite another session's work.
- Before changing a format reader, tag directory, or descriptor, inspect the
  corresponding Java code and tests for prior art, fixes and edge cases. Java
  format implementations are in `Source/com/drew/metadata/`, TIFF parsing is in
  `Source/com/drew/imaging/tiff/`, and tests are in `Tests/com/drew/`.
- Compare the same sample bytes when investigating cross-language differences.
  Neither implementation is automatically the oracle. Verify against the format
  specification and preserve each language's API and intentional behavior.
  Identify related fixes/test cases worth porting without mechanically copying
  whole files or expanding the requested scope.
- Keep sibling discovery, corpus safety, and regression-comparison guidance aligned
  across the three repositories. The images repo's `AGENTS.md` owns detailed corpus
  procedures; link to it rather than maintaining divergent copies. Keep build
  commands, test frameworks, API compatibility rules and code style specific to
  each language repository. Cross-repo discovery is not permission to commit or
  publish changes in another repository.

### Sample media corpus

- Use [drewnoakes/metadata-extractor-images](https://github.com/drewnoakes/metadata-extractor-images)
  for real-world samples and broader regression checks, in addition to the small
  `MetadataExtractor.Tests/Data/` fixtures.
- The corpus is multi-gigabyte (over 2 GB). Cloning or copying it can consume
  substantial disk space and bandwidth. Reuse an existing checkout and avoid
  duplicating the entire corpus for each worktree or validation run; use a focused
  subset or isolated input-only hard links where appropriate.
- Prefer a **sibling checkout**, not a submodule of this repository. For example,
  keep `metadata-extractor-dotnet` and `metadata-extractor-images` under the same
  parent directory. Reuse an existing checkout; clone a missing one only with
  approval. Do not reset, clean, or update someone else's checkout.
- Read the corpus's [`AGENTS.md`](https://github.com/drewnoakes/metadata-extractor-images/blob/main/AGENTS.md)
  and [`README.md`](https://github.com/drewnoakes/metadata-extractor-images/blob/main/README.md),
  plus the linked [working-with-test-images guide](https://github.com/drewnoakes/metadata-extractor/wiki/Working-with-test-images),
  before using its generators.
- Start with the affected format folder, such as `dng/`, and representative
  unrelated controls. Run this worktree's `MetadataExtractor.Tools.FileProcessor`
  against those files for read-only checks. A worktree may not be adjacent to the
  corpus: choose both checkout paths explicitly and ensure the binary being tested
  comes from the active worktree, not a different sibling checkout.
- Corpus tooling lives in
  [`src/dotnet/MetadataExtractor.MediaLibraryProcessor/`](https://github.com/drewnoakes/metadata-extractor-images/tree/main/src/dotnet/MetadataExtractor.MediaLibraryProcessor):
  `Program.cs` selects the runners, `DotNet/DotNetRunner.cs` performs extraction,
  `DotNet/TextFileOutputHandler.cs` writes snapshots, and `DiffRunner.cs` compares
  Java with .NET output. Inspect their current paths, project references and
  enabled runners before executing them.
- Do not regenerate the corpus's tracked `metadata/dotnet`, `metadata/java`, or
  `metadata/diff` outputs for a smoke test. Use fresh isolated trees for generation
  and keep logs, manifests and baseline-versus-change comparisons in session
  artifacts. Java-versus-.NET diffs and historical snapshots are useful references,
  not substitutes for a fresh same-runtime baseline. Inspect extraction errors
  and output changes; a zero process exit code alone does not establish success.

Always run the complete validation cycle before committing changes. The library should extract comprehensive metadata including camera settings, GPS coordinates, color profiles, and embedded thumbnails from supported image files.

---

<dotnet-agent>
# .NET MCP Tool usage

When working on .NET projects, *ALWAYS* prefer to use MCP tools that are dedicated to processing .NET code. This includes:

- `search-package-docs`: Use this before implementing any C# code that uses libraries to check you know the correct latest APIs.
- `find-symbols`: Use this when exploring the codebase to enumerate classes, methods, properties, etc., use the `FindSymbols` tool. This is faster and more accurate than reading C# source code.
- `find-all-references`: Use this to instantly locate all references to a class/method/property/etc.
- `rename-symbol`: Use this to rename classes, properties, methods, etc. This is faster and more accurate than directly writing to C# source code.
- `get-symbol-definition`: Use this to look up the definition of a class, method, property, etc., either from sources or referenced libraries.
- `get-generated-source-file-names`, `get-generated-source-file-content`: Use these to locate and read source generator outputs, since that code will not necessarily exist as files on disk.
- `fix-errors`: Use this tool to fix errors and warnings.
- `list-errors`: Use this tool to find out what warnings and errors exist in the solution. This is faster than running a build manually.
- `find-all-references`: Use this to find all references to a class, method, property, local variables, etc. Can trigger from location of either target symbol declaration or one of its references. This is faster and more accurate than reading C# source code.
- `add-member`, `update-member`: Use these when adding or modifying the source code for methods/properties.

Tool calling strategy:
- Check for relevant docs before deciding what to do.
- Before passing any solution path to these tools, be sure that file really exists on disk (and actually verify this).
- First think about how you can chain together these tools to achieve your overall goal. Often the output from `find-symbols` can be used with other tools.

Confidence:
- When you use these tools, the results will be correct. Do not waste time validating the results. For example, the `rename-symbol` tool will correctly update references in strongly-typed .NET code, and you do not need to verify that.
</dotnet-agent>
