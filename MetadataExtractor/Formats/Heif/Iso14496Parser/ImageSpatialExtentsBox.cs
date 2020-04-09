using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    public class ImageSpatialExtentsBox : FullBox{
        public uint X {get;}
        public uint Y {get;}
	
        public ImageSpatialExtentsBox(BoxLocation loc, SequentialReader sr):base(loc,sr){
            X = sr.GetUInt32();
            Y = sr.GetUInt32();
        }
    }
}