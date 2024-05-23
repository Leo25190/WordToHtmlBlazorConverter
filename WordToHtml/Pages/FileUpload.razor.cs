using BlazorDownloadFile;
using Mammoth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace WordToHtml.Pages;
public partial class FileUpload
{

    [Inject] public IBlazorDownloadFileService DownloadFileService { get; set; }
    [Inject] public ILogger<FileUpload> Logger { get; set; }
    [Inject] public IJSRuntime JSRuntime { get; set; }

    private IBrowserFile uploadedFile;
    private string uploadedFileName;
    private string convertedHtml { get; set; } = string.Empty;
    private string formattedHtml;
    private MarkupString markupHtml;

    private async Task HandleFileSelected(InputFileChangeEventArgs e)
    {
        uploadedFile = e.File;
        uploadedFileName = uploadedFile.Name;

        using var stream = uploadedFile.OpenReadStream();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        try
        {
            var converter = new DocumentConverter().AddStyleMap("p[style-name='Title'] => h1");
            var result = converter.ConvertToHtml(memoryStream); // Use ConvertToHtmlAsync for async processing
            convertedHtml = result.Value; // Converted HTML
            formattedHtml = FormatHtml(convertedHtml); // Formatted HTML
            markupHtml = (MarkupString)formattedHtml;
            StateHasChanged(); // Force the component to re-render
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            convertedHtml = "Erreur lors de la conversion du document";
            StateHasChanged(); // Ensure the error message is displayed
        }
    }

    private string FormatHtml(string html)
    {
        return html.Replace("><", ">\n<");
    }

    private async Task DownloadHtmlFile()
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(formattedHtml);
        var fileWithouthExtension = Path.GetFileNameWithoutExtension(uploadedFileName);
        var fileWithouthExtension2 = $"{fileWithouthExtension} - converted.html";
        await DownloadFileService.DownloadFile(uploadedFileName.Replace(".docx", string.Empty) + " - converted.html", bytes, "text/html");
    }

    private async Task CopyToClipboard()
    {
        if (JSRuntime == null)
        {
            Logger.LogError("JSRuntime is null.");
            return;
        }

        await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", formattedHtml);
        Logger.LogInformation("Copied to clipboard");
    }

    private void OnChange(string html)
    {
        formattedHtml = FormatHtml(html);
        StateHasChanged();
    }
}
