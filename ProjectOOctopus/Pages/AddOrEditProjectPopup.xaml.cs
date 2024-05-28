using Mopups.Pages;
using Mopups.Services;
using ProjectOOctopus.Data;
using ProjectOOctopus.Services;
using System.Collections.ObjectModel;
using System.Data;
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

        ObservableCollection<RoleGroupEntryData> entryDatas = new ObservableCollection<RoleGroupEntryData>();
        foreach (EmployeeRole role in _rolesService.Roles)
        {
            entryDatas.Add(new RoleGroupEntryData(role));
        }
        RolesCollectionView.ItemsSource = entryDatas;

        if (_project != null)
        {
            ProjectNameEntry.Text = _project.ProjectName;
            ProjectDescriptionEntry.Text = _project.ProjectDescription;

            AddOrEditButton.Text = "Edit";


            foreach (RoleGroupEntryData role in _project.EmployeesByRoles.Select(n => new RoleGroupEntryData(n.Role, n.TargetCount)))
            {
                RolesCollectionView.SelectedItems.Add(role);

                var tree = RolesCollectionView.GetVisualTreeDescendants();
                Element el = (Element)tree.First(n => (n as Element)?.BindingContext as RoleGroupEntryData == role);
                Entry entry = el.FindByName("TargetAmountEntry") as Entry;
                entry.Text = role.TargetAmount.ToString();
            }
        }
    }

    private async void AddOrEditButton_Clicked(object sender, EventArgs e)
    {
        if (_project == null)
        {
            _project = new ProjectData(ProjectNameEntry.Text, ProjectDescriptionEntry.Text);
            foreach (RoleGroupEntryData role in RolesCollectionView.SelectedItems.Cast<RoleGroupEntryData>())
            {
                _project.AddRoleGroup(role.Role, role.TargetAmount);
            }

            _projectsService.AddProject(_project);

        }
        else
        {
            _project.ProjectName = ProjectNameEntry.Text;
            _project.ProjectDescription = ProjectDescriptionEntry.Text;

            var addedGroups = RolesCollectionView.SelectedItems.Cast<RoleGroupEntryData>().Where(role => !_project.EmployeesByRoles.Any(group => group.Role == role.Role)).ToList();
            var removedGroups = _project.EmployeesByRoles.Where(group => !RolesCollectionView.SelectedItems.Cast<RoleGroupEntryData>().Any(n => group.Role == n.Role)).ToList();
            var editedGroups = _project.EmployeesByRoles.Where(group => !removedGroups.Contains(group));

            //Handle edited
            foreach (AssignedRoleCollection assignedCollection in editedGroups)
            {
                assignedCollection.TargetCount = int.Parse(GetEntryElementByData(new RoleGroupEntryData(assignedCollection.Role, 0)).Text);
            }

            //Handle added
            if (addedGroups.Count != 0) _project.AddRoleGroups(addedGroups.Select(entryData => new AssignedRoleCollection(entryData.Role, _project, entryData.TargetAmount)));

            //Handle removed
            if (removedGroups.Count != 0)
            {
                bool res = await Shell.Current.DisplayAlert("Confirmation", $"Are you sure you want to remove group{(removedGroups.Count > 1 ? "s" : string.Empty)} {string.Join(',', removedGroups.Select(n => n.Role.Name))}. This action cannot be undone", "Yes", "No");
                _project.RemoveRoleGroups(removedGroups.Select(n => n.Role));
            }
        }

        await MopupService.Instance.PopAsync();
    }

    private Entry GetEntryElementByData(RoleGroupEntryData data)
    {
        var tree = RolesCollectionView.GetVisualTreeDescendants();
        Element el = (Element)tree.First(n => (n as Element)?.BindingContext as RoleGroupEntryData == data);
        Entry entry = el.FindByName("TargetAmountEntry") as Entry;
        return entry;
    }

    private void TargetAmountEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.NewTextValue)) return;

        Entry entry = sender as Entry;
        RoleGroupEntryData data = entry.BindingContext as RoleGroupEntryData;

        if (data != null && int.TryParse(e.NewTextValue, out int result) && result >= 0 && result <= 100)
        {
            data.TargetAmount = result;
        }
    }
}