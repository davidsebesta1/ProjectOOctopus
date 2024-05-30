using OfficeOpenXml;
using OfficeOpenXml.Style;
using ProjectOOctopus.Data;
using System.Globalization;

namespace ProjectOOctopus.Services
{
    /// <summary>
    /// Service class for exporting project data into .xmls file
    /// </summary>
    public class ExcelExporterService
    {
        public const double CellWidth = 10d;

        #region Properties

        private EmployeesService _employeesService;
        private ProjectsService _projectsService;
        private RolesService _rolesService;

        #endregion

        #region Ctor

        public ExcelExporterService(EmployeesService employeesService, ProjectsService projectsService, RolesService rolesService)
        {
            _employeesService = employeesService;
            _projectsService = projectsService;
            _rolesService = rolesService;
        }

        #endregion

        #region Service Methods

        public async Task Export(string path)
        {
            try
            {
                string finalPath = Path.Combine(path, $"ProjectOOctopusExport{DateTime.Now.ToString("dd-M-yyyy", CultureInfo.InvariantCulture)}.xlsx");

                if (File.Exists(finalPath))
                {
                    bool accept = await Shell.Current.DisplayAlert("Export Alert", "Export file in this folder already exists, are you sure you want to override it?", "Yes", "No");
                    if (!accept) return;
                }

                using (ExcelPackage package = new ExcelPackage())
                {
                    #region Projects
                    using ExcelWorksheet worksheetProjects = package.Workbook.Worksheets.Add("Projects");

                    WriteExportHeader(worksheetProjects);

                    int curRowIndexProjects = 3;
                    foreach (ProjectData projectData in _projectsService._allProjects)
                    {
                        curRowIndexProjects = WriteProjectData(worksheetProjects, projectData, curRowIndexProjects) + 1;
                    }

                    for (int i = 1; i < 7; i++)
                    {
                        worksheetProjects.Column(i).Width = CellWidth;
                    }

                    #endregion

                    #region Employees and Known Roles

                    using ExcelWorksheet worksheetEmployees = package.Workbook.Worksheets.Add("Employees");

                    WriteExportHeader(worksheetEmployees);

                    int curRowIndexEmployees = 3;
                    //Header
                    ExcelRange headerCellsAll = worksheetEmployees.Cells[curRowIndexEmployees, 1, curRowIndexEmployees, 7];
                    headerCellsAll.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headerCellsAll.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(10197915));

                    //Emp name
                    ExcelRange headerEmpNameCells = worksheetEmployees.Cells[curRowIndexEmployees, 1, curRowIndexEmployees, 2];
                    headerEmpNameCells.Value = "Employee Name";
                    headerEmpNameCells.Merge = true;

                    //Pr name
                    ExcelRange headerPrNameCells = worksheetEmployees.Cells[curRowIndexEmployees, 3, curRowIndexEmployees, 4];
                    headerPrNameCells.Value = "Project Name";
                    headerPrNameCells.Merge = true;

                    //Role
                    ExcelRange headerRoleNameCells = worksheetEmployees.Cells[curRowIndexEmployees, 5, curRowIndexEmployees, 6];
                    headerRoleNameCells.Value = "Role";
                    headerRoleNameCells.Merge = true;

                    //Assignment
                    worksheetEmployees.Cells[curRowIndexEmployees, 7].Value = "Assignment";


                    //Writing employees
                    curRowIndexEmployees++;
                    foreach (Employee employee in _employeesService._allEmployees)
                    {
                        curRowIndexEmployees = WriteEmployesData(worksheetEmployees, employee, curRowIndexEmployees) + 1;
                    }

                    for (int i = 1; i < 8; i++)
                    {
                        worksheetEmployees.Column(i).Width = CellWidth;
                    }

                    int curRowKnownRolesIndex = 3;
                    int maxRolesCount = _employeesService._allEmployees.Any() ? _employeesService._allEmployees.Max(n => n.Roles.Count) : 0;

                    //Header
                    ExcelRange headerCellsAllRoles = worksheetEmployees.Cells[curRowKnownRolesIndex, 11, curRowKnownRolesIndex, 13 + maxRolesCount];
                    headerCellsAllRoles.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headerCellsAllRoles.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(10197915));

                    //Known role name
                    ExcelRange headerKnownRoleNameCells = worksheetEmployees.Cells[curRowKnownRolesIndex, 11, curRowKnownRolesIndex, 12];
                    headerKnownRoleNameCells.Value = "Employee Name";
                    headerKnownRoleNameCells.Merge = true;
                    headerKnownRoleNameCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    headerKnownRoleNameCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    //Known roles
                    ExcelRange headerRolesCells = worksheetEmployees.Cells[curRowKnownRolesIndex, 13, curRowKnownRolesIndex, 13 + maxRolesCount];
                    headerRolesCells.Value = "Roles";
                    headerRolesCells.Merge = true;
                    headerRolesCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    headerRolesCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    curRowKnownRolesIndex++;

                    //Writing known roles
                    foreach (Employee employee in _employeesService._allEmployees)
                    {
                        curRowKnownRolesIndex = WriteKnownRoles(worksheetEmployees, employee, curRowKnownRolesIndex) + 1;
                    }

                    for (int i = 11; i < 13 + maxRolesCount; i++)
                    {
                        worksheetEmployees.Column(i).Width = CellWidth * (i >= 13 ? 1.5d : 1d);
                    }

                    #endregion

                    FileInfo file = new FileInfo(finalPath);
                    package.SaveAs(file);
                }

