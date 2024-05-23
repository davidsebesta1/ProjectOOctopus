using ProjectOOctopus.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOOctopus.Events
{
    public class RoleAddedEventArgs : EventArgs
    {
        public readonly EmployeeRole Role;

        public RoleAddedEventArgs(EmployeeRole role)
        {
            Role = role;
        }
    }
}
