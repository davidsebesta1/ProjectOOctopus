using Mopups.Pages;
using Mopups.Services;
using ProjectOOctopus.Data;

namespace ProjectOOctopus.Pages;

public partial class AssignEmployeePopup : PopupPage
{
    private readonly ProjectData _projectData;
    private readonly Employee _employee;
    private readonly AssignedRoleCollection _projectRolesCollection;

    public AssignEmployeePopup(Employee employee, ProjectData projectData, AssignedRoleCollection targetCollection)
    {
        InitializeComponent();
        _projectData = projectData;
        _employee = employee;
        _projectRolesCollection = targetCollection;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        int usage = _employee.GetAssignentUsage(_projectData);
        int remaining = _employee.GetRemainingUsageToFull();
        AssignPercEntry.Text = usage > 0 ? (remaining != 100 ? remaining.ToString() : "0") : "0";
        AddOrEditButton.Text = usage > 0 ? "Edit" : "Assign";

        CurrentAssignementUsageText.Text = $"{usage}% currently assigned";
    }

    private async void AddOrEditButton_Clicked(object sender, EventArgs e)
    {
        if (AssignPercErrText.IsVisible) return;

        _employee.SetAssigmentUsage(_projectData, byte.Parse(AssignPercEntry.Text));
        _projectRolesCollection.Add(_employee);

        await MopupService.Instance.PopAsync();
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

        if (_employee.GetAssignentUsage(_projectData) + val > 100)
        {
            AssignPercWarningText.IsVisible = true;
        }
    }
}