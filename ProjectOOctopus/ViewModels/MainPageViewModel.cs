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

            Projects = _projectsService.Projects;
            Employees = _employeesService.Employees;
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
            await MopupService.Instance.PushAsync(new AddOrEditEmployeePopup(_employeesService));
        }

        [RelayCommand]
        private async Task EditEmployee(Employee employee)
        {
            await MopupService.Instance.PushAsync(new AddOrEditEmployeePopup(_employeesService, employee));
        }

        [RelayCommand]
        private async Task AddProject()
        {
            await MopupService.Instance.PushAsync(new AddOrEditProjectPopup(_projectsService));
        }

        [RelayCommand]
        private async Task EditProject(ProjectData project)
        {
            await MopupService.Instance.PushAsync(new AddOrEditProjectPopup(_projectsService, project));
        }

        [RelayCommand]
        private void RemoveEmployee(Employee employee)
        {
            _employeesService.RemoveEmployee(employee);
        }

        [RelayCommand]
        private void RemoveProject(ProjectData project)
        {
            _projectsService.RemoveProject(project);
        }

        [RelayCommand]
        private void SearchProjectsByName(string name)
        {
            _projectsService.SearchByName(name);
        }

        [RelayCommand]
        private void SearchEmployeesByName(string name)
        {
            _employeesService.SearchByName(name);
        }

        [RelayCommand]
        private async Task OpenRoleManager()
        {
            await MopupService.Instance.PushAsync(_roleManagerPopup);
        }

        #endregion
    }
}