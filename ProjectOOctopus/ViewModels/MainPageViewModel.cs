using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using ProjectOOctopus.Data;
using ProjectOOctopus.Pages;
using ProjectOOctopus.Services;
using System.Collections.ObjectModel;

namespace ProjectOOctopus.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        #region Properties

        private readonly EmployeesService _employeesService;
        private readonly ProjectsService _projectsService;
        private readonly RolesService _rolesService;

        [ObservableProperty]
        private ObservableCollection<Employee> _employees;

        [ObservableProperty]
        private ObservableCollection<ProjectData> _projects;

        private readonly RoleManagerPopup _roleManagerPopup;

        #endregion

        #region Ctor

        public MainPageViewModel(EmployeesService empService, ProjectsService projectsService, RolesService rolesService, RoleManagerPopup roleManagerPopup)
        {
            _employeesService = empService;
            _projectsService = projectsService;
            _rolesService = rolesService;

            _roleManagerPopup = roleManagerPopup;

            Projects = _projectsService.Projects;
            Employees = _employeesService.Employees;
        }

        #endregion

        #region Commands

        #region Employee

        [RelayCommand]
        private async Task AddEmployee()
        {
            await MopupService.Instance.PushAsync(new AddOrEditEmployeePopup(_employeesService, _rolesService));
        }

        [RelayCommand]
        private async Task EditEmployee(Employee employee)
        {
            await MopupService.Instance.PushAsync(new AddOrEditEmployeePopup(_employeesService, _rolesService, employee));
        }

        [RelayCommand]
        private async Task RemoveEmployee(Employee employee)
        {
            bool confirm = await Shell.Current.DisplayAlert("Confirmation", $"Are you sure you want to delete {employee.FullName}? This action cannot be undone", "Yes", "No");
            if (confirm) _employeesService.RemoveEmployee(employee);
        }

        [RelayCommand]
        private void SearchEmployeesByName(string name)
        {
            _employeesService.SearchByName(name);
        }

        #endregion

        #region Project
        [RelayCommand]
        private async Task AddProject()
        {
            await MopupService.Instance.PushAsync(new AddOrEditProjectPopup(_projectsService, _rolesService));
        }

        [RelayCommand]
        private async Task EditProject(ProjectData project)
        {
            await MopupService.Instance.PushAsync(new AddOrEditProjectPopup(_projectsService, _rolesService, project));
        }

        [RelayCommand]
        private async Task RemoveProject(ProjectData project)
        {
            bool confirm = await Shell.Current.DisplayAlert("Confirmation", $"Are you sure you want to delete {project.ProjectName}? This action cannot be undone", "Yes", "No");
            if (confirm) _projectsService.RemoveProject(project);
        }

        [RelayCommand]
        private void SearchProjectsByName(string name)
        {
            _projectsService.SearchByName(name);
        }

        #endregion

        [RelayCommand]
        private async Task OpenRoleManager()
        {
            await MopupService.Instance.PushAsync(_roleManagerPopup);
        }

        [RelayCommand]
        private async Task LoadBaseRoles()
        {
            await _rolesService.LoadBaseRoles();
        }

        #endregion
    }
}