using ProjectOOctopus.Data;
using ProjectOOctopus.Events;
using System.Collections.ObjectModel;

namespace ProjectOOctopus.Services
{
    /// <summary>
    /// Service for managing all available roles for employees
    /// All roles must be added into this service for them to be assigned and used without issues
    /// </summary>
    public class RolesService
    {
        public event EventHandler<RoleRemovedEventArgs>? RoleRemovedEvent;

        #region Properties

        public ObservableCollection<EmployeeRole> Roles { get; private set; } = new ObservableCollection<EmployeeRole>();

        private readonly CsvLoader _csvLoader;

        #endregion

        #region Ctor

        public RolesService(CsvLoader loader)
        {
            _csvLoader = loader;
        }

        #endregion

        #region Service Methods

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

        #endregion
    }
}
