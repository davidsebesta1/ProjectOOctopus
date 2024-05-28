using Mopups.Pages;
using ProjectOOctopus.ViewModels;

namespace ProjectOOctopus.Pages;

public partial class RoleManagerPopup : PopupPage
{
    public RoleManagerPopup(RoleManagerViewModel roleManagerViewModel)
    {
        InitializeComponent();
        BindingContext = roleManagerViewModel;
    }
}