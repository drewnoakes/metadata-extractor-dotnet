using Com.Drew.Metadata;
using Com.Drew.Metadata.Gif;
using Sharpen;

namespace Com.Drew.Metadata.Gif
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class GifHeaderDescriptor : TagDescriptor<GifHeaderDirectory>
	{
		public GifHeaderDescriptor(GifHeaderDirectory directory)
			: base(directory)
		{
		}
		//    @Override
		//    public String getDescription(int tagType)
		//    {
		//        switch (tagType) {
		//            case GifHeaderDirectory.TAG_COMPRESSION:
		//                return getCompressionDescription();
		//            default:
		//                return super.getDescription(tagType);
		//        }
		//    }
	}
}
