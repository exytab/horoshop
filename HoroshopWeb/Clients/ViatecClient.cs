using System.Xml.Linq;

namespace HoroshopWeb.Clients;

public class ViatecClient(HttpClient httpClient)
{
    const string URL = "https://viatec.ua/files/product_info_uk.xml";

    public async Task<XDocument> GetDocumentAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(URL, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return XDocument.Parse(content);
    }
}
