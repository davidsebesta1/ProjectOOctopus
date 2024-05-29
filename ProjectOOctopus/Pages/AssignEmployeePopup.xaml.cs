using Mopups.Pages;
using Mopups.Services;
using ProjectOOctopus.Data;

namespace ProjectOOctopus.Pages;

public partial class AssignEmployeePopup : PopupPage
{
    private readonly ProjectData _projectData;
    private readonly Employee _employee;
    private readonly AssignedRoleCollection _projectGroup;

    private readonly bool _edit;

    private readonly Dictionary<string, bool> _validationValues = new Dictionary<string, bool>()
    {
        {"AssignmentPercentage", false },
    };

    public AssignEmployeePopup(Employee employee, ProjectData projectData, AssignedRoleCollection group, bool edit = false)
    {
        InitializeComponent();
        _projectData = projectData;
        _employee = employee;
        _projectGroup = group;

        _edit = edit;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

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

    private void AssignPercEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        bool errResult = !int.TryParse(e.NewTextValue, out int val) || val < 0 || val > 100;

        AssignPercErrText.IsVisible = !errResult;
        _validationValues["RoleName"] = errResult;

        AssignPercWarningText.IsVisible = _employee.TotalAssignmentUsage + val > 100;
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
        if (_validationValues.Any(n => !n.Value))
        {
            await Shell.Current.DisplayAlert("Validation", "Please fix any invalid input fields and try again", "Okay");
            return;
        }

        _projectGroup.Add(_employee, int.Parse(AssignPercEntry.Text));

        await MopupService.Instance.PopAsync();
    }
}