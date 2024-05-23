using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace ProjectOOctopus.Data
{
    public partial class ProjectData : ObservableObject, IEquatable<ProjectData?>
    {
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }

        [ObservableProperty]
        private Dictionary<EmployeeRole, ObservableCollection<Employee>> _employeesByRoles;

        [ObservableProperty]
        private ObservableCollection<Employee> _assignedEmployees = new ObservableCollection<Employee>();

        public ProjectData(string projectName, string projectDescription)
        {
            ProjectName = projectName;
            ProjectDescription = projectDescription;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ProjectData);
        }

        public bool Equals(ProjectData? other)
        {
            return other is not null &&
                   ProjectName == other.ProjectName;
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
