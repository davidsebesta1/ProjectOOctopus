using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Color = Microsoft.Maui.Graphics.Color;

namespace ProjectOOctopus.Data
{
    public partial class AssignedRoleCollection : ObservableObject, IEquatable<AssignedRoleCollection?>, IEnumerable<AssignedEmployeeData>, IDisposable
    {
        #region Properties
        public ProjectData ProjectData { get; set; }

        [ObservableProperty]
        private EmployeeRole _role;

        [ObservableProperty]
        private ObservableCollection<AssignedEmployeeData> _employees;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AssignedEmployeesString))]
        [NotifyPropertyChangedFor(nameof(BackgroundColor))]
        public int _targetCount;

        public int Count => Employees.Count;

        public string AssignedEmployeesString => $"{Employees.Count}/{TargetCount}";

        public Color BackgroundColor => TargetCount != Count ? Role.Color : Color.FromRgba((int)((Role.Color.Red - 0.4f) * 255), (int)((Role.Color.Green - 0.4f) * 255), (int)((Role.Color.Blue - 0.4f) * 255), 0.2f);
        public Color EmployeeCellBackgroundColor => Color.FromRgb((int)((Role.Color.Red * 0.7f) * 255), (int)((Role.Color.Green * 0.7f) * 255), (int)((Role.Color.Blue * 0.7f) * 255));

        #endregion

        #region Ctor

        public AssignedRoleCollection(EmployeeRole role, ProjectData projectData, int targetCount)
        {
            _employees = new ObservableCollection<AssignedEmployeeData>();
            Role = role;
            ProjectData = projectData;
            TargetCount = targetCount;

            Employees.CollectionChanged += Employees_CollectionChanged;
            Role.OnColorChangedEvent += Role_OnColorChangedEvent;
        }

        #endregion

        #region On Property Changed Events

        private void Role_OnColorChangedEvent(object? sender, Events.ColorChangedEvent e)
        {
            OnPropertyChanged(nameof(BackgroundColor));
            OnPropertyChanged(nameof(EmployeeCellBackgroundColor));
        }

        private void Employees_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(AssignedEmployeesString));
            OnPropertyChanged(nameof(BackgroundColor));
        }

        partial void OnTargetCountChanged(int oldValue, int newValue)
        {
            ProjectData.RefreshBackgroundColor();
        }

        #endregion

        #region Collection Methods

        public void Add(Employee emp, int usage)
        {
            emp.SetAssigmentUsage(ProjectData, this, usage);
            Employees.Add(new AssignedEmployeeData(emp, this, usage));
        }

        public bool Remove(Employee emp)
        {
            emp.SetAssigmentUsage(ProjectData, this, 0);
            return Employees.Remove(new AssignedEmployeeData(emp, null, 0));
        }

        public bool Remove(AssignedEmployeeData data)
        {
            data.Employee.SetAssigmentUsage(ProjectData, this, 0);
            return Employees.Remove(data);
        }

        #endregion

        #region Object related methods

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

        public IEnumerator<AssignedEmployeeData> GetEnumerator()
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

        #endregion
    }
}
