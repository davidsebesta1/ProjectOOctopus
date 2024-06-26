﻿using ProjectOOctopus.Data;

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