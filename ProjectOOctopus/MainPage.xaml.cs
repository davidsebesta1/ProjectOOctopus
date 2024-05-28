using Mopups.Services;
using ProjectOOctopus.Data;
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
            if (e.Data.Properties.TryGetValue("Employee", out var obj))
            {
                DropGestureRecognizer dropGestureRecognizer = sender as DropGestureRecognizer;

                Employee emp = obj as Employee;
                AssignedRoleCollection assignedRoles = dropGestureRecognizer.BindingContext as AssignedRoleCollection;
                ProjectData data = dropGestureRecognizer.Parent.Parent.Parent.BindingContext as ProjectData;

                await MopupService.Instance.PushAsync(new AssignEmployeePopup(emp, data, assignedRoles));
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
            e.Data.Properties.Add("Employee", employee);

            AssignedRoleCollection collection = dragGestureRecognizer.Parent.Parent.Parent.BindingContext as AssignedRoleCollection;
            collection.Remove(employee);
        }

        private void EditEmployeeFlyout_Clicked(object sender, EventArgs e)
        {
            _viewModel.EditEmployeeCommand.Execute((sender as MenuFlyoutItem).Parent.Parent.BindingContext as Employee);
        }

        private void RemoveEmployeeFlyout_Clicked(object sender, EventArgs e)
        {
            _viewModel.RemoveEmployeeCommand.Execute((sender as MenuFlyoutItem).Parent.Parent.BindingContext as Employee);
        }
    }
}