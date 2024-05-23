namespace ProjectOOctopus.Data
{
    public class Employee : IEquatable<Employee?>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName => FirstName + " " + LastName;
        public string Initials => FirstName[0] + LastName[0].ToString();

        public Employee(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
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
