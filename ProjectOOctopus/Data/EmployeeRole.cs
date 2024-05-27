
namespace ProjectOOctopus.Data
{
    public class EmployeeRole : IEquatable<EmployeeRole?>
    {
        public string Name { get; set; }
        public Color Color { get; set; }

        public static Color LastColor { get; set; } = Color.FromHsv(0f, 1f, 0.8f);

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
