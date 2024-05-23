using Mopups.Pages;
using Mopups.Services;
using ProjectOOctopus.Data;
using ProjectOOctopus.Services;

namespace ProjectOOctopus.Pages;

public partial class AddOrEditRolePopup : PopupPage
{
    private readonly RolesService _rolesService;

    public AddOrEditRolePopup(RolesService rolesService)
    {
        InitializeComponent();
        _rolesService = rolesService;
    }

    private async void AddOrEditButton_Clicked(object sender, EventArgs e)
    {
        EmployeeRole role = new EmployeeRole(RoleNameEntry.Text, EmployeeRole.LastColor);

        float newHueTotal = (EmployeeRole.LastColor.GetHue() * 360 + 36);
        float hueClamped = newHueTotal % 360;

        bool overflow = newHueTotal > 360f;


        Color newColor = Color.FromHsv(hueClamped / 360f, 1f, 0.87f);
        EmployeeRole.LastColor = newColor;

        _rolesService.AddRole(role);
        await MopupService.Instance.PopAsync();
    }
}