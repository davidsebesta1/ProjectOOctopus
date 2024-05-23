namespace ProjectOOctopus.Data
{
    public class EmployeeRole
    {
        public string Name { get; set; }
        public Color Color { get; set; }

        public static Color LastColor { get; set; } = Color.FromHsv(0f, 1f, 0.8f);

        public EmployeeRole(string name, Color color)
        {
            Name = name;
            Color = color;
        }
    }
}
