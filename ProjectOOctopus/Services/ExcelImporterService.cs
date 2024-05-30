using OfficeOpenXml;
using ProjectOOctopus.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjectOOctopus.Services
{
    public class ExcelImporterService
    {
        private EmployeesService _employeesService;
        private ProjectsService _projectsService;
        private RolesService _rolesService;
        private Regex _fullRoleRegex = new Regex("^(.*?)\\s*\\(\\d+\\/\\d+\\)$");
        private Regex _roleTargetAndActiveCountRegex = new Regex("^\\s*\\((\\d+)\\/(\\d+)\\)$");

        public ExcelImporterService(EmployeesService employeesService, ProjectsService projectsService, RolesService rolesService)
        {
            _employeesService = employeesService;
            _projectsService = projectsService;
            _rolesService = rolesService;
        }

        public async Task Import(string fullPath)
        {
            try
            {
                if (!File.Exists(fullPath))
                {
                    await Shell.Current.DisplayAlert("Error", "File not found", "Okay");
                }

                FileInfo file = new FileInfo(fullPath);
                using ExcelPackage package = new ExcelPackage(file);

                using ExcelWorksheet employeesWorksheet = package.Workbook.Worksheets["Employees"];
                using ExcelWorksheet projectsWorksheet = package.Workbook.Worksheets["Projects"];

                //LoadRolesAndEmployees(employeesWorksheet);
                LoadProjects(projectsWorksheet);

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error while importing data", ex.GetBaseException().Message, "Okay");
            }
        }

        private void LoadProjects(ExcelWorksheet worksheet)
        {
            if (worksheet == null)
            {
                throw new Exception("Worksheet Employees not found");
            }

            for (int row = 3; row <= worksheet.Dimension.End.Row; row++)
            {
                string projectName = worksheet.Cells[row, 1].Text;
                if (string.IsNullOrEmpty(projectName))
                {
                    continue;
                }

                row++;
                string projectDescription = worksheet.Cells[row, 1].Text;
                row += 2;

                ExcelRange cell = worksheet.Cells[row, 1];
                string roleInfo = cell.Text;

                var result = _fullRoleRegex.Match(roleInfo);
                string roleName = 
            }
        }

        private void LoadRolesAndEmployees(ExcelWorksheet worksheet)
        {
            if (worksheet == null)
            {
                throw new Exception("Worksheet Employees not found");
            }

            for (int row = 4; row <= worksheet.Dimension.End.Row; row++)
            {
                if (string.IsNullOrEmpty(worksheet.Cells[row, 11].Text))
                {
                    continue;
                }

                string fullName = worksheet.Cells[row, 11].Text;

                string[] splitName = fullName.Split(' ');

                ObservableCollection<EmployeeRole> knownRoles = new ObservableCollection<EmployeeRole>();

                ExcelRange cell;
                int index = 13;
                while ((cell = worksheet.Cells[row, index]) != null & !string.IsNullOrEmpty(cell.Text))
                {
                    string roleName = cell.Text;

                    string hex = cell.Style.Fill.BackgroundColor.Rgb;
                    Color color = Color.FromHex(hex);

                    EmployeeRole role = _rolesService.Roles.FirstOrDefault(n => n.Name == roleName);
                    if (role == null)
                    {
                        role = new EmployeeRole(roleName, color);
                        _rolesService.AddRole(role);
                    }
                    knownRoles.Add(role);

                    index++;
                }
                Employee emp = new Employee(splitName[0], splitName[1], knownRoles);
                _employeesService.AddEmployee(emp);
            }
        }
    }
}