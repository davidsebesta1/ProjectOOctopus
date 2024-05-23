using Mopups.Pages;
using ProjectOOctopus.Data;

namespace ProjectOOctopus.Pages;

public partial class AssignEmployeePopup : PopupPage
{
    private readonly ProjectData _projectData;
    private readonly EmployeeRole _selectedSpecialization;

    public AssignEmployeePopup(ProjectData projectData)
    {
        InitializeComponent();
        _projectData = projectData;
    }

    private void AddOrEditButton_Clicked(object sender, EventArgs e)
    {

    }
}