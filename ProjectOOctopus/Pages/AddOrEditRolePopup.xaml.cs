using Mopups.Pages;
using Mopups.Services;
using ProjectOOctopus.Data;
using ProjectOOctopus.Services;

namespace ProjectOOctopus.Pages;

public partial class AddOrEditRolePopup : PopupPage
{
    private readonly RolesService _rolesService;
    private EmployeeRole _role;

    private readonly Dictionary<string, bool> _validationValues = new Dictionary<string, bool>()
    {
        {"RoleName", false },
    };

    public AddOrEditRolePopup(RolesService rolesService, EmployeeRole role = null)
    {
        InitializeComponent();
        _rolesService = rolesService;
        _role = role;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

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

    private async void AddOrEditButton_Clicked(object sender, EventArgs e)
    {
        if (_validationValues.Any(n => !n.Value))
        {
            await Shell.Current.DisplayAlert("Validation", "Please fix any invalid input fields and try again", "Okay");
            return;
        }

        if (_role == null)
        {
            _role = new EmployeeRole(RoleNameEntry.Text, PreviewColorFrame.BackgroundColor);
            _rolesService.AddRole(_role);
        }
        else
        {
            _role.Name = RoleNameEntry.Text;
            _role.Color = PreviewColorFrame.BackgroundColor;
        }

        await MopupService.Instance.PopAsync();
    }

    private void AnySlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        PreviewColorFrame.BackgroundColor = Color.FromRgb((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value);
    }

    private void RoleNameEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        bool res = !string.IsNullOrEmpty(e.NewTextValue);

        RoleNameErrText.IsVisible = !res;
        _validationValues["RoleName"] = res;
    }
}