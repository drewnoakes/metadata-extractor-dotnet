namespace Sharpen
{
    public interface FilenameFilter
    {
        bool Accept (FilePath dir, string name);
    }
}
