using Mopups.Pages;
using Mopups.Services;
using ProjectOOctopus.Data;
using ProjectOOctopus.Services;
using System.Collections.ObjectModel;

namespace ProjectOOctopus.Pages;

public partial class AddEmployeePopup : PopupPage
{
    private readonly ProjectData _data;
    private readonly EmployeesService _employeesService;

    public AddEmployeePopup(EmployeesService empService, ProjectData data)
    {
        InitializeComponent();
        _data = data;
        _employeesService = empService;
    }

    private async void AddOrEditButton_Clicked(object sender, EventArgs e)
    {
        Employee emp = new Employee(EmpFirstNameEntry.Text, EmpLastNameEntry.Text);

        _employeesService.Employees.Add(emp);
        _data.AssignedEmployees.Add(emp);

        await MopupService.Instance.PopAsync();
    }
}