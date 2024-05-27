using Mopups.Pages;
using ProjectOOctopus.Data;
using ProjectOOctopus.ViewModels;

namespace ProjectOOctopus.Pages;

public partial class RoleManagerPopup : PopupPage
{
    public RoleManagerPopup(RoleManagerViewModel roleManagerViewModel)
    {
        InitializeComponent();
        BindingContext = roleManagerViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Random random = new Random();
        RoleManagerViewModel vm = BindingContext as RoleManagerViewModel;

        for (int i = 0; i < 15; i++)
        {
            char[] nmArr = new char[8];

            for (int j = 0; j < nmArr.Length; j++)
            {
                nmArr[j] = (char)random.Next(65, 75);
            }

            vm._rolesService.AddRole(new EmployeeRole(new string(nmArr), Color.FromRgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255))));

        }
    }
}