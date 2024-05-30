using OfficeOpenXml;
using ProjectOOctopus.Data;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace ProjectOOctopus.Services
{
    /// <summary>
    /// Service class for handling import from .xlsx into project
    /// </summary>
    public class ExcelImporterService
    {
        #region Properties

        private EmployeesService _employeesService;
        private ProjectsService _projectsService;
        private RolesService _rolesService;
        private Regex _fullRoleRegex = new Regex(@"^(.*?)\s*(\(\d+\/\d+\))$");
        private Regex _roleTargetAndActiveCountRegex = new Regex(@"^\s*\((\d+)\/(\d+)\)$");

        #endregion

        #region Ctor

        public ExcelImporterService(EmployeesService employeesService, ProjectsService projectsService, RolesService rolesService)
        {
            _employeesService = employeesService;
            _projectsService = projectsService;
            _rolesService = rolesService;
        }

        #endregion

        #region Service Methods

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

                LoadRolesAndEmployees(employeesWorksheet);
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
                ExcelRange cell = worksheet.Cells[row, 1];

                string projectName = worksheet.Cells[row, 1].Text;
                string projectDescription = worksheet.Cells[row + 1, 1].Text;
                string roleName;
                int targetEmployeeCount;

                if (string.IsNullOrEmpty(projectName))
                {
                    continue;
                }

                row += 3;

                ProjectData projectData = new ProjectData(projectName, projectDescription);

                ExcelRange roleCell;
                while ((roleCell = worksheet.Cells[row, 1]) != null & !string.IsNullOrEmpty(roleCell.Text))
                {
                    string roleInfo = roleCell.Text;

                    Match result = _fullRoleRegex.Match(roleInfo);
                    roleName = result.Groups[1].Value;

                    Match empCountMatch = _roleTargetAndActiveCountRegex.Match(result.Groups[2].Value);
                    targetEmployeeCount = int.Parse(empCountMatch.Groups[2].Value);

                    EmployeeRole role = _rolesService.Roles.FirstOrDefault(n => n.Name == roleName);
                    if (role == null)
                    {
                        string hex = roleCell.Style.Fill.BackgroundColor.Rgb;
                        Color color = Color.FromHex(hex);

                        role = new EmployeeRole(roleName, color);
                        _rolesService.AddRole(role);
                    }

                    AssignedRoleCollection collection = new AssignedRoleCollection(role, projectData, targetEmployeeCount);
                    projectData.AddRoleGroup(collection);

                    ExcelRange roleCellExtended;
                    while ((roleCellExtended = worksheet.Cells[row, 1]).Text == roleCell.Text)
                    {
                        ExcelRange testForEmptyCell = worksheet.Cells[row, 3];
                        if (testForEmptyCell.Text == "No employee assigned" || testForEmptyCell.Style.Fill.BackgroundColor.Rgb == "#FFFFEFC6")
                        {
                            row++;
                            continue;
                        }

                        string empFullName = worksheet.Cells[row, 3].Text;
                        string assignmentPercentText = worksheet.Cells[row, 5].Text;

                        int assingmentAmount = int.Parse(assignmentPercentText.Substring(0, assignmentPercentText.Length - 1));

                        Employee emp = _employeesService._allEmployees.FirstOrDefault(n => n.FullName == empFullName);
                        if (emp == null)
                        {
                            string[] splitName = empFullName.Split(' ');
                            emp = new Employee(splitName[0], splitName[1]);
                        }

                        collection.Add(emp, assingmentAmount);
                        row++;
                    }

                }
                projectData.ReOrderRoles();

                _projectsService.AddProject(projectData);
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

        #endregion
    }
}