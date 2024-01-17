using ClosedXML.Excel;

namespace HoroshopWeb.Infrastructure;

public class ExcelService
{
    public MemoryStream Export(IEnumerable<Product> products, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var stream = new MemoryStream();
        
        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Sheet1");
        worksheet.Cell("A1").Value = "Артикул";
        worksheet.Cell("B1").Value = "Цена";
        worksheet.Cell("C1").Value = "Наличие";

        var outputIndex = 2;
        foreach (var product in products)
        {
            cancellationToken.ThrowIfCancellationRequested();

            worksheet.Cell($"A{outputIndex}").Value = product.Sku;
            worksheet.Cell($"B{outputIndex}").Value = product.Price;
            worksheet.Cell($"C{outputIndex}").Value = product.Stock;

            outputIndex++;
        }

        workbook.SaveAs(stream);

        stream.Position = 0;
        return stream;
    }
}
