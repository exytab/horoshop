using HoroshopWeb.Clients;
using HoroshopWeb.HostedServices;
using HoroshopWeb.Infrastructure;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<ViatecClient>();
builder.Services.AddScoped<DataService>();
builder.Services.AddScoped<ExcelService>();
builder.Services.AddSingleton<FileService>();

builder.Services.AddHostedService<ViatecHostedService>();

var app = builder.Build();

app.MapPost("/upload", async (
    IFormFile file,
    [FromServices] FileService fileService,
    CancellationToken cancellationToken
) =>
{
    await fileService.StoreHoroshopFileAsync(file, cancellationToken);
    return Results.Ok();
}).DisableAntiforgery();

app.MapGet("/download", async (
    [FromServices] ExcelService excelService,
    [FromServices] DataService dataService,
    CancellationToken cancellationToken) =>
{
    var products = await dataService.GetProductsAsync(cancellationToken);
    var memoryStream = excelService.Export(products, cancellationToken);
    var filename = $"{DateTime.Now:yyyy-MM-dd HH-mm-ss}.xlsx";
    return Results.File(memoryStream,
        contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        fileDownloadName: filename);
}).DisableAntiforgery();

app.Run();
