
using CommunityToolkit.Mvvm.ComponentModel;
using ProjectOOctopus.Events;

namespace ProjectOOctopus.Data
{
    /// <summary>
    /// Employee role data class
    /// </summary>
    public partial class EmployeeRole : ObservableObject, IEquatable<EmployeeRole?>
    {
        public event EventHandler<ColorChangedEvent> OnColorChangedEvent;

        #region Properties

        [ObservableProperty]
        public string _name;

        [ObservableProperty]
        public Color _color;

        #endregion

        #region Ctor

        public EmployeeRole(string name, Color color)
        {
            Name = name;
            Color = color;
        }

        #endregion

        #region Property changed events

        partial void OnColorChanged(Color value)
        {
            OnColorChangedEvent?.Invoke(this, new ColorChangedEvent(value));
        }

        #endregion

        #region Object methods

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

        #endregion
    }
}
