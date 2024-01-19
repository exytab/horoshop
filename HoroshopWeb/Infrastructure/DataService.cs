using ClosedXML.Excel;

namespace HoroshopWeb.Infrastructure;

public class DataService(FileService fileService)
{
    public async Task<Product[]> GetProductsAsync(CancellationToken cancellationToken)
    {
        var viatecDocument = await fileService.GetViatecDocumentAsync(cancellationToken);
        if (viatecDocument is null)
            throw new Exception("Viatec document is null");

        var horoshopStream = await fileService.GetHoroshopFileAsync(cancellationToken);
        if (horoshopStream is null)
            throw new Exception("Horoshop stream is null");

        var products = viatecDocument.Descendants("product");
        var productBySku = products
            .Where(p => p.Element("code") != null && p.Element("code")!.Value != "00-00000000")
            .GroupBy(p => p.Element("code")!.Value)
            .ToDictionary(g => g.Key, g => g.First());
        using var inputWorkbook = new XLWorkbook(horoshopStream);
        var inputsheet = inputWorkbook.Worksheet(1);
        var codeIndex = "C";
        var skuIndex = "A";
        
        var result = new List<Product>();
        var count = 2000;

        var nonViatecPrefix = "R_";

        for (var inputIndex = 2; inputIndex <= count; inputIndex++)
        {
            var nSku = inputsheet.Cell($"{codeIndex}{inputIndex}").Value.ToString();
            if (string.IsNullOrWhiteSpace(nSku))
                continue;

            if (nSku.StartsWith(nonViatecPrefix))
                continue;

            if (!productBySku.ContainsKey(nSku))
            {
                continue;
            }

            var product = productBySku[nSku];

            var oldSku = inputsheet.Cell($"{skuIndex}{inputIndex}").Value.ToString();

            var newPrice = product.Element("price_uah").Value;
            var newStock = product.Element("stock").Value;

            result.Add(new Product(
                Sku: oldSku,
                Price: decimal.Parse(newPrice),
                Stock: toQty(newStock)
            ));
        }

        horoshopStream.Dispose();

        return result.ToArray();
    }

    string toQty(string stock)
    {
        switch (stock)
        {
            case "no":
                return "Немає в наявності";
            case "yes":
                return "В наявності";
            case "few":
                return "В наявності";
            default:
                {
                    throw new Exception($"Unknown stock: {stock}");
                }
        }
    }
}

public record Product(string Sku, decimal Price, string Stock);
