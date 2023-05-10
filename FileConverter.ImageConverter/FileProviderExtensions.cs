using FileConverter.Foundation;

namespace FileConverter.ImageConverter;

public static class FileProviderExtensions
{
    public static void AddImageConverters(this IFileConverterProvider pProvider) 
        => pProvider.AddFromAssembly(typeof(FileProviderExtensions).Assembly);
}