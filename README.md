NMetadataExtractor
====================

master: [![Build status](https://ci.appveyor.com/api/projects/status/12bkj9y5wcydqak7/branch/master?svg=true)](https://ci.appveyor.com/project/imazen/n-metadata-extractor/branch/master) most recent commit: [![Build status](https://ci.appveyor.com/api/projects/status/12bkj9y5wcydqak7?svg=true)](https://ci.appveyor.com/project/imazen/n-metadata-extractor) [Download documentation archive](https://ci.appveyor.com/project/imazen/n-metadata-extractor/build/artifacts)


C# port of the [excellent Java MetadataExtractor library by Drew Noakes](https://drewnoakes.com/code/exif/). 


Initial coversion was performed with [Sharpen](https://github.com/imazen/sharpen); details can be found in [HowToConvert.md](HowToConvert.md) and [SharpenBugsAndProblems.md](SharpenBugsAndProblems.md). Significant manual work was required afterward to make the library build and pass all tests correctly.

Conversion was performed against [this copy](https://github.com/ydanila/j-metadata-extractor) of the MetadataExtractor library, which [was reorganized to be amenable to Sharpen conversion](https://github.com/ydanila/j-metadata-extractor/commit/0b6d857dde184bf992a975957521f950ed0e92f6). This copy, in turn, was based on the [Jan 18, 2014 commit eb70b234529a](https://code.google.com/p/metadata-extractor/source/detail?r=eb70b234529ae267c9ba72e9df68d9acb7e3504b). 

Many thanks to Yakov Danilov <yakodani@gmail.com> his work in porting this library from Java to C#. 


Also, special thanks to [Ferret Renaud, who provided a C# port for many years](http://ferretrenaud.fr/Projets/MetaDataExtractor/index.html). His code can be found in the 'renaud' branch. 

All code herein is licensed under one of the following licenses: Apache 2, BSD, MIT. See LICENSE for details about which code is licensed under which license.


### To-do

* Publish to NuGet
* Add documentation (and where possible, re-use documentation from the original).
