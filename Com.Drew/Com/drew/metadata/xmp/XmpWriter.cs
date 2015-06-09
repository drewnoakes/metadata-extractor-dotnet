using System.IO;
using Com.Adobe.Xmp;
using Com.Adobe.Xmp.Options;
using Sharpen;

namespace Com.Drew.Metadata.Xmp
{
    public class XmpWriter
    {
        /// <summary>Serializes the XmpDirectory component of <c>Metadata</c> into an <c>OutputStream</c></summary>
        /// <param name="stream">Destination for the xmp data</param>
        /// <param name="data">populated metadata</param>
        /// <returns>serialize success</returns>
        public static bool Write(Stream stream, Metadata data)
        {
            XmpDirectory dir = data.GetFirstDirectoryOfType<XmpDirectory>();
            if (dir == null)
            {
                return false;
            }
            IXmpMeta meta = dir.GetXmpMeta();
            try
            {
                SerializeOptions so = new SerializeOptions();
                so.OmitPacketWrapper = true;
                XmpMetaFactory.Serialize(meta, stream, so);
            }
            catch (XmpException e)
            {
                Runtime.PrintStackTrace(e);
                return false;
            }
            return true;
        }
    }
}
