using System.IO;
using Com.Adobe.Xmp;
using Com.Adobe.Xmp.Options;
using Sharpen;

namespace Com.Drew.Metadata.Xmp
{
	public class XmpWriter
	{
		/// <summary>Serializes the XmpDirectory component of <code>Metadata</code> into an <code>OutputStream</code></summary>
		/// <param name="os">Destination for the xmp data</param>
		/// <param name="data">populated metadata</param>
		/// <returns>serialize success</returns>
		public static bool Write(OutputStream os, Com.Drew.Metadata.Metadata data)
		{
			XmpDirectory dir = data.GetFirstDirectoryOfType<XmpDirectory>();
			if (dir == null)
			{
				return false;
			}
			XMPMeta meta = dir.GetXMPMeta();
			try
			{
				SerializeOptions so = new SerializeOptions().SetOmitPacketWrapper(true);
				XMPMetaFactory.Serialize(meta, os, so);
			}
			catch (XMPException e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
				return false;
			}
			return true;
		}
	}
}
