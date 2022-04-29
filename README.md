![metadata-extractor logo](https://cdn.rawgit.com/drewnoakes/metadata-extractor/master/Resources/metadata-extractor-logo.svg)

[![Build Status](https://dev.azure.com/metadata-extractor/metadata-extractor-dotnet/_apis/build/status/drewnoakes.metadata-extractor-dotnet?branchName=master)](https://dev.azure.com/metadata-extractor/metadata-extractor-dotnet/_build/latest?definitionId=1&branchName=master)
[![MetadataExtractor NuGet version](https://img.shields.io/nuget/v/MetadataExtractor)](https://www.nuget.org/packages/MetadataExtractor/)
[![MetadataExtractor on fuget.org](https://www.fuget.org/packages/MetadataExtractor/badge.svg)](https://www.fuget.org/packages/MetadataExtractor)
[![MetadataExtractor Nuget download count](https://img.shields.io/nuget/dt/MetadataExtractor)](https://www.nuget.org/packages/MetadataExtractor/)

_MetadataExtractor_ is a straightforward .NET library for reading metadata from image, movie and audio files.

## Installation

The easiest way to use this library is via its [NuGet package](https://www.nuget.org/packages/MetadataExtractor/).

Either add this to your project file

```xml
<ItemGroup>
    <PackageReference Include="MetadataExtractor" Version="2.7.2" />
</ItemGroup>
```

Or type this in Visual Studio's Package Manager Console:

```
PM> Install-Package MetadataExtractor
```

Or search for `MetadataExtractor` in the Visual Studio NuGet Package Manager.

## Usage

```csharp
IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(imagePath);
```

The resulting `directories` sequence holds potentially many different directories of metadata, depending upon the input image.

To print out all values from all directories:

```csharp
foreach (var directory in directories)
foreach (var tag in directory.Tags)
    Console.WriteLine($"{directory.Name} - {tag.Name} = {tag.Description}");
```

Producing:

```text
Exif SubIFD - Exposure Time = 1/60 sec
Exif SubIFD - F-Number = f/8.0
...
Exif IFD0 - Make = NIKON CORPORATION
Exif IFD0 - Model = NIKON D70
...
IPTC - Credit = Drew Noakes
IPTC - City = London
...
```

Access a specific value, in this case the Exif DateTime tag:

```csharp
var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
var dateTime = subIfdDirectory?.GetDescription(ExifDirectoryBase.TagDateTime);
```

## Features

The library understands several formats of metadata, many of which may be present in a single image:

* [Exif](http://en.wikipedia.org/wiki/Exchangeable_image_file_format)
* [IPTC](http://en.wikipedia.org/wiki/IPTC)
* [XMP](http://en.wikipedia.org/wiki/Extensible_Metadata_Platform)
* [JFIF / JFXX](http://en.wikipedia.org/wiki/JPEG_File_Interchange_Format)
* [ICC Profiles](http://en.wikipedia.org/wiki/ICC_profile)
* [Photoshop](http://en.wikipedia.org/wiki/Photoshop) fields
* [WebP](http://en.wikipedia.org/wiki/WebP) properties
* [Netpbm](https://en.wikipedia.org/wiki/Netpbm_format) properties
* [PNG](http://en.wikipedia.org/wiki/Portable_Network_Graphics) properties
* [BMP](http://en.wikipedia.org/wiki/BMP_file_format) properties
* [GIF](http://en.wikipedia.org/wiki/Graphics_Interchange_Format) properties
* [ICO](https://en.wikipedia.org/wiki/ICO_(file_format)) properties
* [PCX](http://en.wikipedia.org/wiki/PCX) properties

It supports various file types.

* Images
  * BMP
  * EPS
  * GIF
  * HEIF / HEIC
  * ICO
  * JPEG / JFIF
  * Netpbm
  * PCX
  * PNG
  * PSD
  * TGA
  * TIFF / BigTIFF
  * WebP
  * Camera Raw
    * ARW (Sony)
    * CR2 (Canon)
    * NEF (Nikon)
    * ORF (Olympus)
    * RW2 (Panasonic)
    * RWL (Leica)
    * SRW (Samsung)

* Movies
  * AVCI
  * AVI
  * MOV (QuickTime)
  * MP4

* Audio
  * WAV
  * MP3

Camera-specific "makernote" data is decoded for cameras manufactured by:

* Agfa
* Apple
* Canon
* Casio
* DJI
* Epson
* FLIR
* Fujifilm
* Kodak
* Kyocera
* Leica
* Minolta
* Nikon
* Olympus
* Panasonic
* Pentax
* Reconyx
* Sanyo
* Sigma/Foveon
* Sony

## Supported Frameworks

This library targets:

- .NET 5.0+ (`net5.0`)
- .NET Standard 2.0 (`netstandard2.0`), which supports .NET Framework 4.6.1+ and .NET Core 1.0+  

All target frameworks are provided via the [one NuGet package](https://www.nuget.org/packages/MetadataExtractor).

For older version of .NET Framework and .NET Standard (`net35`, `net45` and `netstandard1.3`) use version 2.x packages.

A PCL build was supported until [version 1.5.3](https://www.nuget.org/packages/MetadataExtractor/1.5.3) which supported Silverlight 5.0, Windows 8.0, Windows Phone 8.1 and Windows Phone Silverlight 8.0. PCL versions did not support file-system metadata due to restricted IO APIs.

## Building

Building this repo requires a recent version of Visual Studio 2022. Ensure you have the _.NET Core Development Tools_ workload installed via the Visual Studio Installer.

The library itself, once built, may be consumed from projects in much earlier versions of Visual Studio.

## Questions & Feedback

The quickest way to have your questions answered is via [Stack Overflow](http://stackoverflow.com/questions/tagged/metadata-extractor).
Check whether your question has already been asked, and if not, ask a new one tagged with both `metadata-extractor` and `.net`.

Bugs and feature requests should be provided via the project's [issue tracker](https://github.com/drewnoakes/metadata-extractor-dotnet/issues).
Please attach sample images where possible as most issues cannot be investigated without an image.

## Contributing

If you want to get your hands dirty, making a pull request is a great way to enhance the library.
In general it's best to create an issue first that captures the problem you want to address.
You can discuss your proposed solution in that issue.
This gives others a chance to provide feedback before you spend your valuable time working on it.

An easier way to help is to contribute to the [sample image file library](https://github.com/drewnoakes/metadata-extractor-images/wiki) used for research and testing.

## Credits

This library is developed by [Drew Noakes](https://drewnoakes.com/code/exif/) and contributors.

Thanks are due to the many [users](https://github.com/drewnoakes/metadata-extractor/wiki/UsedBy) who sent in suggestions, bug reports,
[sample images](https://github.com/drewnoakes/metadata-extractor-images/wiki) from their cameras as well as encouragement.
Wherever possible, they have been credited in the source code and commit logs.

This library was [originally written in Java](https://github.com/drewnoakes/metadata-extractor/) in 2002.
In 2014, Yakov Danilov (for Imazen LLC) converted the code to C# using Sharpen.
The code has subsequently been edited to provide a more idiomatic .NET API.
Both projects are now developed in unison and aim to be functionally equivalent.

## Other languages

- Java  [metadata-extractor](https://github.com/drewnoakes/metadata-extractor) is the original implementation of this project, from which this .NET version was ported
- PHP [php-metadata-extractor](https://github.com/gomoob/php-metadata-extractor) wraps the Java project, making it available to users of PHP

---

More information about this project is available at:

* https://drewnoakes.com/code/exif/
* https://github.com/drewnoakes/metadata-extractor-dotnet/
