using HoroshopWeb.Settings;
using Microsoft.Extensions.Options;
using System.Xml.Linq;

namespace HoroshopWeb.Infrastructure;

public class FileService(IOptions<FileSettings> settings)
{
    public async Task StoreHoroshopFileAsync(IFormFile file, CancellationToken cancellationToken)
    {
        var path = Path.Combine(settings.Value.DataFolder, "horoshop.xlsx");
        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);
    }

    public async Task<Stream?> GetHoroshopFileAsync(CancellationToken cancellationToken)
    {
        var path = Path.Combine(settings.Value.DataFolder, "horoshop.xlsx");
        if (!File.Exists(path))
            return null;

        return await Task.FromResult(new FileStream(path, FileMode.Open));
    }

    public async Task StoreViatecFileAsync(XDocument document, CancellationToken cancellationToken)
    {
        var path = Path.Combine(settings.Value.DataFolder, "viatec.xml");
        await using var stream = new FileStream(path, FileMode.Create);
        await document.SaveAsync(stream, SaveOptions.None, cancellationToken);
    }

    public async Task<XDocument?> GetViatecDocumentAsync(CancellationToken cancellationToken)
    {
        var path = Path.Combine(settings.Value.DataFolder, "viatec.xml");
        if (!File.Exists(path))
            return null;

        await using var stream = new FileStream(path, FileMode.Open);
        return await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);
    }
}
