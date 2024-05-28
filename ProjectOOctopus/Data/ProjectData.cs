using CommunityToolkit.Mvvm.ComponentModel;
using ProjectOOctopus.Events;
using ProjectOOctopus.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;

namespace ProjectOOctopus.Data
{
    public partial class ProjectData : ObservableObject, IEquatable<ProjectData?>, IDisposable
    {
        [ObservableProperty]
        private string _projectName;

        [ObservableProperty]
        private string _projectDescription;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BackgroundColor))]
        private ObservableCollection<AssignedRoleCollection> _employeesByRoles = new ObservableCollection<AssignedRoleCollection>();

        public Color BackgroundColor => EmployeesByRoles.All(n => n.TargetCount == n.Count) ? Color.FromHex("#99222222") : Color.FromHex("#88AFAFAF");

        public ProjectData(string projectName, string projectDescription)
        {
            ProjectName = projectName;
            ProjectDescription = projectDescription;
        }

        public void OnNewEmployeeRoleRemoved(object? sender, RoleRemovedEventArgs e)
        {
            RemoveRoleGroup(e.Role);
        }

        public void RemoveEmployeeFromAllRoles(Employee employee)
        {
            foreach (AssignedRoleCollection role in EmployeesByRoles)
            {
                role.Remove(employee);
            }
        }

        public void AddRoleGroup(EmployeeRole role, int targetCount, bool reorder = false)
        {
            if (EmployeesByRoles.Any(n => n.Role == role))
            {
                return;
            }

            AssignedRoleCollection collection = new AssignedRoleCollection(role, this, targetCount);
            EmployeesByRoles.Add(collection);

            collection.Employees.CollectionChanged += Employees_CollectionChanged;

            if (reorder)
            {
                RolesService employeeRoleService = ServicesHelper.GetService<RolesService>();
                EmployeesByRoles = new ObservableCollection<AssignedRoleCollection>(EmployeesByRoles.OrderBy(n => employeeRoleService.Roles.IndexOf(n.Role)));
            }
        }

        private void Employees_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(BackgroundColor));
        }

        private void AddRoleGroup(AssignedRoleCollection role, bool reorder = false)
        {
            EmployeesByRoles.Add(role);

            if (reorder)
            {
                RolesService employeeRoleService = ServicesHelper.GetService<RolesService>();
                EmployeesByRoles = new ObservableCollection<AssignedRoleCollection>(EmployeesByRoles.OrderBy(n => employeeRoleService.Roles.IndexOf(n.Role)));
            }
        }

        public void AddRoleGroups(IEnumerable<AssignedRoleCollection> roles)
        {
            if (roles == null) return;

            var last = roles.Last();
            foreach (AssignedRoleCollection role in roles)
            {
                AddRoleGroup(role, role == last);
            }
        }

        public void RemoveRoleGroup(EmployeeRole role, bool reorder = false)
        {
            AssignedRoleCollection collection = EmployeesByRoles.FirstOrDefault(n => n.Role == role);
            if (collection == null)
            {
                return;
            }

            foreach (Employee emp in collection.Select(n => n.Employee))
            {
                emp.SetAssigmentUsage(collection.ProjectData, collection, 0);
            }

            EmployeesByRoles.Remove(collection);
            collection.Employees.CollectionChanged -= Employees_CollectionChanged;

            if (reorder)
            {
                RolesService employeeRoleService = ServicesHelper.GetService<RolesService>();
                EmployeesByRoles = new ObservableCollection<AssignedRoleCollection>(EmployeesByRoles.OrderBy(n => employeeRoleService.Roles.IndexOf(n.Role)));
            }

            collection.Dispose();
        }

        public void RemoveRoleGroups(IEnumerable<EmployeeRole> roles)
        {
            if (roles == null) return;

            var last = roles.Last();
            foreach (EmployeeRole role in roles)
            {
                RemoveRoleGroup(role, role == last);
            }
        }

        public void Dispose()
        {
            foreach (AssignedRoleCollection col in EmployeesByRoles)
            {
                col.Dispose();
            }

            EmployeesByRoles?.Clear();
            EmployeesByRoles = null;

            GC.SuppressFinalize(this);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ProjectData);
        }

        public bool Equals(ProjectData? other)
        {
            return other is not null && ProjectName == other.ProjectName && ProjectDescription == other.ProjectDescription;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ProjectName, ProjectDescription);
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
