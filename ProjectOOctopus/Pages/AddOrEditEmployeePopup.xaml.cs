using Mopups.Pages;
using Mopups.Services;
using ProjectOOctopus.Data;
using ProjectOOctopus.Services;

namespace ProjectOOctopus.Pages;

public partial class AddOrEditEmployeePopup : PopupPage
{
    private EmployeesService _employeesService;
    private Employee _employee;

    public AddOrEditEmployeePopup(EmployeesService projectsService, Employee employee = null)
    {
        InitializeComponent();
        _employeesService = projectsService;
        _employee = employee;

        if (_employee != null)
        {
            EmpFirstNameEntry.Text = _employee.FirstName;
            EmpLastNameEntry.Text = _employee.LastName;

            AddOrEditButton.Text = "Edit";
        }
    }

    private async void AddOrEditButton_Clicked(object sender, EventArgs e)
    {
        if (_employee == null)
        {
            _employee = new Employee(EmpFirstNameEntry.Text, EmpLastNameEntry.Text);
            _employeesService.AddEmployee(_employee);
        }
        else
        {
            _employee.FirstName = EmpFirstNameEntry.Text;
            _employee.LastName = EmpLastNameEntry.Text;
        }

        await MopupService.Instance.PopAsync();
    }
}