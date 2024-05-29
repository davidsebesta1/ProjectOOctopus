using ProjectOOctopus.Data;
using ProjectOOctopus.Events;
using System.Collections.ObjectModel;

namespace ProjectOOctopus.Services
{
    public class RolesService
    {
        public event EventHandler<RoleRemovedEventArgs>? RoleRemovedEvent;

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
            Roles.Add(role);
        }

        public void RemoveRole(EmployeeRole role)
        {
            Roles.Remove(role);

            RoleRemovedEvent?.Invoke(this, new RoleRemovedEventArgs(role));
        }

        public void ClearAllRoles()
        {
            Roles.Clear();
        }
    }
}
