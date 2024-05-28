using Mopups.Services;
using ProjectOOctopus.Data;
using ProjectOOctopus.Extensions;
using ProjectOOctopus.Pages;
using ProjectOOctopus.ViewModels;

namespace ProjectOOctopus
{
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel _viewModel;

        public MainPage(MainPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            _viewModel = vm;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await _viewModel.LoadBaseRolesCommand.ExecuteAsync(null);
        }

        private void DragStartEmployee(object sender, DragStartingEventArgs e)
        {
            IsBusy = true;
            e.Data.Properties.Add("Employee", (Employee)((DragGestureRecognizer)sender).BindingContext);
        }

        private async void DropGestureRecognizer_Drop(object sender, DropEventArgs e)
        {
            if (e.Data.Properties.TryGetValue("Employee", out object obj))
            {
                DropGestureRecognizer dropGestureRecognizer = (DropGestureRecognizer)sender;

                AssignedRoleCollection targetRoleGroup = (AssignedRoleCollection)dropGestureRecognizer.BindingContext;
                ProjectData targetProjectData = (ProjectData)dropGestureRecognizer.GetParent(3).BindingContext;
                Employee targetEmployee = obj is Employee ? (Employee)obj : ((AssignedEmployeeData)obj).Employee;

                if (targetRoleGroup.Any(n => n.Employee == targetEmployee))
                {
                    await Shell.Current.DisplayAlert("Error", "Unable to assign a one employee to the same project group twice in a single project", "Okay");
                    return;
                }

                if (e.Data.Properties.TryGetValue("OriginalGroup", out object group) && e.Data.Properties.TryGetValue("OriginalAssignmentUsage", out object usage) && e.Data.Properties.TryGetValue("OriginalProjectData", out object prData))
                {
                    AssignedRoleCollection originalCollection = (AssignedRoleCollection)group;
                    ProjectData project = (ProjectData)prData;

                    if (targetProjectData == project && targetRoleGroup == originalCollection)
                    {
                        int originalusage = (int)usage;

                        originalCollection.Add(targetEmployee, originalusage);
                        return;
                    }
                }

                await MopupService.Instance.PushAsync(new AssignEmployeePopup(targetEmployee, targetProjectData, targetRoleGroup));
            }

            IsBusy = false;
        }

        private void SearchProjectByName_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsBusy = true;
            _viewModel.SearchProjectsByNameCommand.Execute(e.NewTextValue);
            IsBusy = false;
        }

        private void SearchEmployeeByName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.SearchEmployeesByNameCommand.Execute(e.NewTextValue);
        }

        private void DragStartEmployeeReAssign(object sender, DragStartingEventArgs e)
        {
            DragGestureRecognizer dragGestureRecognizer = (DragGestureRecognizer)sender;
            AssignedEmployeeData employee = (AssignedEmployeeData)dragGestureRecognizer.BindingContext;

            AssignedRoleCollection assignedRoleCollection = (AssignedRoleCollection)dragGestureRecognizer.GetParent(4).BindingContext;
            ProjectData data = (ProjectData)dragGestureRecognizer.GetParent(7).BindingContext;
            e.Data.Properties.Add("Employee", employee);
            e.Data.Properties.Add("OriginalGroup", assignedRoleCollection);
            e.Data.Properties.Add("OriginalProject", data);
            e.Data.Properties.Add("OriginalAssignmentUsage", employee.Employee.GetAssignentUsage(data, assignedRoleCollection));

            AssignedRoleCollection collection = (AssignedRoleCollection)dragGestureRecognizer.GetParent(3).BindingContext;
            collection.Remove(employee);
        }

        private async void EditEmployeeFlyout_Clicked(object sender, EventArgs e)
        {
            IsBusy = true;
            await _viewModel.EditEmployeeCommand.ExecuteAsync((sender as MenuFlyoutItem).GetParent(2).BindingContext as Employee);
            IsBusy = false;
        }

        private async void RemoveEmployeeFlyout_Clicked(object sender, EventArgs e)
        {
            IsBusy = true;
            await _viewModel.RemoveEmployeeCommand.ExecuteAsync((sender as MenuFlyoutItem).GetParent(2).BindingContext as Employee);
            IsBusy = false;
        }

        private void LocalGroupEmpLabel_Loaded(object sender, EventArgs e)
        {
            IsBusy = true;
            Label label = (Label)sender;
            Employee emp = (Employee)label.BindingContext;
            AssignedRoleCollection col = (AssignedRoleCollection)label.GetParent(5).BindingContext;
            ProjectData data = (ProjectData)label.GetParent(9).BindingContext;

            label.Text = emp.GetAssignentUsage(data, col) + "%";
            IsBusy = false;
        }
    }
}