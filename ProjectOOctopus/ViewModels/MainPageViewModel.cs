using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using ProjectOOctopus.Data;
using ProjectOOctopus.Pages;
using ProjectOOctopus.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOOctopus.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private EmployeesService _employeesService;
        private ProjectsService _projectsService;

        public MainPageViewModel(EmployeesService service, ProjectsService projectsService)
        {
            _employeesService = service;
            _projectsService = projectsService;

            RefreshEmployeesCommand.Execute(null);
            RefreshProjectsCommand.Execute(null);
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool _isBusy = false;

        public bool IsNotBusy => !IsBusy;

        [ObservableProperty]
        private ObservableCollection<Employee> _employees;

        [ObservableProperty]
        private ObservableCollection<ProjectData> _projects;

        [RelayCommand]
        private async Task AddEmployeeToProject(ProjectData project)
        {
            await MopupService.Instance.PushAsync(new AddEmployeePopup(_employeesService, project));
        }

        [RelayCommand]
        private async Task AddProject()
        {
            await MopupService.Instance.PushAsync(new AddProjectPopup(_projectsService));
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
    }
}
