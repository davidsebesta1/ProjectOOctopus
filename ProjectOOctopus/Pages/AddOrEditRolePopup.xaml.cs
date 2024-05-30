using Mopups.Pages;
using Mopups.Services;
using ProjectOOctopus.Data;
using ProjectOOctopus.Services;

namespace ProjectOOctopus.Pages;

public partial class AddOrEditRolePopup : PopupPage
{
    #region Properties

    private readonly RolesService _rolesService;
    private EmployeeRole _role;

    private EntryValidatorService _validatorService;

    #endregion

    #region Ctor

    public AddOrEditRolePopup(RolesService rolesService, EntryValidatorService entryValidatorService, EmployeeRole role = null)
    {
        InitializeComponent();
        _validatorService = entryValidatorService;
        _rolesService = rolesService;
        _role = role;
    }

    #endregion

    #region Page methods and events

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _validatorService.TryRegisterValidation("RoleName", RoleNameEntry, (text) => !string.IsNullOrEmpty(text), RoleNameErrText);

        if (_role != null)
        {
            RoleNameEntry.Text = _role.Name;
            RedSlider.Value = _role.Color.Red * 255;
            BlueSlider.Value = _role.Color.Blue * 255;
            GreenSlider.Value = _role.Color.Green * 255;

            AddOrEditButton.Text = "Edit";
        }
        else
        {
            RedSlider.Value = 128;
            BlueSlider.Value = 128;
            GreenSlider.Value = 128;
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _validatorService.ClearAllValidations();
    }

    private async void AddOrEditButton_Clicked(object sender, EventArgs e)
    {
        _validatorService.RevalidateAll();
        if (!_validatorService.AllValid)
        {
            await Shell.Current.DisplayAlert("Validation", "Please fix any invalid input fields and try again", "Okay");
            return;
        }

        string roleName = RoleNameEntry.Text.Trim();
        if (_role == null)
        {
            _role = new EmployeeRole(roleName, PreviewColorFrame.BackgroundColor);
            _rolesService.AddRole(_role);
        }
        else
        {
            _role.Name = roleName;
            _role.Color = PreviewColorFrame.BackgroundColor;
        }

        await MopupService.Instance.PopAsync();
    }

    private void AnySlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        PreviewColorFrame.BackgroundColor = Color.FromRgb((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value);
    }

    #endregion
}