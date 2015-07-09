![metadata-extractor logo](https://raw.githubusercontent.com/drewnoakes/metadata-extractor/master/Resources/metadata-extractor-logo-500x123.png)

[![Build status](https://ci.appveyor.com/api/projects/status/90hfuleg8wj8r956?svg=true)](https://ci.appveyor.com/project/drewnoakes/metadata-extractor-dotnet)
[![MetadataExtractor NuGet version](https://img.shields.io/nuget/v/MetadataExtractor.svg)](https://www.nuget.org/packages/MetadataExtractor/)
[![MetadataExtractor download stats](https://img.shields.io/nuget/dt/MetadataExtractor.svg)](https://www.nuget.org/packages/MetadataExtractor/)
[![Issue Stats](http://issuestats.com/github/drewnoakes/metadata-extractor-dotnet/badge/issue?style=flat)](http://issuestats.com/github/drewnoakes/metadata-extractor-dotnet)

_MetadataExtractor_ is a straightforward .NET library for reading metadata from image files.

## Installation

The easiest way to reference this project is to install [its NuGet package](https://www.nuget.org/packages/MetadataExtractor/):

    PM> Install-Package MetadataExtractor

## Usage

```csharp
IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(imagePath);
```

The resulting `directories` sequence holds potentially many different directories of metadata, depending upon the input image.

To print out all values from all directories:

```csharp
foreach (var directory in directories)
foreach (var tag in directory.Tags)
{
    Console.Out.WriteLine("{0} - {1} = {2}", directory.Name, tag.TagName, tag.Description);
}
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
var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().SingleOrDefault();
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
* [PNG](http://en.wikipedia.org/wiki/Portable_Network_Graphics) properties
* [BMP](http://en.wikipedia.org/wiki/BMP_file_format) properties
* [GIF](http://en.wikipedia.org/wiki/Graphics_Interchange_Format) properties
* [ICO](https://en.wikipedia.org/wiki/ICO_(file_format)) properties
* [PCX](http://en.wikipedia.org/wiki/PCX) properties

It will process files of type:

* JPEG
* TIFF
* WebP
* PSD
* PNG
* BMP
* GIF
* ICO
* PCX
* Camera Raw
  * NEF (Nikon)
  * CR2 (Canon)
  * ORF (Olympus)
  * ARW (Sony)
  * RW2 (Panasonic)
  * RWL (Leica)
  * SRW (Samsung)

Camera-specific "makernote" data is decoded for cameras manufactured by:

* Agfa
* Canon
* Casio
* Epson
* Fujifilm
* Kodak
* Kyocera
* Leica
* Minolta
* Nikon
* Olympus
* Panasonic
* Pentax
* Sanyo
* Sigma/Foveon
* Sony

## Mailing Lists

* [metadata-extractor-dev](https://groups.google.com/forum/#!forum/metadata-extractor-dev) for discussion about development and notifications of changes to issues and source code
* [metadata-extractor-announce](https://groups.google.com/forum/#!forum/metadata-extractor-announce) for announcements of new releases

## Feedback

Have questions or ideas? Try the [mailing list](http://groups.google.com/group/metadata-extractor-dev) or [open an issue](https://github.com/drewnoakes/metadata-extractor-dotnet/issues). GitHub's issue tracker accepts attachments, and sample images are often crucial in debugging problems.

## Contribute

If you want to get your hands dirty, clone this repository, enhance the library and submit a pull request. Review the issue list and ask around on the mailing list to avoid duplication of work.

An easier way to help is to contribute to the [sample image file library](https://github.com/drewnoakes/metadata-extractor/wiki/ImageDatabase) used for research and testing.

## Credits

This library is developed by [Drew Noakes](https://drewnoakes.com/code/exif/).

Thanks are due to the many [users](https://github.com/drewnoakes/metadata-extractor/wiki/UsedBy) who sent in suggestions, bug reports,
[sample images](https://github.com/drewnoakes/metadata-extractor/wiki/ImageDatabase) from their cameras as well as encouragement.
Wherever possible, they have been credited in the source code and commit logs.

This library was [originally written in Java](https://github.com/drewnoakes/metadata-extractor/) in 2002. In 2014, Yakov Danilov (for Imazen LLC) converted the code to C# using Sharpen. Both projects are now developed in unison and aim to be functionally equivalent.

## License

Copyright 2002-2015 Drew Noakes

> Licensed under the Apache License, Version 2.0 (the "License");
> you may not use this file except in compliance with the License.
> You may obtain a copy of the License at
>
>     http://www.apache.org/licenses/LICENSE-2.0
>
> Unless required by applicable law or agreed to in writing, software
> distributed under the License is distributed on an "AS IS" BASIS,
> WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
> See the License for the specific language governing permissions and
> limitations under the License.

More information about this project is available at:

* https://drewnoakes.com/code/exif/
* https://github.com/drewnoakes/metadata-extractor-dotnet/
