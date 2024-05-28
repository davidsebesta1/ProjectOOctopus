using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ProjectOOctopus.Data
{
    public partial class AssignedRoleCollection : ObservableObject, IEquatable<AssignedRoleCollection?>, IEnumerable<Employee>, IDisposable
    {
        public ProjectData ProjectData { get; set; }
        public EmployeeRole Role { get; set; }

        [ObservableProperty]
        private ObservableCollection<Employee> _employees;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AssignedEmployeesString))]
        public int _targetCount;

        public int Count => Employees.Count;

        public string AssignedEmployeesString => $"{_employees.Count}/{TargetCount}";

        public Color BackgroundColor => TargetCount == Count ? Role.Color : Color.FromRgb((int)((Role.Color.Red - 0.2) * 255), (int)((Role.Color.Green - 0.2) * 255), (int)((Role.Color.Blue - 0.2) * 255));

        public AssignedRoleCollection(EmployeeRole role, ProjectData projectData, int targetCount)
        {
            _employees = new ObservableCollection<Employee>();
            Role = role;
            ProjectData = projectData;
            TargetCount = targetCount;

            Employees.CollectionChanged += Employees_CollectionChanged;
        }

        private void Employees_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(AssignedEmployeesString));
        }

        public void Add(Employee emp, int usage)
        {
            emp.SetAssigmentUsage(ProjectData, this, usage);
            Employees.Add(emp);
        }

        public bool Remove(Employee emp)
        {
            emp.SetAssigmentUsage(ProjectData, this, 0);
            return Employees.Remove(emp);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as AssignedRoleCollection);
        }

        public bool Equals(AssignedRoleCollection? other)
        {
            return other is not null &&
                   EqualityComparer<EmployeeRole>.Default.Equals(Role, other.Role);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Role);
        }

        public IEnumerator<Employee> GetEnumerator()
        {
            return Employees.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Employees.GetEnumerator();
        }

        public void Dispose()
        {
            Employees.CollectionChanged -= Employees_CollectionChanged;

            Employees?.Clear();
            Employees = null;

            ProjectData = null;
            Role = null;

            GC.SuppressFinalize(this);
        }

        public static bool operator ==(AssignedRoleCollection? left, AssignedRoleCollection? right)
        {
            return EqualityComparer<AssignedRoleCollection>.Default.Equals(left, right);
        }

        public static bool operator !=(AssignedRoleCollection? left, AssignedRoleCollection? right)
        {
            return !(left == right);
        }
    }
}
