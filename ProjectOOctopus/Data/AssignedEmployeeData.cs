using CommunityToolkit.Mvvm.ComponentModel;
using System.Data;

namespace ProjectOOctopus.Data
{
    public partial class AssignedEmployeeData : ObservableObject, IEquatable<AssignedEmployeeData?>
    {
        [ObservableProperty]
        private Employee _employee;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AssignmentUsageString))]
        private int _localAssignmentUsage;

        public string AssignmentUsageString => $"{LocalAssignmentUsage}%";

        public AssignedEmployeeData(Employee employee, int localAssignmentUsage)
        {
            Employee = employee;
            LocalAssignmentUsage = localAssignmentUsage;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as AssignedEmployeeData);
        }

        public bool Equals(AssignedEmployeeData? other)
        {
            return other is not null &&
                   EqualityComparer<Employee>.Default.Equals(Employee, other.Employee);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Employee);
        }

        public static bool operator ==(AssignedEmployeeData? left, AssignedEmployeeData? right)
        {
            return EqualityComparer<AssignedEmployeeData>.Default.Equals(left, right);
        }

        public static bool operator !=(AssignedEmployeeData? left, AssignedEmployeeData? right)
        {
            return !(left == right);
        }
    }
}
