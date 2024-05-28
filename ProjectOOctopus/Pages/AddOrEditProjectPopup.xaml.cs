using Mopups.Pages;
using Mopups.Services;
using ProjectOOctopus.Data;
using ProjectOOctopus.Services;
using System.Linq;

namespace ProjectOOctopus.Pages;

public partial class AddOrEditProjectPopup : PopupPage
{
    private readonly ProjectsService _projectsService;
    private readonly RolesService _rolesService;

    private ProjectData _project;

    public AddOrEditProjectPopup(ProjectsService projectsService, RolesService rolesService, ProjectData project = null)
    {
        InitializeComponent();
        _projectsService = projectsService;
        _rolesService = rolesService;
        _project = project;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        RolesCollectionView.ItemsSource = _rolesService.Roles;

        if (_project != null)
        {
            ProjectNameEntry.Text = _project.ProjectName;
            ProjectDescriptionEntry.Text = _project.ProjectDescription;

            AddOrEditButton.Text = "Edit";


            foreach (EmployeeRole role in _project.EmployeesByRoles.Select(n => n.Role))
            {
                RolesCollectionView.SelectedItems.Add(role);
                //Frame el = RolesCollectionView.GetVisualTreeDescendants().First(n => (n as Frame)?.BindingContext == role) as Frame;
                //VisualStateManager.GoToState(el, "Selected");
            }
        }
    }

    private async void AddOrEditButton_Clicked(object sender, EventArgs e)
    {
        if (_project == null)
        {
            _project = new ProjectData(ProjectNameEntry.Text, ProjectDescriptionEntry.Text);
            foreach (EmployeeRole role in RolesCollectionView.SelectedItems.Cast<EmployeeRole>())
            {
                _project.AddRoleGroup(role);
            }

            _projectsService.AddProject(_project);

        }
        else
        {
            _project.ProjectName = ProjectNameEntry.Text;
            _project.ProjectDescription = ProjectDescriptionEntry.Text;

            var addedGroups = RolesCollectionView.SelectedItems.Cast<EmployeeRole>().Where(role => !_project.EmployeesByRoles.Any(group => group.Role == role)).ToList();
            var removedGroups = _project.EmployeesByRoles.Where(group => !RolesCollectionView.SelectedItems.Cast<EmployeeRole>().Contains(group.Role)).ToList();

            if (addedGroups.Count != 0) _project.AddRoleGroups(addedGroups);

            if (removedGroups.Count != 0)
            {
                bool res = await Shell.Current.DisplayAlert("Confirmation", $"Are you sure you want to remove group{(removedGroups.Count > 1 ? "s" : string.Empty)} {string.Join(',', removedGroups.Select(n => n.Role.Name))}. This action cannot be undone", "Yes", "No");
                _project.RemoveRoleGroups(removedGroups.Select(n => n.Role));
            }
        }

        await MopupService.Instance.PopAsync();
    }
}