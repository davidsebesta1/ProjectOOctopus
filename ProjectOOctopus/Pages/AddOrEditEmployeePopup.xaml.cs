using Mopups.Pages;
using Mopups.Services;
using ProjectOOctopus.Data;
using ProjectOOctopus.Services;
using System.Collections.ObjectModel;

namespace ProjectOOctopus.Pages;

public partial class AddOrEditEmployeePopup : PopupPage
{
    private Employee _employee;

    private readonly EmployeesService _employeesService;
    private readonly RolesService _rolesService;

    public AddOrEditEmployeePopup(EmployeesService projectsService, RolesService rolesService, Employee employee = null)
    {
        InitializeComponent();

        _employeesService = projectsService;
        _employee = employee;
        _rolesService = rolesService;

        RolesCollectionView.ItemsSource = _rolesService.Roles;

        if (_employee != null)
        {
            EmpFirstNameEntry.Text = _employee.FirstName;
            EmpLastNameEntry.Text = _employee.LastName;

            AddOrEditButton.Text = "Edit";
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_employee != null)
        {
            foreach (EmployeeRole role in _employee.Roles)
            {
                RolesCollectionView.SelectedItems.Add(role);
                //Frame el = RolesCollectionView.GetVisualTreeDescendants().First(n => (n as Frame)?.BindingContext == role) as Frame;
                //VisualStateManager.GoToState(el, "Selected");
            }

        }
    }

    private async void AddOrEditButton_Clicked(object sender, EventArgs e)
    {
        if (_employee == null)
        {
            ObservableCollection<EmployeeRole> _roles = new ObservableCollection<EmployeeRole>();
            string firstname = EmpFirstNameEntry.Text;
            string lastname = EmpLastNameEntry.Text;

            foreach (EmployeeRole role in RolesCollectionView.SelectedItems.Cast<EmployeeRole>())
            {
                _roles.Add(role);
            }

            _employee = new Employee(firstname, lastname, _roles);
            _employeesService.AddEmployee(_employee);
        }
        else
        {
            _employee.FirstName = EmpFirstNameEntry.Text;
            _employee.LastName = EmpLastNameEntry.Text;

            _employee.Roles.Clear();
            foreach (EmployeeRole role in RolesCollectionView.SelectedItems)
            {
                _employee.Roles.Add(role);
            }
        }

        await MopupService.Instance.PopAsync();
    }
}