                await Shell.Current.DisplayAlert("Export", "Export successfull!", "Return");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Export", ex.GetBaseException().Message, "Okay");
            }
        }

        private void WriteExportHeader(ExcelWorksheet worksheet)
        {
            worksheet.Cells[1, 1].Value = $"Project Organization Octopus - Export {DateTime.Now.ToString(new CultureInfo("cz-CZ"))}";
            worksheet.Cells[1, 1, 1, 6].Merge = true;
        }

        private int WriteProjectData(ExcelWorksheet worksheet, ProjectData projectData, int startingRowIndex)
        {
            int currentRowIndex = startingRowIndex;

            //Project name
            ExcelRange prNameCell = worksheet.Cells[currentRowIndex, 1, currentRowIndex, 6];
            prNameCell.Value = projectData.ProjectName;

            prNameCell.Style.WrapText = true;
            prNameCell.Merge = true;

            prNameCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            prNameCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(15263976));

            currentRowIndex++;

            //Project description
            ExcelRange prDescriptionCell = worksheet.Cells[currentRowIndex, 1, currentRowIndex, 6];
            prDescriptionCell.Value = projectData.ProjectDescription;

            prDescriptionCell.Style.WrapText = true;
            prDescriptionCell.Merge = true;

            prDescriptionCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            prDescriptionCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(15263976));

            currentRowIndex++;

            //Header
            ExcelRange prHeaderCell = worksheet.Cells[currentRowIndex, 1, currentRowIndex, 6];
            prHeaderCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            prHeaderCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(10197915));

            prHeaderCell.Value = "Role";
            prHeaderCell.Merge = true;

            prHeaderCell.Value = "Employee Name";
            prHeaderCell.Merge = true;

            prHeaderCell.Value = "Employee Assignment %";
            prHeaderCell.Merge = true;

            currentRowIndex++;

            foreach (AssignedRoleCollection col in projectData.EmployeesByRoles)
            {
                int startRowIndex = currentRowIndex;

                //No employees found
                if (col.Employees.Count == 0)
                {
                    int endRowIndex = currentRowIndex + col.TargetCount - 1;

                    ExcelRange noEmpCell = worksheet.Cells[startRowIndex, 3, endRowIndex, 6];
                    noEmpCell.Value = $"No employee{(col.TargetCount > 1 ? "s" : string.Empty)} assigned";
                    noEmpCell.Style.WrapText = true;
                    noEmpCell.Merge = true;
                    noEmpCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    noEmpCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    noEmpCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    noEmpCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(16773062));

                    ExcelRange roleCell = worksheet.Cells[startRowIndex, 1, endRowIndex, 2];
                    roleCell.Value = $"{col.Role.Name} ({$"{col.Employees.Count}/{col.TargetCount}"})";
                    roleCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    roleCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(col.Role.Color.ToInt()));
                    roleCell.Style.WrapText = true;
                    roleCell.Merge = true;

                    roleCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    roleCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    currentRowIndex += col.TargetCount;
                }
                else
                {
                    //Employee name and assignment
                    foreach (AssignedEmployeeData employee in col.Employees)
                    {

                        ExcelRange nameCell = worksheet.Cells[currentRowIndex, 3, currentRowIndex, 4];
                        nameCell.Value = employee.Employee.FullName;
                        nameCell.Style.WrapText = true;
                        nameCell.Merge = true;

                        ExcelRange assignmentCell = worksheet.Cells[currentRowIndex, 5, currentRowIndex, 6];
                        assignmentCell.Value = employee.Employee.GetAssignentUsage(projectData, col) + "%";
                        assignmentCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        assignmentCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        assignmentCell.Merge = true;

                        //Assignment warning over 100%
                        if (employee.Employee.TotalAssignmentUsage > 100)
                        {
                            assignmentCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            assignmentCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(16249906));
                        }

                        currentRowIndex++;
                    }

                    int endRowIndex = currentRowIndex - 1;

                    ExcelRange roleCell = worksheet.Cells[startRowIndex, 1, endRowIndex, 2];
                    roleCell.Value = $"{col.Role.Name} ({$"{col.Employees.Count}/{col.TargetCount}"})";
                    roleCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    roleCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(col.Role.Color.ToInt()));
                    roleCell.Style.WrapText = true;
                    roleCell.Merge = true;

                    roleCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    roleCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
            }

            return currentRowIndex;
        }

        private int WriteEmployesData(ExcelWorksheet worksheet, Employee employee, int startingRowIndex)
        {
            int currentRowIndex = startingRowIndex;

            int sum = employee._assignmentsPerctangeByProject.Sum(n => n.Value.Count);
            if (sum == 0)
            {
                ExcelRange empName = worksheet.Cells[currentRowIndex, 1, currentRowIndex, 2];
                empName.Value = employee.FullName;
                empName.Merge = true;
                empName.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                empName.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ExcelRange prNameCells = worksheet.Cells[currentRowIndex, 3, currentRowIndex, 4];
                prNameCells.Value = "-";
                prNameCells.Merge = true;
                prNameCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                prNameCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ExcelRange roleNameCells = worksheet.Cells[currentRowIndex, 5, currentRowIndex, 6];
                roleNameCells.Value = "-";
                roleNameCells.Merge = true;
                roleNameCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                roleNameCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ExcelRange roleAssignmentCells = worksheet.Cells[currentRowIndex, 7];
                roleAssignmentCells.Value = "-";
                roleAssignmentCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                roleAssignmentCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                roleAssignmentCells.Merge = true;

                currentRowIndex++;

                ExcelRange roleAssignmentTotalCells = worksheet.Cells[currentRowIndex, 7];
                roleAssignmentTotalCells.Value = employee.TotalAssignmentUsage + "%";
                roleAssignmentTotalCells.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(-569311));
                roleAssignmentTotalCells.Style.Font.Bold = true;
                roleAssignmentTotalCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                roleAssignmentTotalCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                roleAssignmentTotalCells.Merge = true;


                return currentRowIndex + 1;
            }

            int totalRequiredRows = employee._assignmentsPerctangeByProject.Sum(n => n.Value.Count) - 1;

            ExcelRange empCell = worksheet.Cells[currentRowIndex, 1, currentRowIndex + totalRequiredRows, 2];
            empCell.Value = employee.FullName;
            empCell.Style.WrapText = true;
            empCell.Merge = true;
            empCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            empCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            foreach (var kvp in employee._assignmentsPerctangeByProject)
            {
                if (kvp.Value.Count == 0) continue;

                int projectRequiredRows = kvp.Value.Count - 1;

                ExcelRange prNameCell = worksheet.Cells[currentRowIndex, 3, currentRowIndex + projectRequiredRows, 4];
                prNameCell.Value = kvp.Key.ProjectName;
                prNameCell.Style.WrapText = true;
                prNameCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                prNameCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                prNameCell.Merge = true;

                foreach (var roleKvp in kvp.Value)
                {

                    ExcelRange roleNameCell = worksheet.Cells[currentRowIndex, 5, currentRowIndex, 6];
                    roleNameCell.Value = roleKvp.Key.Role.Name;
                    roleNameCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    roleNameCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    roleNameCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(roleKvp.Key.Role.Color.ToInt()));
                    roleNameCell.Merge = true;

                    ExcelRange assignmentCellLocal = worksheet.Cells[currentRowIndex, 7];
                    assignmentCellLocal.Value = roleKvp.Value + "%";
                    assignmentCellLocal.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    assignmentCellLocal.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    assignmentCellLocal.Merge = true;

                    currentRowIndex++;
                }
            }


            System.Drawing.Color textColor;
            if (employee.TotalAssignmentUsage < 100) textColor = System.Drawing.Color.FromArgb(-569311);
            else if (employee.TotalAssignmentUsage > 100) textColor = System.Drawing.Color.FromArgb(-536287);
            else textColor = System.Drawing.Color.FromArgb(-12527265);

            ExcelRange assignmentCell = worksheet.Cells[currentRowIndex, 7];
            assignmentCell.Value = employee.TotalAssignmentUsage + "%";
            assignmentCell.Style.Font.Color.SetColor(textColor);
            assignmentCell.Style.Font.Bold = true;
            assignmentCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            assignmentCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            assignmentCell.Merge = true;

            currentRowIndex++;

            return currentRowIndex;
        }

        private int WriteKnownRoles(ExcelWorksheet worksheet, Employee employee, int currentRowIndex)
        {
            ExcelRange empNameCell = worksheet.Cells[currentRowIndex, 11, currentRowIndex, 12];
            empNameCell.Value = employee.FullName;
            empNameCell.Style.WrapText = true;
            empNameCell.Merge = true;
            empNameCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            empNameCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            for (int i = 0; i < employee.Roles.Count; i++)
            {
                EmployeeRole role = employee.Roles[i];
                ExcelRange cell = worksheet.Cells[currentRowIndex, 13 + i];
                cell.Value = role.Name;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(role.Color.ToInt()));
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            return currentRowIndex + 1;
        }

        #endregion
    }
}