using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectOOctopus.Data
{
    /// <summary>
    /// Entry data for a role group and its target count. Roles are invidial objects in a project so it is not possible to store target amount inside of them.
    /// </summary>
    public partial class RoleGroupEntryData : ObservableObject, IEquatable<RoleGroupEntryData?>
    {
        #region Properties

        [ObservableProperty]
        private EmployeeRole _role;

        [ObservableProperty]
        private int _targetAmount;

        #endregion

        #region Ctor

        public RoleGroupEntryData(EmployeeRole role, int targetAmount = 0)
        {
            Role = role;
            TargetAmount = targetAmount;
        }

        #endregion

        #region Object methods

        public override bool Equals(object? obj)
        {
            return Equals(obj as RoleGroupEntryData);
        }

        public bool Equals(RoleGroupEntryData? other)
        {
            return other is not null &&
                   EqualityComparer<EmployeeRole>.Default.Equals(Role, other.Role);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Role);
        }

        public static bool operator ==(RoleGroupEntryData? left, RoleGroupEntryData? right)
        {
            return EqualityComparer<RoleGroupEntryData>.Default.Equals(left, right);
        }

        public static bool operator !=(RoleGroupEntryData? left, RoleGroupEntryData? right)
        {
            return !(left == right);
        }

        #endregion
    }
}