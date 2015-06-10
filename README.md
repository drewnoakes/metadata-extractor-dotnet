NMetadataExtractor
====================

    using MetadataExtractor;

    Metadata metadata = new ImageMetadataExtractor().Extract(filePath);

MetadataExtractor.Formats.Png
MetadataExtractor.Formats.Bpm
MetadataExtractor.Formats.Exif
MetadataExtractor.Util (PhotographicConversions, FileType, FileTypeDetector)

class StreamCapabilities
{
    bool AcceptsStream(Stream stream);
    
    /// <summary>Returns a stream that has the capabilities required for this reader.</summary>
    /// <remarks>Note that the returned stream need not be disposed, but the source stream still does.</summary>
    Stream Decorate(Stream source);
}

interface IMetadataReader
{
    StreamCapabilities StreamRequirements { get; }

    IEnumerable<Directory> Read(Stream stream);
}

interface IFileMetadataReader
{
    IEnumerable<byte[]> MagicNumbers { get; }
}



public static class MetadataReaderExtension
{
    public static IEnumerable<Directory> Read(this IMetadataReader reader, string filePath)
    {
        using (var stream = new FileStream(filePath, FileMode.Open))
             return reader.Read(reader.StreamRequirements.Decorate(stream));
    }
}


master: [![Build status](https://ci.appveyor.com/api/projects/status/12bkj9y5wcydqak7/branch/master?svg=true)](https://ci.appveyor.com/project/imazen/n-metadata-extractor/branch/master) most recent commit: [![Build status](https://ci.appveyor.com/api/projects/status/12bkj9y5wcydqak7?svg=true)](https://ci.appveyor.com/project/imazen/n-metadata-extractor) [Download documentation archive](https://ci.appveyor.com/project/imazen/n-metadata-extractor/build/artifacts)


C# port of the [excellent Java MetadataExtractor library by Drew Noakes](https://drewnoakes.com/code/exif/). 




Initial coversion was performed with [Sharpen](https://github.com/imazen/sharpen); details can be found in [HowToConvert.md](HowToConvert.md) and [SharpenBugsAndProblems.md](SharpenBugsAndProblems.md). Significant manual work was required afterward to make the library build and pass all tests correctly.

Conversion was performed against [this copy](https://github.com/ydanila/j-metadata-extractor) of the MetadataExtractor library, which [was reorganized to be amenable to Sharpen conversion](https://github.com/ydanila/j-metadata-extractor/commit/0b6d857dde184bf992a975957521f950ed0e92f6). This copy, in turn, was based on the [Jan 18, 2014 commit eb70b234529a](https://code.google.com/p/metadata-extractor/source/detail?r=eb70b234529ae267c9ba72e9df68d9acb7e3504b). 

Many thanks to Yakov Danilov <yakodani@gmail.com> his work in porting this library from Java to C#. 


Also, special thanks to [Ferret Renaud, who provided a C# port for many years](http://ferretrenaud.fr/Projets/MetaDataExtractor/index.html). His code can be found in the 'renaud' branch. 

All code herein is licensed under one of the following licenses: Apache 2, BSD, MIT. See LICENSE for details about which code is licensed under which license. 

ICSharpCode.SharpZipLib is licensed under the GPL with a linking exception, so it can be used in commerical products. 


### To-do

* Publish to NuGet
* Add documentation (and where possible, re-use documentation from the original).

## Example use

Autocoverted examples: 

https://github.com/imazen/n-metadata-extractor/blob/master/Com.Drew/Com/drew/tools/ProcessAllImagesInFolderUtility.cs

https://github.com/imazen/n-metadata-extractor/blob/master/Com.Drew/Com/drew/tools/ProcessUrlUtility.cs

Sample project "SampleReader"


### Basic usage

```

var meta = ImageMetadataReader.ReadMetadata(InputStream.Wrap(stream)); //Stream must be seekable

//Produce a list of strings containing metadata key/value pairs. 
var strings = metadata.GetDirectories().SelectMany(d => d.GetTags().Select(t => String.Format("[{0}] {1} = {2}\n",d.GetName(), t.GetTagName(), t.GetDescription()))).ToList();
```