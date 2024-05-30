using ProjectOOctopus.Data;
using System.Collections.ObjectModel;

namespace ProjectOOctopus.Services
{
    /// <summary>
    /// Service for managing all projects within application.
    /// It is required to add new projects into this service for them to be shown and manipulated.
    /// </summary>
    public class ProjectsService
    {
        #region Prroperties
        private string _currentSearch = string.Empty;

        public readonly ObservableCollection<ProjectData> _allProjects = new ObservableCollection<ProjectData>();
        public ObservableCollection<ProjectData> Projects { get; private set; } = new ObservableCollection<ProjectData>();

        #endregion

        #region Ctor

        public ProjectsService()
        {

        }

        #endregion

        #region Service Methods

        public void AddProject(ProjectData project)
        {
            _allProjects.Add(project);

            TryAddEmployeeByNameFilter(project);
        }

        public void RemoveProject(ProjectData project)
        {
            project.RemoveAllEmployees();

            _allProjects.Remove(project);
            Projects.Remove(project);

            project.Dispose();
        }

        public void RemoveEmployeeFromAllProjects(Employee employee)
        {
            foreach (ProjectData project in _allProjects)
            {
                project.RemoveEmployeeFromAllRoles(employee);
            }
        }

        public void SearchByName(string name)
        {
            _currentSearch = name;
            if (string.IsNullOrEmpty(_currentSearch))
            {
                Projects.Clear();
                foreach (ProjectData emp in _allProjects)
                {
                    Projects.Add(emp);
                }

                return;
            }

            Projects.Clear();
            foreach (ProjectData emp in _allProjects)
            {
                TryAddEmployeeByNameFilter(emp);
            }
        }

        private async void TryAddEmployeeByNameFilter(ProjectData project)
        {
            if (project.ProjectName.Contains(_currentSearch, StringComparison.InvariantCultureIgnoreCase))
            {
                Projects.Add(project);
            }
        }

        #endregion
    }
}
