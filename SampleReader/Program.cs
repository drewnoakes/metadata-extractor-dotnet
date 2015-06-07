using Com.Drew.Imaging;
using Sharpen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SampleReader
{
    class Program
    {
        static int Main(string[] args)
        {
            var resource = "https://raw.githubusercontent.com/drewnoakes/metadata-extractor-images/master/jpg/Apple%20iPhone%206%20plus.jpg";
            var request = HttpWebRequest.Create(new Uri(resource));
            var exit_code = 0;

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream()){
                var meta = ParseMetadata(stream, resource);
                if (meta != null)
                {
                    var success = PrintMetadata(meta);
                    if (!success)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        //Some metadata bits might be corrupt
                        Console.Error.WriteLine("Some invalid metadata was detected.");
                        exit_code = 2;
                    }
                    else
                    {
                        exit_code = 0; //All peachy!
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    //This might not be an image, or it might be badly corrupted, or unsupported.
                    Console.WriteLine("This is not a valid image, or is not a type supported by MetadataExtractor");
                    exit_code =  1;
                }

            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ReadKey();
            return exit_code;
           

        }
        /// <summary>
        /// Parses the metadata, writing error information to the console if it failes.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static Com.Drew.Metadata.Metadata  ParseMetadata(Stream data, string info){

            try
			{
                if (!data.CanSeek)
                {
                    var ms = new MemoryStream(4096);//We have to guess at length, can't seek
                    data.CopyTo(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    return ImageMetadataReader.ReadMetadata(InputStream.Wrap(ms));
                }
                return ImageMetadataReader.ReadMetadata(InputStream.Wrap(data));
			}
			catch (ImageProcessingException e)
			{
                Console.ForegroundColor = ConsoleColor.Red;
				// this is an error in the Jpeg segment structure.  we're looking for bad handling of
				// metadata segments.  in this case, we didn't even get a segment.
				Console.Error.WriteLine("{0}: {1} [Error Extracting Metadata]\n\t{2}\n", e.GetType().FullName, info, e.Message);
				return null;
			}
			catch (Exception t)
			{
                Console.ForegroundColor = ConsoleColor.Red;
				// general, uncaught exception during processing of jpeg segments
				Console.Error.WriteLine("{0}: {1} [Error Extracting Metadata]\n", t.GetType().FullName, info);
                Console.Error.WriteLine(t);
				return null;
			}
        }

        static bool PrintMetadata(Com.Drew.Metadata.Metadata metadata){
            
            foreach (Com.Drew.Metadata.Directory dir in metadata.GetDirectories())
			{
                if (dir.HasErrors())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    foreach (string error in dir.GetErrors())
                    {
                        Console.Error.WriteLine("\t[{0}] {1}", dir.GetName(), error);
                    }
                }
              
				foreach (Com.Drew.Metadata.Tag tag in dir.GetTags())
				{
					string tagName = tag.GetTagName();
					string directoryName = dir.GetName();
					string description = tag.GetDescription(); 
					// truncate the description if it's too long
					if (description != null && description.Length > 1024)
					{
						description = description.Substring(0,1024)  + "...";
					}
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] {1} = {2}", directoryName, tagName, description);
				}

			}
            
            return metadata.GetDirectories().Any(d => d.HasErrors()); //True if there are any metadata errors;
        }

    }
}
