using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using ProjectOOctopus.Data;
using ProjectOOctopus.Pages;
using ProjectOOctopus.Services;
using System.Collections.ObjectModel;

namespace ProjectOOctopus.ViewModels
{
    public partial class RoleManagerViewModel : ObservableObject
    {
        public readonly RolesService _rolesService;

        [ObservableProperty]
        private ObservableCollection<EmployeeRole> _roles;

        public RoleManagerViewModel(RolesService rolesService)
        {
            _rolesService = rolesService;

            _roles = _rolesService.Roles;
        }

        [RelayCommand]
        public async Task AddNewRole()
        {
            await MopupService.Instance.PushAsync(new AddOrEditRolePopup(_rolesService));
        }
    }
}
