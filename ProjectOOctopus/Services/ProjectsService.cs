using ProjectOOctopus.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOOctopus.Services
{
    public class ProjectsService
    {
        public ProjectsService() { }

        public ObservableCollection<ProjectData> Projects { get; private set; } = new ObservableCollection<ProjectData>();
    }
}
