using CommunityToolkit.Maui.Storage;
using Mopups.Pages;
using ProjectOOctopus.Services;

namespace ProjectOOctopus.Pages;

public partial class ImportPopup : PopupPage
{
    #region Properties

    private string _importString;
    private ExcelImporterService _excelImporterService;

    #endregion

    #region Ctor

    public ImportPopup(ExcelImporterService excelImporterService)
    {
        InitializeComponent();
        _excelImporterService = excelImporterService;
    }

    #endregion

    #region Page methods and events

    protected override void OnAppearing()
    {
        base.OnAppearing();

        SetTargetPath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
    }

    private async void ImportButton_Clicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_importString) && File.Exists(_importString))
        {
            await _excelImporterService.Import(_importString);
        }
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
        _importString = path;
        TargetDirectoryPathLabel.Text = _importString;
    }

    #endregion
}