using ProjectOOctopus.Data;

namespace ProjectOOctopus.Events
{
    public class RoleRemovedEventArgs : EventArgs
    {
        public readonly EmployeeRole Role;

        public RoleRemovedEventArgs(EmployeeRole role)
        {
            Role = role;
        }
    }
}
