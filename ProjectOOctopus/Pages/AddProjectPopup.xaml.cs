namespace ProjectOOctopus.Pages;
using Mopups.Pages;
using Mopups.Services;
using ProjectOOctopus.Data;
using ProjectOOctopus.Services;

public partial class AddProjectPopup : PopupPage
{
    private ProjectsService _projectsService;

    public AddProjectPopup(ProjectsService projectsService)
    {
        InitializeComponent();
        _projectsService = projectsService;
    }

    private async void AddOrEditButton_Clicked(object sender, EventArgs e)
    {
        _projectsService.Projects.Add(new ProjectData(ProjectNameEntry.Text, ProjectDescriptionEntry.Text));
        await MopupService.Instance.PopAsync();
    }
}