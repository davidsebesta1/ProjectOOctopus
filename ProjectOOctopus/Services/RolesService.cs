using ProjectOOctopus.Data;
using ProjectOOctopus.Events;
using System.Collections.ObjectModel;

namespace ProjectOOctopus.Services
{
    public class RolesService
    {
        public static event EventHandler<RoleAddedEventArgs>? RoleAddedEvent;

        private string _currentSearch = string.Empty;

        private readonly ObservableCollection<EmployeeRole> _allRoles = new ObservableCollection<EmployeeRole>();
        public ObservableCollection<EmployeeRole> Roles { get; private set; } = new ObservableCollection<EmployeeRole>();


        public void AddRole(EmployeeRole role)
        {
            _allRoles.Add(role);

            TryAddEmployeeRoleNameCheck(role);
            RoleAddedEvent?.Invoke(this, new RoleAddedEventArgs(role));
            //add remove role event, make it so the dictionary in projectdata updates with this
        }

        public void SearchByName(string name)
        {
            _currentSearch = name;
            if (string.IsNullOrEmpty(_currentSearch))
            {
                Roles.Clear();
                foreach (EmployeeRole role in _allRoles)
                {
                    Roles.Add(role);
                }

                return;
            }

            Roles.Clear();
            foreach (EmployeeRole role in _allRoles)
            {
                TryAddEmployeeRoleNameCheck(role);
            }
        }

        private void TryAddEmployeeRoleNameCheck(EmployeeRole role)
        {
            if (role.Name.Contains(_currentSearch, StringComparison.InvariantCultureIgnoreCase))
            {
                Roles.Add(role);
            }
        }
    }
}
