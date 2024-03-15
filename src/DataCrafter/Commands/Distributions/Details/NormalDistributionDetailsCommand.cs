using Spectre.Console;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.Distributions.Details;
internal sealed class NormalDistributionDetailsCommand : AsyncCommand
{
    private const string NormalDistributionPdfUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/7/74/Normal_Distribution_PDF.svg/720px-Normal_Distribution_PDF.svg.png";
    private readonly IAnsiConsole _ansiConsole;

    public NormalDistributionDetailsCommand(IAnsiConsole ansiConsole)
    {
        _ansiConsole = ansiConsole;
    }

    public async Task Test()
    {
        var cacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DataCrafter", "NormalDistributionCache");
        Directory.CreateDirectory(cacheFolder);
        await WriteProbabilityDensityFunction(cacheFolder);
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var cacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DataCrafter", "NormalDistributionCache");
        Directory.CreateDirectory(cacheFolder);
        await WriteProbabilityDensityFunction(cacheFolder);
        return 0;
    }

    private async Task WriteProbabilityDensityFunction(string cacheFolder)
    {
        var imageName = "probability_density_function.png";
        var imagePath = Path.Combine(cacheFolder, imageName);

        if (!File.Exists(imagePath))
            await DownloadImageAsync(NormalDistributionPdfUrl, imagePath);

        var image = new CanvasImage(imagePath).MaxWidth(120).PixelWidth(1).BilinearResampler();

        _ansiConsole.WriteLine();
        _ansiConsole.MarkupLine("Probability Density Function");
        _ansiConsole.Write(image);
    }

    static async Task DownloadImageAsync(string imageUrl, string filePath)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "DataCrafter");
        var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
        await File.WriteAllBytesAsync(filePath, imageBytes);
    }
}
