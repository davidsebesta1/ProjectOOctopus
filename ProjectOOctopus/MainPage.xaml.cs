using ProjectOOctopus.Data;
using ProjectOOctopus.Services;
using ProjectOOctopus.ViewModels;
using System.Collections.ObjectModel;

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

            /*
            vm.Projects.Add(new ProjectData("Test", "A")
            {
                AssignedEmployees = new Dictionary<EmployeeSpecialization, ObservableCollection<Employee>>()
                {
                    {new EmployeeSpecialization("Tester"), new ObservableCollection<Employee>()
                        {
                            new Employee("Jan", "Kos")
                        }
                    }
                }
            });

            vm.Projects.Add(new ProjectData("Test2", "AAA")
            {
                AssignedEmployees = new Dictionary<EmployeeSpecialization, ObservableCollection<Employee>>()
                {
                    {new EmployeeSpecialization("Tester"), new ObservableCollection<Employee>()
                        {

                        }
                    }
                }
            });
            */
        }


        private void Button_Clicked(object sender, EventArgs e)
        {
            MainPageViewModel vm = (MainPageViewModel)BindingContext;
            Button button = sender as Button;
            ProjectData data = button.BindingContext as ProjectData;
            vm.AddEmployeeToProjectCommand.Execute(data);
        }

    }

}