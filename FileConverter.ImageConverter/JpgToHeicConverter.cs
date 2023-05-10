using FileConverter.Foundation;
using ImageMagick;

namespace FileConverter.ImageConverter;

public class JpgToHeicConverter : IFileConverter
{
    public string SourceFileEnding => "JPG";
    public string TargetFileEnding => "HEIC";
    
    public void Convert(string pInputFile, string pExportFile)
    {
        using var image = new MagickImage(pInputFile);
        image.Format = MagickFormat.Heic;
        image.Write(pExportFile);
    }
}