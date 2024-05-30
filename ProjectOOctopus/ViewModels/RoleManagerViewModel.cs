using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using ProjectOOctopus.Data;
using ProjectOOctopus.Pages;
using ProjectOOctopus.Services;
using System.Collections.ObjectModel;

namespace ProjectOOctopus.ViewModels
{
    /// <summary>
    /// ViewModel for Role Manager page. Mainly used for background communication and correct showcase of roles
    /// </summary>
    public partial class RoleManagerViewModel : ObservableObject
    {
        #region Properties

        public readonly RolesService _rolesService;

        [ObservableProperty]
        private ObservableCollection<EmployeeRole> _roles;

        #endregion

        #region Ctor

        public RoleManagerViewModel(RolesService rolesService)
        {
            _rolesService = rolesService;

            _roles = _rolesService.Roles;
        }

        #endregion

        #region Commands

        [RelayCommand]
        public async Task AddNewRole()
        {
            await MopupService.Instance.PushAsync(new AddOrEditRolePopup(_rolesService, ServicesHelper.GetService<EntryValidatorService>()));
        }

        [RelayCommand]
        public async Task EditRole(EmployeeRole role)
        {
            await MopupService.Instance.PushAsync(new AddOrEditRolePopup(_rolesService, ServicesHelper.GetService<EntryValidatorService>(), role));
        }

        [RelayCommand]
        public async Task RemoveRole(EmployeeRole role)
        {
            bool res = await Shell.Current.DisplayAlert("Confirmation", $"Are you sure you want to delete \"{role.Name}\" role? This cannot be undone", "Yes", "No");
            if (res) _rolesService.RemoveRole(role);
        }

        #endregion
    }
}
