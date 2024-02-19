![metadata-extractor logo](https://raw.githubusercontent.com/drewnoakes/metadata-extractor/main/Resources/metadata-extractor-logo.svg)

[![Build Status](https://github.com/drewnoakes/metadata-extractor-dotnet/actions/workflows/CI.yml/badge.svg)](https://github.com/drewnoakes/metadata-extractor-dotnet/actions/)
[![MetadataExtractor NuGet version](https://img.shields.io/nuget/v/MetadataExtractor)](https://www.nuget.org/packages/MetadataExtractor/)
[![MetadataExtractor NuGet download count](https://img.shields.io/nuget/dt/MetadataExtractor)](https://www.nuget.org/packages/MetadataExtractor/)

_MetadataExtractor_ is a straightforward .NET library for reading metadata from image, movie and audio files.

## Installation

The easiest way to use this library is via its [NuGet package](https://www.nuget.org/packages/MetadataExtractor/).

Either add this to your project file

```xml
<ItemGroup>
    <PackageReference Include="MetadataExtractor" Version="2.8.1" />
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
* [Photoshop](https://en.wikipedia.org/wiki/Adobe_Photoshop)

It supports various file types.

* Images
  * [AVIF](https://en.wikipedia.org/wiki/AVIF)
  * [BMP](https://en.wikipedia.org/wiki/BMP_file_format)
  * [DNG](https://en.wikipedia.org/wiki/Digital_Negative)
  * [EPS](https://en.wikipedia.org/wiki/Encapsulated_PostScript)
  * [GIF](http://en.wikipedia.org/wiki/Graphics_Interchange_Format)
  * [HEIF](https://en.wikipedia.org/wiki/High_Efficiency_Image_File_Format) / [HEIC](https://en.wikipedia.org/wiki/High_Efficiency_Image_File_Format#HEIC:_HEVC_in_HEIF)
  * [ICO](https://en.wikipedia.org/wiki/ICO_(file_format))
  * [JPEG](https://en.wikipedia.org/wiki/JPEG) / [JFIF](https://en.wikipedia.org/wiki/JPEG_File_Interchange_Format)
  * [Netpbm](https://en.wikipedia.org/wiki/Netpbm_format)
  * [PCX](http://en.wikipedia.org/wiki/PCX)
  * [PNG](http://en.wikipedia.org/wiki/Portable_Network_Graphics)
  * [PSD]([url](https://en.wikipedia.org/wiki/Adobe_Photoshop#File_format))
  * [TGA](https://en.wikipedia.org/wiki/Truevision_TGA)
  * [TIFF]([url](https://en.wikipedia.org/wiki/TIFF)) / [BigTIFF]([url](https://en.wikipedia.org/wiki/TIFF#BigTIFF))
  * [WebP](http://en.wikipedia.org/wiki/WebP)
  * [Camera Raw](https://en.wikipedia.org/wiki/Raw_image_format)
    * 3FR (Hasselblad)
    * [ARW](https://en.wikipedia.org/wiki/Sony_%CE%B1) (Sony)
    * CRW / CR2 / CRX (Canon)
    * GPR (GoPro)
    * KDC (Kodak)
    * NEF (Nikon)
    * [ORF](https://en.wikipedia.org/wiki/ORF_format) (Olympus)
    * PEF (Pentax)
    * RAF (Fujifilm)
    * RW2 (Panasonic)
    * RWL (Leica)
    * SRW (Samsung)

* Movies
  * [AVCI](https://en.wikipedia.org/wiki/Advanced_Video_Coding)
  * [AVI](https://en.wikipedia.org/wiki/Audio_Video_Interleave)
  * [MOV](https://en.wikipedia.org/wiki/QuickTime_File_Format) (QuickTime)
  * [MP4](https://en.wikipedia.org/wiki/MP4_file_format)

* Audio
  * [WAV](https://en.wikipedia.org/wiki/WAV)
  * [MP3](https://en.wikipedia.org/wiki/MP3)

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
* Ricoh
* Samsung
* Sanyo
* Sigma/Foveon
* Sony

## Supported Frameworks

This library targets:

- .NET 8.0 (`net8.0`)
- .NET Framework 4.6.2 (`net462`)
- .NET Standard 1.3 (`netstandard1.3`)
- .NET Standard 2.1 (`netstandard2.1`)

All target frameworks are provided via the [one NuGet package](https://www.nuget.org/packages/MetadataExtractor).

`net8.0` implements .NET 8, including support for NativeAOT.

`netstandard1.3` implements version 1.3 of the [.NET Standard](https://docs.microsoft.com/en-us/dotnet/articles/standard/library) which covers .NET Core, Mono, Xamarin platforms, UWP, and future platforms. 

`netstandard2.1` implements version 2.1 of the .NET Standard, which uses newer APIs where possible.

`net462` targets the full .NET Framework, from version 4.6.2 onwards.

A PCL build was supported until [version 1.5.3](https://www.nuget.org/packages/MetadataExtractor/1.5.3) which supported Silverlight 5.0, Windows 8.0, Windows Phone 8.1 and Windows Phone Silverlight 8.0. PCL versions did not support file-system metadata due to restricted IO APIs.

A `net3.5` build was supported until [version 2.8.1](https://www.nuget.org/packages/MetadataExtractor/2.8.1). Support for this framework was dropped in early 2024 to enable use of newer, more efficient, .NET APIs.

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
