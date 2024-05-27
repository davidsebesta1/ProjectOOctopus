using ProjectOOctopus.Data;
using System.Collections.ObjectModel;

namespace ProjectOOctopus.Services
{
    public class ProjectsService
    {
        public ProjectsService() { }

        private string _currentSearch = string.Empty;

        private readonly ObservableCollection<ProjectData> _allProjects = new ObservableCollection<ProjectData>();
        public ObservableCollection<ProjectData> Projects { get; private set; } = new ObservableCollection<ProjectData>();

        public void AddProject(ProjectData project)
        {
            _allProjects.Add(project);

            RolesService.RoleAddedEvent += project.OnNewEmployeeRoleAdded;
            TryAddEmployeeByNameFilter(project);
        }

        public void RemoveProject(ProjectData project)
        {
            _allProjects.Remove(project);
            Projects.Remove(project);

            RolesService.RoleAddedEvent -= project.OnNewEmployeeRoleAdded;
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

        private void TryAddEmployeeByNameFilter(ProjectData project)
        {
            if (project.ProjectName.Contains(_currentSearch, StringComparison.InvariantCultureIgnoreCase))
            {
                Projects.Add(project);
            }
        }
    }
}
