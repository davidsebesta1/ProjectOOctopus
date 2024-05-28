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
            AssignPercEntry.Text = usage > 0 ? (remaining != 100 ? remaining.ToString() : "0") : "0";
        }

        CurrentAssignementUsageText.Text = $"{usage}% currently assigned";

        AssignPercEntry.Focus();
    }

    private void AssignPercEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        AssignPercErrText.IsVisible = false;
        AssignPercWarningText.IsVisible = false;
        if (!int.TryParse(e.NewTextValue, out int val) || val < 0 || val > 100)
        {
            AssignPercErrText.IsVisible = true;
            return;
        }

        if (_employee.TotalAssignmentUsage + val > 100)
        {
            AssignPercWarningText.IsVisible = true;
        }
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
        if (AssignPercErrText.IsVisible) return;

        _employee.SetAssigmentUsage(_projectData, _projectGroup, int.Parse(AssignPercEntry.Text));
        _projectGroup.Add(_employee);

        await MopupService.Instance.PopAsync();
    }
}