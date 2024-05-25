using Mopups.Pages;
using Mopups.Services;
using ProjectOOctopus.Data;
using ProjectOOctopus.Services;

namespace ProjectOOctopus.Pages;

public partial class AddOrEditProjectPopup : PopupPage
{
    private ProjectsService _projectsService;
    private ProjectData _project;

    public AddOrEditProjectPopup(ProjectsService projectsService, ProjectData project = null)
    {
        InitializeComponent();
        _projectsService = projectsService;
        _project = project;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_project != null)
        {
            ProjectNameEntry.Text = _project.ProjectName;
            ProjectDescriptionEntry.Text = _project.ProjectDescription;

            AddOrEditButton.Text = "Edit";
        }
    }

    private async void AddOrEditButton_Clicked(object sender, EventArgs e)
    {
        if (_project == null)
        {
            _project = new ProjectData(ProjectNameEntry.Text, ProjectDescriptionEntry.Text);
            _projectsService.AddProject(_project);
        }
        else
        {
            _project.ProjectName = ProjectNameEntry.Text;
            _project.ProjectDescription = ProjectDescriptionEntry.Text;
        }

        await MopupService.Instance.PopAsync();
    }
}