namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    internal static class TypeStringConverter
    {
        public static uint ToCode(string s)
        {
            uint ret = 0;
            for (int i = 0; i < 4; i++)
            {
                ret = ret << 8;
                ret |= s[i];
            }

            return ret;
        }

        public static string ToTypeString(uint input)
        {
            return string.Concat(MakeChar(input >> 24), MakeChar(input >> 16), MakeChar(input >> 8), MakeChar(input));
        }

        private static char MakeChar(uint v)
        {
            return (char)(v & 0xFF);
        }
    }
}
