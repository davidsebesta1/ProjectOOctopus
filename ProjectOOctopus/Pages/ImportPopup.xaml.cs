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
    }

    private async void ImportButton_Clicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_importString) && File.Exists(_importString))
        {
            await _excelImporterService.Import(_importString);
            await Shell.Current.DisplayAlert("Import", "Import successful", "Okay");
        }
    }

    private async void SelectPathButton_Clicked(object sender, EventArgs e)
    {
        FileResult result = await FilePicker.Default.PickAsync();
        if (result != null && result.FullPath.EndsWith(".xlsx"))
        {
            SetTargetPath(result.FullPath);
        }
        else
        {
            await Shell.Current.DisplayAlert("Import", "Importing file must be of type 'xlsx'", "Okay");
        }
    }

    private void SetTargetPath(string path)
    {
        _importString = path;
        TargetDirectoryPathLabel.Text = _importString;
    }

    #endregion
}