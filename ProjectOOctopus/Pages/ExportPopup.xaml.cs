using CommunityToolkit.Maui.Storage;
using Mopups.Pages;
using ProjectOOctopus.Services;

namespace ProjectOOctopus.Pages;

public partial class ExportPopup : PopupPage
{
    private string _exportString;
    private ExcelService _excelService;

    public ExportPopup(ExcelService excelService)
    {
        InitializeComponent();
        _excelService = excelService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        SetTargetPath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
    }

    private async void SelectPathButton_Clicked(object sender, EventArgs e)
    {
        CancellationToken token = new CancellationToken();

        FolderPickerResult result = await FolderPicker.Default.PickAsync(token);
        if (result.IsSuccessful)
        {
            SetTargetPath(result.Folder.Path);
        }
    }

    private void SetTargetPath(string path)
    {
        _exportString = path;
        TargetDirectoryPathLabel.Text = _exportString;
    }

    private async void ExportButton_Clicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_exportString) && Directory.Exists(_exportString))
        {
            await _excelService.Export(_exportString);
        }
    }
}