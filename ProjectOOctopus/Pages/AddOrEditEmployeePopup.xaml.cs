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

    private readonly Dictionary<string, bool> _validationValues = new Dictionary<string, bool>()
    {
        {"FirstName", false },
        {"LastName", false }
    };

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
            }
        }
    }

    private async void AddOrEditButton_Clicked(object sender, EventArgs e)
    {
        if (_validationValues.Any(n => !n.Value))
        {
            await Shell.Current.DisplayAlert("Validation", "Please fix any invalid input fields and try again", "Okay");
            return;
        }

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
            foreach (EmployeeRole role in RolesCollectionView.SelectedItems.Cast<EmployeeRole>())
            {
                _employee.Roles.Add(role);
            }
        }

        await MopupService.Instance.PopAsync();
    }

    private void EmpFirstNameEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        bool res = !string.IsNullOrEmpty(e.NewTextValue);

        FirstNameErrText.IsVisible = !res;
        _validationValues["FirstName"] = res;
    }

    private void EmpLastNameEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        bool res = !string.IsNullOrEmpty(e.NewTextValue);

        LastNameErrText.IsVisible = !res;
        _validationValues["LastName"] = res;
    }
}