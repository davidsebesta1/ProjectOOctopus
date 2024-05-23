using ProjectOOctopus.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOOctopus.Services
{
    public class EmployeesService
    {
        public EmployeesService() { }

        public ObservableCollection<Employee> Employees { get; private set; } = new ObservableCollection<Employee>();
    }
}
