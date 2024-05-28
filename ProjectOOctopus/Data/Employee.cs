using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

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
        [NotifyPropertyChangedFor(nameof(BackgroundFrameColor))]
        [NotifyPropertyChangedFor(nameof(AssignmentUsageString))]
        [NotifyPropertyChangedFor(nameof(AssignmentUsageLightBackgroundColor))]
        [NotifyPropertyChangedFor(nameof(AssignmentUsageDarkColor))]
        private int _totalAssignmentUsage;
        private readonly Dictionary<ProjectData, Dictionary<AssignedRoleCollection, int>> _assignmentsPerctangeByProject = new Dictionary<ProjectData, Dictionary<AssignedRoleCollection, int>>();

        [ObservableProperty]
        private ObservableCollection<EmployeeRole> _roles;

        [ObservableProperty]
        private string _rolesToolTip = "No known roles";

        public string FullName => FirstName + " " + LastName;
        public string Initials => FirstName[0] + LastName[0].ToString();
        public string AssignmentUsageString => $"{TotalAssignmentUsage}%";

        public Color BackgroundFrameColor => TotalAssignmentUsage != 100 ? Color.FromHex("#88AFAFAF") : Color.FromHex("#33AFAFAF");

        public Color AssignmentUsageLightBackgroundColor => TotalAssignmentUsage <= 100 ? Color.FromHex("#000000") : Color.FromHex("#E1561D");
        public Color AssignmentUsageDarkColor => TotalAssignmentUsage <= 100 ? Color.FromHex("#FFFFFF") : Color.FromHex("#FFE000");

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

        public void SetAssigmentUsage(ProjectData project, AssignedRoleCollection group, int usage)
        {
            if (_assignmentsPerctangeByProject.TryGetValue(project, out var innerDir))
            {
                if (usage == 0)
                {
                    innerDir.Remove(group);
                }
                else if (!innerDir.TryAdd(group, usage))
                {
                    innerDir[group] = usage;
                }
            }
            else
            {
                _assignmentsPerctangeByProject.Add(project, new Dictionary<AssignedRoleCollection, int>() { { group, usage } });
            }

            TotalAssignmentUsage = (byte)_assignmentsPerctangeByProject.SelectMany(n => n.Value).Select(n => n.Value).Sum();
        }

        public int GetAssignentUsage(ProjectData data, AssignedRoleCollection group)
        {
            if (_assignmentsPerctangeByProject.TryGetValue(data, out var groups) && groups.TryGetValue(group, out int usage))
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

            GC.SuppressFinalize(this);
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
