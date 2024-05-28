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

        private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection == null || !e.CurrentSelection.Any()) return;

            ProjectData selected = e.CurrentSelection[0] as ProjectData;

            return;
        }

        private void DragStartEmployee(object sender, DragStartingEventArgs e)
        {
            e.Data.Properties.Add("Employee", (sender as DragGestureRecognizer).BindingContext as Employee);
        }

        private async void DropGestureRecognizer_Drop(object sender, DropEventArgs e)
        {
            if (e.Data.Properties.TryGetValue("Employee", out object obj))
            {
                DropGestureRecognizer dropGestureRecognizer = sender as DropGestureRecognizer;

                AssignedRoleCollection targetRoleGroup = dropGestureRecognizer.BindingContext as AssignedRoleCollection;
                ProjectData targetProjectData = dropGestureRecognizer.GetParent(3).BindingContext as ProjectData;
                Employee targetEmployee = obj as Employee;

                if (targetRoleGroup.Contains(targetEmployee))
                {
                    await Shell.Current.DisplayAlert("Error", "Unable to assign a one employee to the same project group twice in a single project", "Okay");
                    return;
                }

                if (e.Data.Properties.TryGetValue("OriginalGroup", out object group) && e.Data.Properties.TryGetValue("OriginalAssignmentUsage", out object usage) && group is AssignedRoleCollection originalCollection && originalCollection == targetRoleGroup)
                {
                    int originalusage = (int)usage;

                    originalCollection.Add(targetEmployee, originalusage);

                    await Shell.Current.DisplayAlert("Error", "Unable to assign a one employee to the same project group twice in a single project", "Okay");
                    return;
                }


                await MopupService.Instance.PushAsync(new AssignEmployeePopup(targetEmployee, targetProjectData, targetRoleGroup));
            }
        }

        private void SearchProjectByName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.SearchProjectsByNameCommand.Execute(e.NewTextValue);
        }

        private void SearchEmployeeByName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.SearchEmployeesByNameCommand.Execute(e.NewTextValue);
        }

        private void DragStartEmployeeReAssign(object sender, DragStartingEventArgs e)
        {
            DragGestureRecognizer dragGestureRecognizer = sender as DragGestureRecognizer;
            Employee employee = dragGestureRecognizer.BindingContext as Employee;

            AssignedRoleCollection assignedRoleCollection = dragGestureRecognizer.GetParent(4).BindingContext as AssignedRoleCollection;
            ProjectData data = dragGestureRecognizer.GetParent(7).BindingContext as ProjectData;
            e.Data.Properties.Add("Employee", employee);
            e.Data.Properties.Add("OriginalGroup", assignedRoleCollection);
            e.Data.Properties.Add("OriginalAssignmentUsage", employee.GetAssignentUsage(data, assignedRoleCollection));

            AssignedRoleCollection collection = dragGestureRecognizer.GetParent(3).BindingContext as AssignedRoleCollection;
            collection.Remove(employee);
        }

        private async void EditEmployeeFlyout_Clicked(object sender, EventArgs e)
        {
            await _viewModel.EditEmployeeCommand.ExecuteAsync((sender as MenuFlyoutItem).GetParent(2).BindingContext as Employee);
        }

        private async void RemoveEmployeeFlyout_Clicked(object sender, EventArgs e)
        {
            await _viewModel.RemoveEmployeeCommand.ExecuteAsync((sender as MenuFlyoutItem).GetParent(2).BindingContext as Employee);
        }

        private void LocalGroupEmpLabel_Loaded(object sender, EventArgs e)
        {
            Label label = sender as Label;
            Employee emp = label.BindingContext as Employee;
            AssignedRoleCollection col = label.GetParent(5).BindingContext as AssignedRoleCollection;
            ProjectData data = label.GetParent(9).BindingContext as ProjectData;

            label.Text = emp.GetAssignentUsage(data, col) + "%";
        }
    }
}