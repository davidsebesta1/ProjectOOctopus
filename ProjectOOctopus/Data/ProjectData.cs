using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOOctopus.Data
{
    public partial class ProjectData : ObservableObject
    {
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }

        [ObservableProperty]
        private ObservableCollection<Employee> _assignedEmployees = new ObservableCollection<Employee>();

        public ProjectData(string projectName, string projectDescription)
        {
            ProjectName = projectName;
            ProjectDescription = projectDescription;
        }
    }
}
