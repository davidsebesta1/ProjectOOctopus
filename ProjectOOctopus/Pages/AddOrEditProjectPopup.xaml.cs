using Mopups.Pages;
using Mopups.Services;
using ProjectOOctopus.Data;
using ProjectOOctopus.Services;
using System.Collections.ObjectModel;
using System.Data;

namespace ProjectOOctopus.Pages;

public partial class AddOrEditProjectPopup : PopupPage
{
    private readonly ProjectsService _projectsService;
    private readonly RolesService _rolesService;

    private ProjectData _project;

    private Dictionary<RoleGroupEntryData, Entry> _entryCheckBoxesCache;

    private EntryValidatorService _validatorService;

    public AddOrEditProjectPopup(ProjectsService projectsService, RolesService rolesService, EntryValidatorService entryValidatorService, ProjectData project = null)
    {
        InitializeComponent();
        _projectsService = projectsService;
        _rolesService = rolesService;
        _project = project;
        _validatorService = entryValidatorService;

        _entryCheckBoxesCache = new Dictionary<RoleGroupEntryData, Entry>(rolesService.Roles.Count);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _validatorService.TryRegisterValidation("ProjectName", ProjectNameEntry, (text) => !string.IsNullOrEmpty(text), PrNameErrText);
        _validatorService.TryRegisterValidation("ProjectDescription", ProjectDescriptionEntry, (text) => !string.IsNullOrEmpty(text), PrDescErrText);

        RolesCollectionView.ItemsSource = null;

        ObservableCollection<RoleGroupEntryData> entryDatas = new ObservableCollection<RoleGroupEntryData>();
        RolesCollectionView.ItemsSource = entryDatas;

        foreach (EmployeeRole role in _rolesService.Roles)
        {
            entryDatas.Add(new RoleGroupEntryData(role));
        }

        if (_project != null)
        {
            ProjectNameEntry.Text = _project.ProjectName;
            ProjectDescriptionEntry.Text = _project.ProjectDescription;

            AddOrEditButton.Text = "Edit";
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _entryCheckBoxesCache.Clear();
        _entryCheckBoxesCache = null;

        _validatorService.ClearAllValidations();
    }

    private async void AddOrEditButton_Clicked(object sender, EventArgs e)
    {
        _validatorService.RevalidateAll();
        if (!_validatorService.AllValid)
        {
            await Shell.Current.DisplayAlert("Validation", "Please fix any invalid input fields and try again", "Okay");
            return;
        }

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
                Entry entry = _entryCheckBoxesCache[new RoleGroupEntryData(assignedCollection.Role, 0)];
                if (entry != null) assignedCollection.TargetCount = int.Parse(entry.Text);
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

    private void RolesCollectionView_ChildAdded(object sender, ElementEventArgs e)
    {
        if (_project != null && e.Element is Frame)
        {
            Entry entry = (Entry)e.Element.FindByName("TargetAmountEntry");
            RoleGroupEntryData roleGroup = (RoleGroupEntryData)e.Element.BindingContext;
            _entryCheckBoxesCache.Add(roleGroup, entry);

            AssignedRoleCollection target = _project.EmployeesByRoles.FirstOrDefault(n => n.Role == roleGroup.Role);

            if (target != null && target.TargetCount > 0)
            {
                RolesCollectionView.SelectedItems.Add(roleGroup);
                entry.Text = target.TargetCount.ToString();
            }
        }
    }
}