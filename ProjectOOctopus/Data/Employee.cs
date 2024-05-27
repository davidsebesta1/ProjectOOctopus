using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace ProjectOOctopus.Data
{
    public partial class Employee : ObservableObject, IEquatable<Employee?>, IDisposable
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullName))]
        [NotifyPropertyChangedFor(nameof(Initials))]
        private string _firstName;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullName))]
        [NotifyPropertyChangedFor(nameof(Initials))]
        private string _lastName;


        [ObservableProperty]
        private int _totalAssignmentUsage;
        private Dictionary<ProjectData, int> _assignmentsPerctangeByProject = new Dictionary<ProjectData, int>();

        [ObservableProperty]
        private ObservableCollection<EmployeeRole> _roles;

        [ObservableProperty]
        private string _rolesToolTip = "No known roles";

        public string FullName => FirstName + " " + LastName;
        public string Initials => FirstName[0] + LastName[0].ToString();

        public Employee(string firstName, string lastName, ObservableCollection<EmployeeRole> roles = null)
        {
            FirstName = firstName;
            LastName = lastName;
            Roles = roles;

            if (Roles != null)
            {
                Roles.CollectionChanged += Roles_CollectionChanged;
                Roles_CollectionChanged(null, null);
            }
        }

        public void SetAssigmentUsage(ProjectData project, byte usage)
        {
            if (_assignmentsPerctangeByProject.ContainsKey(project))
            {
                _assignmentsPerctangeByProject[project] = usage;
            }
            else
            {
                _assignmentsPerctangeByProject.Add(project, usage);
            }

            TotalAssignmentUsage = (byte)_assignmentsPerctangeByProject.Sum(n => n.Value);
        }

        public int GetAssignentUsage(ProjectData data)
        {
            if (_assignmentsPerctangeByProject.TryGetValue(data, out int usage))
            {
                return usage;
            }

            return 0;
        }

        public int GetRemainingUsageToFull()
        {
            return 100 - TotalAssignmentUsage;
        }

        partial void OnRolesChanged(ObservableCollection<EmployeeRole>? oldValue, ObservableCollection<EmployeeRole> newValue)
        {
            if (oldValue != null) Roles.CollectionChanged -= Roles_CollectionChanged;
            if (newValue != null) Roles.CollectionChanged += Roles_CollectionChanged;
        }

        private void Roles_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RolesToolTip = Roles.Any() ? string.Join(',', Roles.Select(n => n.Name)) : "No known roles";
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Employee);
        }

        public bool Equals(Employee? other)
        {
            return other is not null &&
                   FirstName == other.FirstName &&
                   LastName == other.LastName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FirstName, LastName);
        }

        public void Dispose()
        {
            if (Roles == null) return;

            Roles.Clear();
            Roles = null;
        }

        public static bool operator ==(Employee? left, Employee? right)
        {
            return EqualityComparer<Employee>.Default.Equals(left, right);
        }

        public static bool operator !=(Employee? left, Employee? right)
        {
            return !(left == right);
        }
    }
}
