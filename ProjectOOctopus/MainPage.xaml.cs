using ProjectOOctopus.Data;
using ProjectOOctopus.Services;
using ProjectOOctopus.ViewModels;

namespace ProjectOOctopus
{
    public partial class MainPage : ContentPage
    {
        private EmployeesService _employeesService;

        public MainPage(MainPageViewModel vm, EmployeesService service)
        {
            InitializeComponent();
            BindingContext = vm;
            _employeesService = service;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MainPageViewModel vm = BindingContext as MainPageViewModel;
        }

        private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection == null || !e.CurrentSelection.Any()) return;

            ProjectData selected = e.CurrentSelection[0] as ProjectData;

            return;

        }

        private void DragGestureRecognizer_DragStarting(object sender, DragStartingEventArgs e)
        {
            e.Data.Properties.Add("Employee", (sender as DragGestureRecognizer).BindingContext as Employee);
        }

        private void DropGestureRecognizer_Drop(object sender, DropEventArgs e)
        {
            if (e.Data.Properties.TryGetValue("Employee", out var obj))
            {
                Employee emp = obj as Employee;

                ProjectData project = (sender as DropGestureRecognizer).BindingContext as ProjectData;
                project.AssignedEmployees.Add(emp);
            }
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            MainPageViewModel vm = BindingContext as MainPageViewModel;
            vm.SearchProjectsByNameCommand.Execute(e.NewTextValue);
        }
    }

}