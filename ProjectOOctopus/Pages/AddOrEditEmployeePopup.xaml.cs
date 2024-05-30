using Mopups.Pages;
using Mopups.Services;
using ProjectOOctopus.Data;
using ProjectOOctopus.Services;
using System.Collections.ObjectModel;

namespace ProjectOOctopus.Pages;

public partial class AddOrEditEmployeePopup : PopupPage
{
    #region Properties
    private Employee _employee;

    private readonly EmployeesService _employeesService;
    private readonly RolesService _rolesService;

    private EntryValidatorService _entryValidatorService;

    #endregion

    #region Ctor

    public AddOrEditEmployeePopup(EmployeesService projectsService, RolesService rolesService, EntryValidatorService entryValidatorService, Employee employee = null)
    {
        InitializeComponent();

        _employeesService = projectsService;
        _employee = employee;
        _rolesService = rolesService;
        _entryValidatorService = entryValidatorService;

        RolesCollectionView.ItemsSource = _rolesService.Roles;

        if (_employee != null)
        {
            EmpFirstNameEntry.Text = _employee.FirstName;
            EmpLastNameEntry.Text = _employee.LastName;

            AddOrEditButton.Text = "Edit";
        }
    }

    #endregion

    #region Page methods and events

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

        _entryValidatorService.TryRegisterValidation("FirstName", EmpFirstNameEntry, (text) => !string.IsNullOrEmpty(text), FirstNameErrText);
        _entryValidatorService.TryRegisterValidation("LastName", EmpLastNameEntry, (text) => !string.IsNullOrEmpty(text), LastNameErrText);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _entryValidatorService.ClearAllValidations();
    }

    private async void AddOrEditButton_Clicked(object sender, EventArgs e)
    {
        _entryValidatorService.RevalidateAll();
        if (!_entryValidatorService.AllValid)
        {
            await Shell.Current.DisplayAlert("Validation", "Please fix any invalid input fields and try again", "Okay");
            return;
        }

        string firstname = EmpFirstNameEntry.Text.Trim();
        string lastname = EmpLastNameEntry.Text.Trim();
        if (_employee == null)
        {
            ObservableCollection<EmployeeRole> _roles = new ObservableCollection<EmployeeRole>();

            foreach (EmployeeRole role in RolesCollectionView.SelectedItems.Cast<EmployeeRole>())
            {
                _roles.Add(role);
            }

            _employee = new Employee(firstname, lastname, _roles);
            _employeesService.AddEmployee(_employee);
        }
        else
        {
            _employee.FirstName = firstname;
            _employee.LastName = lastname;

            _employee.Roles.Clear();
            foreach (EmployeeRole role in RolesCollectionView.SelectedItems.Cast<EmployeeRole>())
            {
                _employee.Roles.Add(role);
            }
        }

        await MopupService.Instance.PopAsync();
    }

    #endregion
}