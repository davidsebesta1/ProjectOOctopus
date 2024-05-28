using ProjectOOctopus.Data;
using ProjectOOctopus.Events;
using System.Collections.ObjectModel;

namespace ProjectOOctopus.Services
{
    public class RolesService
    {
        public static event EventHandler<RoleRemovedEventArgs>? RoleRemovedEvent;

        private string _currentSearch = string.Empty;

        private readonly ObservableCollection<EmployeeRole> _allRoles = new ObservableCollection<EmployeeRole>();
        public ObservableCollection<EmployeeRole> Roles { get; private set; } = new ObservableCollection<EmployeeRole>();

        private readonly CsvLoader _csvLoader;

        public RolesService(CsvLoader loader)
        {
            _csvLoader = loader;
        }

        public async Task LoadBaseRoles()
        {
            await foreach (EmployeeRole role in _csvLoader.LoadBaseRoles())
            {
                AddRole(role);
            }
        }

        public void AddRole(EmployeeRole role)
        {
            _allRoles.Add(role);

            TryAddEmployeeRoleNameCheck(role);
        }

        public void RemoveRole(EmployeeRole role)
        {
            _allRoles.Remove(role);
            Roles.Remove(role);

            RoleRemovedEvent?.Invoke(this, new RoleRemovedEventArgs(role));
        }

        public void ClearAllRoles()
        {
            _allRoles.Clear();
            Roles.Clear();
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
