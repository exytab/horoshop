using System.Xml.Linq;

namespace HoroshopWeb.Extensions;

public static class XDocumentExtensions
{
    public static string GetVersion(this XDocument document)
    {
        var date = document.Root.Element("date").Value;
        var time = document.Root.Element("time").Value;
        var version = $"{date} {time}";
        return version;
    }
}
