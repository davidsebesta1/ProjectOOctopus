using ProjectOOctopus.Data;
using ProjectOOctopus.Services;
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
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

        private void DropGestureRecognizer_Drop(object sender, DropEventArgs e)
        {
            if (e.Data.Properties.TryGetValue("Employee", out var obj))
            {
                Employee emp = obj as Employee;

                AssignedRoleCollection assignedRoles = (sender as DropGestureRecognizer).BindingContext as AssignedRoleCollection;
                assignedRoles.Add(emp);
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