using Mopups.Pages;
using Mopups.Services;
using ProjectOOctopus.Data;
using ProjectOOctopus.Services;

namespace ProjectOOctopus.Pages;

public partial class AssignEmployeePopup : PopupPage
{
    #region Properties

    private readonly ProjectData _projectData;
    private readonly Employee _employee;
    private readonly AssignedRoleCollection _projectGroup;

    private readonly bool _edit;

    private EntryValidatorService _validatorService;

    #endregion

    #region Ctor

    public AssignEmployeePopup(Employee employee, ProjectData projectData, AssignedRoleCollection group, EntryValidatorService entryValidatorService, bool edit = false)
    {
        InitializeComponent();
        _validatorService = entryValidatorService;
        _projectData = projectData;
        _employee = employee;
        _projectGroup = group;

        _edit = edit;
    }

    #endregion

    #region Page methods and events

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _validatorService.TryRegisterValidation("AssignmentPercentage", AssignPercEntry, (text) =>
        {
            bool success = int.TryParse(text, out int val) && val >= 1 && val <= 100;
            AssignPercWarningText.IsVisible = _employee.TotalAssignmentUsage + val > 100;
            return success;
        }, AssignPercErrText);

        int usage = _employee.TotalAssignmentUsage;
        int remaining = _employee.GetRemainingUsageToFull();
        if (_edit)
        {
            AssignPercEntry.Text = _employee.GetAssignentUsage(_projectData, _projectGroup).ToString();
            AddOrEditButton.Text = "Edit";
        }
        else
        {
            AssignPercEntry.Text = remaining.ToString();
        }

        CurrentAssignementUsageText.Text = $"{usage}% currently assigned";

        AssignPercEntry.Focus();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _validatorService.ClearAllValidations();
    }

    private async void AddOrEditButton_Clicked(object sender, EventArgs e)
    {
        await TrySaveAndExit();
    }

    private async void AssignPercEntry_Completed(object sender, EventArgs e)
    {
        await TrySaveAndExit();
    }

    private async Task TrySaveAndExit()
    {
        _validatorService.RevalidateAll();
        if (!_validatorService.AllValid)
        {
            await Shell.Current.DisplayAlert("Validation", "Please fix any invalid input fields and try again", "Okay");
            return;
        }

        _projectGroup.Add(_employee, int.Parse(AssignPercEntry.Text));

        await MopupService.Instance.PopAsync();
    }

    #endregion
}