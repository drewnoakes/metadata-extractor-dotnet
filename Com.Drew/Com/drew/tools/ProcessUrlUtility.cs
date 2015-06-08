/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System;
using Com.Drew.Imaging;
using Com.Drew.Metadata;
using Sharpen;

namespace Com.Drew.Tools
{
    /// <summary>Utility that extracts metadata found at a given URL.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class ProcessUrlUtility
    {
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Com.Drew.Imaging.Jpeg.JpegProcessingException"/>
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.Println("Expects one or more URLs as arguments.");
                Environment.Exit(1);
            }
            foreach (string url in args)
            {
                ProcessUrl(new Uri(url));
            }
            Console.Out.Println("Completed.");
        }

        /// <exception cref="System.IO.IOException"/>
        private static void ProcessUrl(Uri url)
        {
            URLConnection con = url.OpenConnection();
            //        con.setConnectTimeout(connectTimeout);
            //        con.setReadTimeout(readTimeout);
            InputStream @in = con.GetInputStream();
            // Read metadata
            Metadata.Metadata metadata;
            try
            {
                metadata = ImageMetadataReader.ReadMetadata(@in);
            }
            catch (ImageProcessingException e)
            {
                // this is an error in the Jpeg segment structure.  we're looking for bad handling of
                // metadata segments.  in this case, we didn't even get a segment.
                Console.Error.Printf("%s: %s [Error Extracting Metadata]\n\t%s%n", e.GetType().FullName, url, e.Message);
                return;
            }
            catch (Exception t)
            {
                // general, uncaught exception during processing of jpeg segments
                Console.Error.Printf("%s: %s [Error Extracting Metadata]%n", t.GetType().FullName, url);
                Runtime.PrintStackTrace(t, Console.Error);
                return;
            }
            if (metadata.HasErrors())
            {
                Console.Error.Println(url);
                foreach (Directory directory in metadata.GetDirectories())
                {
                    if (!directory.HasErrors())
                    {
                        continue;
                    }
                    foreach (string error in directory.GetErrors())
                    {
                        Console.Error.Printf("\t[%s] %s%n", directory.GetName(), error);
                    }
                }
            }
            // Iterate through all values
            foreach (Directory directory_1 in metadata.GetDirectories())
            {
                foreach (Tag tag in directory_1.GetTags())
                {
                    string tagName = tag.GetTagName();
                    string directoryName = directory_1.GetName();
                    string description = tag.GetDescription();
                    // truncate the description if it's too long
                    if (description != null && description.Length > 1024)
                    {
                        description = Runtime.Substring(description, 0, 1024) + "...";
                    }
                    Console.Out.Printf("[%s] %s = %s%n", directoryName, tagName, description);
                }
            }
        }
        //        if (processedCount > 0)
        //            System.out.println(String.format("Processed %,d files (%,d bytes) with %,d exceptions and %,d file errors in %s", processedCount, byteCount, exceptionCount, errorCount, path));
    }
}
