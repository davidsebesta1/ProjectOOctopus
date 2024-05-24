using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOOctopus.Data
{
    public class AssignedRoleCollection : ObservableCollection<Employee>
    {
        public EmployeeRole Role { get; set; }

        public AssignedRoleCollection(EmployeeRole role)
        {
            Role = role;
        }
    }
}
