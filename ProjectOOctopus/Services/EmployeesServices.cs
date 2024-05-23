using ProjectOOctopus.Data;
using System.Collections.ObjectModel;

namespace ProjectOOctopus.Services
{
    public class EmployeesService
    {
        public EmployeesService() { }

        private string _currentSearch = string.Empty;

        private readonly ObservableCollection<Employee> _allEmployees = new ObservableCollection<Employee>();
        public ObservableCollection<Employee> Employees { get; private set; } = new ObservableCollection<Employee>();

        public void AddEmployee(Employee emp)
        {
            _allEmployees.Add(emp);

            TryAddEmployeeNameCheck(emp);
        }

        public void SearchByName(string name)
        {
            _currentSearch = name;
            if (string.IsNullOrEmpty(_currentSearch))
            {
                Employees.Clear();
                foreach (Employee emp in _allEmployees)
                {
                    Employees.Add(emp);
                }

                return;
            }

            Employees.Clear();
            foreach (Employee emp in _allEmployees)
            {
                TryAddEmployeeNameCheck(emp);
            }
        }

        private void TryAddEmployeeNameCheck(Employee emp)
        {
            if (emp.FullName.Contains(_currentSearch, StringComparison.InvariantCultureIgnoreCase))
            {
                Employees.Add(emp);
            }
        }
    }
}
