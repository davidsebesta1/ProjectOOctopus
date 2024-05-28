using System.Collections.ObjectModel;

namespace ProjectOOctopus.Data
{
    public class AssignedRoleCollection : ObservableCollection<Employee>, IEquatable<AssignedRoleCollection?>
    {
        public EmployeeRole Role { get; set; }

        public AssignedRoleCollection(EmployeeRole role)
        {
            Role = role;
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
