using FileConverter.Foundation;

namespace ImageConverter;

public class FileConverterProvider
{
    private IEnumerable<IFileConverter>? _converters;
    private IEnumerable<IFileConverter> Converters => _converters ??= GetConverter();

    private IEnumerable<IFileConverter> GetConverter()
    {
        Type imageConverterType = typeof(IFileConverter);
        
        IEnumerable<Type> imageConverterTypes = imageConverterType.Assembly
            .GetTypes()
            .Where(pType => !pType.IsAbstract
                            && pType.IsClass
                            && imageConverterType.IsAssignableFrom(pType));

        IEnumerable<IFileConverter> fileConverters = imageConverterTypes.Select(pType => (IFileConverter)Activator.CreateInstance(pType)!);

        return fileConverters;
    }

    public IFileConverter ByFileEndings(string pSourceFileEnding, string pTargetFileEnding)
    {
        IFileConverter? fileConverter = Converters.SingleOrDefault(pConverter => string.Equals(pConverter.TargetFileEnding, pTargetFileEnding, StringComparison.CurrentCultureIgnoreCase) 
            && string.Equals(pConverter.SourceFileEnding, pSourceFileEnding, StringComparison.CurrentCultureIgnoreCase));

        return fileConverter ?? throw new NotSupportedException($"No converter found for file ending: {pTargetFileEnding}");
    }
}