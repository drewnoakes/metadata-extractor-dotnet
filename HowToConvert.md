Most of the library source was automatically converted from Java to C# using Sharpen plugin. Here I describe how to setup environment and to convert this or any other project.

More detailed guide about Sharpen you can find here http://pauldb.tumblr.com/post/14916717048/a-guide-to-sharpen-a-great-tool-for-converting-java .

##Preparing environment

Sharpen is Eclipse plugin written in Java and it may seem strange, but first we need to setup Java and Eclipse. 

As Sharpen plugin isn't maintained now it hasn't support for the latest Java version. Official release isn't supports even Java 1.6, but we will use port for the Java 1.6 . Latest release for the Java 1.6 could be found in archive http://www.oracle.com/technetwork/java/javase/downloads/java-archive-downloads-javase6-419409.html .

Also Sharpen has some incompatibilities with the latest Eclipse versions. So prefer to use Eclipse 4.2 "Juno" which released in 2012 when Sharpen has last significant activity. Latest "Juno" release could be found here http://www.eclipse.org/downloads/packages/release/juno/sr2. Better to use version for the Java EE developers.

##Building Sharpen

Original Sharpen source located here https://source.db4o.com/db4o/trunk/sharpen/. But prefer to use https://github.com/slluis/sharpen which is compatible with 1.6 or my fork of this project which has some bugfixes https://github.com/ydanila/sharpen. Other problems and bugs found in Sharpen decribed in [SharpenBugsAndProblems.md](SharpenBugsAndProblems.md).

Clone one of the repositories. Statr Eclipse and open Sharpen projects via File->Import. 

To build and install Sharpen plugin go to File->Export and select "Deployable plug-ins and fragments". Select at least sharpen.core, choose plugins installation directory and install them. I had problem here as Eclipse ignored just built Sharpen plugins which was installed to the plugins directory (as recommended in all manuals). Plroblem solved via installation to the ECLIPSE_HOME/**dropins**/ directory.

##Using Sharpen

To call Sharpen functionality we need to add some configuration files to the source project. Example config link could be found here http://pauldb.tumblr.com/post/14916717048/a-guide-to-sharpen-a-great-tool-for-converting-java or my config files from [_sharpen_config](/_sharpen_config).

Before start with Metadata Extractor first we need to convert on of its dependencies - Adobe Xmp Core library. Source located here https://github.com/imazen/n-metadata-extractor/tree/xmp-core or here you can get source with Eclipse project https://github.com/ydanila/n-metadata-extractor/tree/xmp-core and Sharpen configuration files.

Clone. Import to Eclipse. Create empty "sharpen" folder in imported project. Add Sharpen config files if not exists.

Run Sharpen - right click on run-sharpen.xml and select Run As -> Ant Build. After ant build finish you will find converted sources in sharpen/sharpen.net/src.

Remember that default Sharpen config has some problems, decribed in [SharpenBugsAndProblems.md](SharpenBugsAndProblems.md), so better to use my configuration **MEConfiguration** or create your own. Now autoconverted source could be copied to manually created Visual studio project.

Same way convert sources from the https://code.google.com/p/metadata-extractor or from https://github.com/ydanila/j-metadata-extractor . Second contains metadata sources splited into two projects: library and tests and includes Sharpen configuration.

To reduce follow work also add Sharpen project from the [NGit project](https://github.com/mono/ngit) or use modified and extended Sharpen project sources from current repository.

Autoconverted sources and final results you can find in current repository.

##Remarks

Sharpen doesn't gives from the box result which could be used immediately. Converting is iterative process where with each step you can find new parts of code which could be replaced with native .Net calls or your own wrappers. This can take long time.

Sometimes this will require writing custom configuration as Sharpen doesn't allows to override all behaviour and convertion options this options file.