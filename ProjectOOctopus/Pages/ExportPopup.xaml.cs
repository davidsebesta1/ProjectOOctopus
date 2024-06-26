using CommunityToolkit.Maui.Storage;
using Mopups.Pages;
using ProjectOOctopus.Services;

namespace ProjectOOctopus.Pages;

public partial class ExportPopup : PopupPage
{
    #region Properties

    private string _exportString;
    private ExcelExporterService _excelExporterService;

    #endregion

    #region Ctor

    public ExportPopup(ExcelExporterService excelService)
    {
        InitializeComponent();
        _excelExporterService = excelService;
    }

    #endregion

    #region Page methods and events

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
            await _excelExporterService.Export(_exportString);
        }
    }

    #endregion
}