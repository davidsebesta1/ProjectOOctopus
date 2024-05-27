using CommunityToolkit.Mvvm.ComponentModel;
using ProjectOOctopus.Events;
using System.Collections.ObjectModel;

namespace ProjectOOctopus.Data
{
    public partial class ProjectData : ObservableObject, IEquatable<ProjectData?>, IDisposable
    {
        [ObservableProperty]
        private string _projectName;

        [ObservableProperty]
        private string _projectDescription;

        [ObservableProperty]
        private ObservableCollection<AssignedRoleCollection> _employeesByRoles = new ObservableCollection<AssignedRoleCollection>();

        public ProjectData(string projectName, string projectDescription)
        {
            ProjectName = projectName;
            ProjectDescription = projectDescription;
        }

        public void OnNewEmployeeRoleAdded(object? sender, RoleAddedEventArgs e)
        {
            EmployeesByRoles.Add(new AssignedRoleCollection(e.Role));
        }

        public void OnNewEmployeeRoleRemoved(object? sender, RoleRemovedEventArgs e)
        {
            EmployeesByRoles.Remove(EmployeesByRoles.FirstOrDefault(n => n.Role == e.Role));
        }

        public void RemoveEmployeeFromAllRoles(Employee employee)
        {
            foreach (AssignedRoleCollection role in EmployeesByRoles)
            {
                role.Remove(employee);
            }
        }

        public void Dispose()
        {
            _employeesByRoles.Clear();
            _employeesByRoles = null;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ProjectData);
        }

        public bool Equals(ProjectData? other)
        {
            return other is not null && ProjectName == other.ProjectName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ProjectName);
        }


        public static bool operator ==(ProjectData? left, ProjectData? right)
        {
            return EqualityComparer<ProjectData>.Default.Equals(left, right);
        }

        public static bool operator !=(ProjectData? left, ProjectData? right)
        {
            return !(left == right);
        }
    }
}
