using System;
using System.IO;
using Com.Adobe.Xmp;
using Com.Adobe.Xmp.Options;

namespace MetadataExtractor.Formats.Xmp
{
    public class XmpWriter
    {
        /// <summary>Serializes the XmpDirectory component of <c>Metadata</c> into an <c>OutputStream</c></summary>
        /// <param name="stream">Destination for the xmp data</param>
        /// <param name="data">populated metadata</param>
        /// <returns>serialize success</returns>
        public static bool Write(Stream stream, Metadata data)
        {
            var dir = data.GetFirstDirectoryOfType<XmpDirectory>();
            if (dir == null)
            {
                return false;
            }
            var meta = dir.GetXmpMeta();
            try
            {
                XmpMetaFactory.Serialize(meta, stream, new SerializeOptions { OmitPacketWrapper = true });
            }
            catch (XmpException e)
            {
                Console.WriteLine (e);
                return false;
            }
            return true;
        }
    }
}
