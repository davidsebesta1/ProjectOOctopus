using Mopups.Pages;
using Mopups.Services;
using ProjectOOctopus.Data;
using ProjectOOctopus.Services;

namespace ProjectOOctopus.Pages;

public partial class AddEmployeePopup : PopupPage
{
    private readonly EmployeesService _employeesService;

    public AddEmployeePopup(EmployeesService empService)
    {
        InitializeComponent();
        _employeesService = empService;
    }

    private async void AddOrEditButton_Clicked(object sender, EventArgs e)
    {
        Employee emp = new Employee(EmpFirstNameEntry.Text, EmpLastNameEntry.Text);

        _employeesService.AddEmployee(emp);

        await MopupService.Instance.PopAsync();
    }
}