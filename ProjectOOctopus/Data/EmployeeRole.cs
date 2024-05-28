
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectOOctopus.Data
{
    public partial class EmployeeRole : ObservableObject, IEquatable<EmployeeRole?>
    {
        [ObservableProperty]
        public string _name;

        [ObservableProperty]
        public Color _color;

        public EmployeeRole(string name, Color color)
        {
            Name = name;
            Color = color;
        }

        public override string? ToString()
        {
            return Name;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as EmployeeRole);
        }

        public bool Equals(EmployeeRole? other)
        {
            return other is not null &&
                   Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        public static bool operator ==(EmployeeRole? left, EmployeeRole? right)
        {
            return EqualityComparer<EmployeeRole>.Default.Equals(left, right);
        }

        public static bool operator !=(EmployeeRole? left, EmployeeRole? right)
        {
            return !(left == right);
        }
    }
}
