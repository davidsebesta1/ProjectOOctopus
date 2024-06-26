﻿using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectOOctopus.Data
{

    /// <summary>
    /// Temporary class used to display employee data by its assignment
    /// </summary>
    public partial class AssignedEmployeeData : ObservableObject, IEquatable<AssignedEmployeeData?>
    {
        #region Properties

        [ObservableProperty]
        private Employee _employee;

        [ObservableProperty]
        private AssignedRoleCollection _assignedRoles;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AssignmentUsageString))]
        private int _localAssignmentUsage;

        public string AssignmentUsageString => $"{LocalAssignmentUsage}%";

        #endregion

        #region Ctor

        public AssignedEmployeeData(Employee employee, AssignedRoleCollection assignedRoles, int localAssignmentUsage)
        {
            Employee = employee;
            AssignedRoles = assignedRoles;
            LocalAssignmentUsage = localAssignmentUsage;
        }
        #endregion

        #region Object methods and operators

        public override bool Equals(object? obj)
        {
            return Equals(obj as AssignedEmployeeData);
        }

        public bool Equals(AssignedEmployeeData? other)
        {
            return other is not null &&
                   EqualityComparer<Employee>.Default.Equals(Employee, other.Employee);
        }

        public override int GetHashCode()
        {
            return _employee.GetHashCode();
        }

        public static bool operator ==(AssignedEmployeeData? left, AssignedEmployeeData? right)
        {
            return EqualityComparer<AssignedEmployeeData>.Default.Equals(left, right);
        }

        public static bool operator !=(AssignedEmployeeData? left, AssignedEmployeeData? right)
        {
            return !(left == right);
        }

        #endregion
    }
}
