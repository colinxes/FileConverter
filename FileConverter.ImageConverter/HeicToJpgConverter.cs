using FileConverter.Foundation;
using ImageMagick;

namespace FileConverter.ImageConverter;

public class HeicToJpgConverter : IFileConverter
{
    public string SourceFileEnding => "HEIC";
    public string TargetFileEnding => "JPG";
    
    public void Convert(string pInputFile, string pExportFile)
    {
        using MagickImage image = new MagickImage(pInputFile);
        image.Format = MagickFormat.Jpeg;
        image.Write(pExportFile);
    }
}