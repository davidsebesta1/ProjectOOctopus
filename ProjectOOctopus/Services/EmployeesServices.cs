using ProjectOOctopus.Data;
using System.Collections.ObjectModel;

namespace ProjectOOctopus.Services
{
    public class EmployeesService
    {
        private ProjectsService _projectsService;

        public EmployeesService(ProjectsService projectsService)
        {
            _projectsService = projectsService;
        }

        private string _currentSearch = string.Empty;

        private readonly ObservableCollection<Employee> _allEmployees = new ObservableCollection<Employee>();
        public ObservableCollection<Employee> Employees { get; private set; } = new ObservableCollection<Employee>();

        public bool HideEmployeeIfFullyAssigned = false;

        public void AddEmployee(Employee employee)
        {
            _allEmployees.Add(employee);

            TryAddEmployeeByNameFilter(employee);
        }

        public void RemoveEmployee(Employee employee)
        {
            _allEmployees.Remove(employee);
            Employees.Remove(employee);

            _projectsService.RemoveEmployeeFromAllProjects(employee);

            employee.Dispose();
        }

        public void Refresh()
        {
            SearchByName(_currentSearch);
        }

        public void SearchByName(string name)
        {
            _currentSearch = name;
            if (string.IsNullOrEmpty(_currentSearch))
            {
                Employees.Clear();
                foreach (Employee emp in _allEmployees)
                {
                    if (HideEmployeeIfFullyAssigned)
                    {
                        if (emp.TotalAssignmentUsage != 100)
                        {
                            Employees.Add(emp);
                        }

                        continue;
                    }

                    Employees.Add(emp);
                }

                return;
            }

            Employees.Clear();
            foreach (Employee emp in _allEmployees)
            {
                TryAddEmployeeByNameFilter(emp);
            }
        }

        private void TryAddEmployeeByNameFilter(Employee employee)
        {
            if (employee.FullName.Contains(_currentSearch, StringComparison.InvariantCultureIgnoreCase))
            {
                if (HideEmployeeIfFullyAssigned)
                {
                    if (employee.TotalAssignmentUsage != 100)
                    {
                        Employees.Add(employee);
                    }

                    return;
                }

                Employees.Add(employee);
            }
        }
    }
}
