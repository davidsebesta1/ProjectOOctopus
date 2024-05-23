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

        private RoleManagerPopup _roleManagerPopup;

        #endregion

        #region Ctor

        public MainPageViewModel(EmployeesService empService, ProjectsService projectsService, RolesService rolesService, RoleManagerPopup roleManagerPopup)
        {
            _employeesService = empService;
            _projectsService = projectsService;
            _rolesService = rolesService;

            _roleManagerPopup = roleManagerPopup;

            RefreshEmployees();
            RefreshProjects();
        }

        #endregion

        #region Commands

        [RelayCommand]
        private async Task AddEmployeeToProject(ProjectData project)
        {
            await MopupService.Instance.PushAsync(new AssignEmployeePopup(project));
        }

        [RelayCommand]
        private async Task AddEmployee()
        {
            await MopupService.Instance.PushAsync(new AddEmployeePopup(_employeesService));
        }

        [RelayCommand]
        private async Task AddProject()
        {
            await MopupService.Instance.PushAsync(new AddProjectPopup(_projectsService));
        }

        [RelayCommand]
        private void SearchProjectsByName(string name)
        {
            _projectsService.SearchByName(name);
        }

        [RelayCommand]
        private void RefreshEmployees()
        {
            Employees = _employeesService.Employees;
        }

        [RelayCommand]
        private void RefreshProjects()
        {
            Projects = _projectsService.Projects;
        }

        [RelayCommand]
        private async Task OpenRoleManager()
        {
            await MopupService.Instance.PushAsync(_roleManagerPopup);
        }

        #endregion
    }
}