using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using ProjectOOctopus.Data;
using ProjectOOctopus.Pages;
using ProjectOOctopus.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ProjectOOctopus.ViewModels
{
    /// <summary>
    /// ViewModel for the main page. Essentially a "backend" for pages. Handles communication from specified commands
    /// </summary>
    public partial class MainPageViewModel : ObservableObject
    {
        #region Properties

        private readonly EmployeesService _employeesService;
        private readonly ProjectsService _projectsService;
        private readonly RolesService _rolesService;
        private readonly ExcelExporterService _excelExporterService;
        private readonly ExcelImporterService _excelImporterService;

        [ObservableProperty]
        private ObservableCollection<Employee> _employees;

        [ObservableProperty]
        private ObservableCollection<ProjectData> _projects;

        private readonly RoleManagerPopup _roleManagerPopup;

        #endregion

        #region Ctor

        public MainPageViewModel(EmployeesService empService, ProjectsService projectsService, RolesService rolesService, RoleManagerPopup roleManagerPopup, ExcelExporterService excelExporterService, ExcelImporterService excelImporterService)
        {
            _employeesService = empService;
            _projectsService = projectsService;
            _rolesService = rolesService;

            _roleManagerPopup = roleManagerPopup;

            Projects = _projectsService.Projects;
            Employees = _employeesService.Employees;
            _excelExporterService = excelExporterService;
            _excelImporterService = excelImporterService;
        }

        #endregion

        #region Commands

        #region Employee

        [RelayCommand]
        private async Task AddEmployee()
        {
            await MopupService.Instance.PushAsync(new AddOrEditEmployeePopup(_employeesService, _rolesService, ServicesHelper.GetService<EntryValidatorService>()));
        }

        [RelayCommand]
        private async Task EditEmployee(Employee employee)
        {
            await MopupService.Instance.PushAsync(new AddOrEditEmployeePopup(_employeesService, _rolesService, ServicesHelper.GetService<EntryValidatorService>(), employee));
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

        [RelayCommand]
        private void HideEmployeeByAssignement(bool value)
        {
            _employeesService.HideEmployeeIfFullyAssigned = value;
            _employeesService.Refresh();
        }

        [RelayCommand]
        private void RefreshEmployees()
        {
            _employeesService.Refresh();
        }

        #endregion

        #region Project
        [RelayCommand]
        private async Task AddProject()
        {
            await MopupService.Instance.PushAsync(new AddOrEditProjectPopup(_projectsService, _rolesService, ServicesHelper.GetService<EntryValidatorService>()));
        }

        [RelayCommand]
        private async Task EditProject(ProjectData project)
        {
            await MopupService.Instance.PushAsync(new AddOrEditProjectPopup(_projectsService, _rolesService, ServicesHelper.GetService<EntryValidatorService>(), project));
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

        #region Other

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

        #endregion

        #region Import/Export

        [RelayCommand]
        public async Task ExportToExcel()
        {
            await MopupService.Instance.PushAsync(new ExportPopup(_excelExporterService));
        }

        [RelayCommand]
        public async Task ImportFromExcel()
        {
            await MopupService.Instance.PushAsync(new ImportPopup(_excelImporterService));
        }

        #endregion

        #region Info & Help

        [RelayCommand]
        public async Task OpenInfoPopup()
        {
            await MopupService.Instance.PushAsync(new InfoPopup());
        }

        [RelayCommand]
        public async void SafeRestartApp()
        {
            string tmpPath = Path.Combine(Path.GetTempPath(), "projectooctopussaferestart.xlsx");
            _excelExporterService.Export(tmpPath, true);

            string exeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string dirPath = Path.GetDirectoryName(exeFilePath);

            try
            {
                using (Process myProcess = new Process())
                {
                    myProcess.StartInfo.UseShellExecute = false;
                    myProcess.StartInfo.FileName = Path.Combine(dirPath, "ProjectOOctopus.exe");
                    myProcess.StartInfo.CreateNoWindow = true;
                    myProcess.Start();
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }

            await Task.Delay(1000);
            Environment.Exit(0);
        }

        #endregion
    }
}