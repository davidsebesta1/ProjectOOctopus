using ProjectOOctopus.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Globalization;

namespace ProjectOOctopus.Services
{
    public class ExcelExporterService
    {
        public const double CellWidth = 10d;
        private EmployeesService _employeesService;
        private ProjectsService _projectsService;
        private RolesService _rolesService;

        public ExcelExporterService(EmployeesService employeesService, ProjectsService projectsService, RolesService rolesService)
        {
            _employeesService = employeesService;
            _projectsService = projectsService;
            _rolesService = rolesService;
        }

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
            worksheet.Cells[currentRowIndex, 1, currentRowIndex, 6].Value = projectData.ProjectName;

            worksheet.Cells[currentRowIndex, 1, currentRowIndex, 6].Style.WrapText = true;
            worksheet.Cells[currentRowIndex, 1, currentRowIndex, 6].Merge = true;

            worksheet.Cells[currentRowIndex, 1, currentRowIndex, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[currentRowIndex, 1, currentRowIndex, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(15263976));

            currentRowIndex++;

            //Project description
            worksheet.Cells[currentRowIndex, 1, currentRowIndex, 6].Value = projectData.ProjectDescription;

            worksheet.Cells[currentRowIndex, 1, currentRowIndex, 6].Style.WrapText = true;
            worksheet.Cells[currentRowIndex, 1, currentRowIndex, 6].Merge = true;

            worksheet.Cells[currentRowIndex, 1, currentRowIndex, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[currentRowIndex, 1, currentRowIndex, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(15263976));

            currentRowIndex++;

            //Header
            worksheet.Cells[currentRowIndex, 1, currentRowIndex, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[currentRowIndex, 1, currentRowIndex, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(10197915));

            worksheet.Cells[currentRowIndex, 1, currentRowIndex, 2].Value = "Role";
            worksheet.Cells[currentRowIndex, 1, currentRowIndex, 2].Merge = true;

            worksheet.Cells[currentRowIndex, 3, currentRowIndex, 4].Value = "Employee Name";
            worksheet.Cells[currentRowIndex, 3, currentRowIndex, 4].Merge = true;

            worksheet.Cells[currentRowIndex, 5, currentRowIndex, 6].Value = "Employee Assignment %";
            worksheet.Cells[currentRowIndex, 5, currentRowIndex, 6].Merge = true;

            currentRowIndex++;

            foreach (AssignedRoleCollection col in projectData.EmployeesByRoles)
            {
                int startRowIndex = currentRowIndex;

                //No employees found
                if (col.Employees.Count == 0)
                {
                    int endRowIndex = currentRowIndex + col.TargetCount - 1;

                    worksheet.Cells[startRowIndex, 3, endRowIndex, 6].Value = $"No employee{(col.TargetCount > 1 ? "s" : string.Empty)} assigned";
                    worksheet.Cells[startRowIndex, 3, endRowIndex, 6].Style.WrapText = true;
                    worksheet.Cells[startRowIndex, 3, endRowIndex, 6].Merge = true;
                    worksheet.Cells[startRowIndex, 3, endRowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[startRowIndex, 3, endRowIndex, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                    worksheet.Cells[startRowIndex, 1, endRowIndex, 2].Value = $"{col.Role.Name} ({$"{col.Employees.Count}/{col.TargetCount}"})";
                    worksheet.Cells[startRowIndex, 1, endRowIndex, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[startRowIndex, 1, endRowIndex, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(col.Role.Color.ToInt()));
                    worksheet.Cells[startRowIndex, 1, endRowIndex, 2].Style.WrapText = true;
                    worksheet.Cells[startRowIndex, 1, endRowIndex, 2].Merge = true;

                    worksheet.Cells[startRowIndex, 1, endRowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[startRowIndex, 1, endRowIndex, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    currentRowIndex += col.TargetCount;
                }
                else
                {
                    //Employee name and assignment
                    foreach (AssignedEmployeeData employee in col.Employees)
                    {
                        worksheet.Cells[currentRowIndex, 3, currentRowIndex, 4].Value = employee.Employee.FullName;
                        worksheet.Cells[currentRowIndex, 3, currentRowIndex, 4].Style.WrapText = true;
                        worksheet.Cells[currentRowIndex, 3, currentRowIndex, 4].Merge = true;

                        worksheet.Cells[currentRowIndex, 5, currentRowIndex, 6].Value = employee.Employee.GetAssignentUsage(projectData, col) + "%";
                        worksheet.Cells[currentRowIndex, 5, currentRowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[currentRowIndex, 5, currentRowIndex, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[currentRowIndex, 5, currentRowIndex, 6].Merge = true;

                        //Assignment warning over 100%
                        if (employee.Employee.TotalAssignmentUsage > 100)
                        {
                            worksheet.Cells[currentRowIndex, 5, currentRowIndex, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[currentRowIndex, 5, currentRowIndex, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(16249906));
                        }

                        currentRowIndex++;
                    }

                    int endRowIndex = currentRowIndex - 1;
                    worksheet.Cells[startRowIndex, 1, endRowIndex, 2].Value = $"{col.Role.Name} ({$"{col.Employees.Count}/{col.TargetCount}"})";
                    worksheet.Cells[startRowIndex, 1, endRowIndex, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[startRowIndex, 1, endRowIndex, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(col.Role.Color.ToInt()));
                    worksheet.Cells[startRowIndex, 1, endRowIndex, 2].Style.WrapText = true;
                    worksheet.Cells[startRowIndex, 1, endRowIndex, 2].Merge = true;

                    worksheet.Cells[startRowIndex, 1, endRowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[startRowIndex, 1, endRowIndex, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
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

            worksheet.Cells[currentRowIndex, 1, currentRowIndex + totalRequiredRows, 2].Value = employee.FullName;
            worksheet.Cells[currentRowIndex, 1, currentRowIndex + totalRequiredRows, 2].Style.WrapText = true;
            worksheet.Cells[currentRowIndex, 1, currentRowIndex + totalRequiredRows, 2].Merge = true;
            worksheet.Cells[currentRowIndex, 1, currentRowIndex + totalRequiredRows, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[currentRowIndex, 1, currentRowIndex + totalRequiredRows, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            foreach (var kvp in employee._assignmentsPerctangeByProject)
            {
                if (kvp.Value.Count == 0) continue;

                int projectRequiredRows = kvp.Value.Count - 1;
                worksheet.Cells[currentRowIndex, 3, currentRowIndex + projectRequiredRows, 4].Value = kvp.Key.ProjectName;
                worksheet.Cells[currentRowIndex, 3, currentRowIndex + projectRequiredRows, 4].Style.WrapText = true;
                worksheet.Cells[currentRowIndex, 3, currentRowIndex + projectRequiredRows, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[currentRowIndex, 3, currentRowIndex + projectRequiredRows, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[currentRowIndex, 3, currentRowIndex + projectRequiredRows, 4].Merge = true;

                foreach (var roleKvp in kvp.Value)
                {
                    worksheet.Cells[currentRowIndex, 5, currentRowIndex, 6].Value = roleKvp.Key.Role.Name;
                    worksheet.Cells[currentRowIndex, 5, currentRowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[currentRowIndex, 5, currentRowIndex, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[currentRowIndex, 5, currentRowIndex, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(roleKvp.Key.Role.Color.ToInt()));
                    worksheet.Cells[currentRowIndex, 5, currentRowIndex, 6].Merge = true;

                    worksheet.Cells[currentRowIndex, 7].Value = roleKvp.Value + "%";
                    worksheet.Cells[currentRowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[currentRowIndex, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[currentRowIndex, 7].Merge = true;

                    currentRowIndex++;
                }
            }


            System.Drawing.Color textColor;
            if (employee.TotalAssignmentUsage < 100) textColor = System.Drawing.Color.FromArgb(-569311);
            else if (employee.TotalAssignmentUsage > 100) textColor = System.Drawing.Color.FromArgb(-536287);
            else textColor = System.Drawing.Color.FromArgb(-12527265);

            worksheet.Cells[currentRowIndex, 7].Value = employee.TotalAssignmentUsage + "%";
            worksheet.Cells[currentRowIndex, 7].Style.Font.Color.SetColor(textColor);
            worksheet.Cells[currentRowIndex, 7].Style.Font.Bold = true;
            worksheet.Cells[currentRowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[currentRowIndex, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[currentRowIndex, 7].Merge = true;

            currentRowIndex++;

            return currentRowIndex;
        }

        private int WriteKnownRoles(ExcelWorksheet worksheet, Employee employee, int currentRowIndex)
        {
            worksheet.Cells[currentRowIndex, 11, currentRowIndex, 12].Value = employee.FullName;
            worksheet.Cells[currentRowIndex, 11, currentRowIndex, 12].Style.WrapText = true;
            worksheet.Cells[currentRowIndex, 11, currentRowIndex, 12].Merge = true;
            worksheet.Cells[currentRowIndex, 11, currentRowIndex, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[currentRowIndex, 11, currentRowIndex, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

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
    }
}