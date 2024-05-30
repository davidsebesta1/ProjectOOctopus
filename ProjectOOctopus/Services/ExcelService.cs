using ProjectOOctopus.Data;
using System.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Globalization;

namespace ProjectOOctopus.Services
{
    public class ExcelService
    {
        public const double CellWidth = 10d;
        private EmployeesService _employeesService;
        private ProjectsService _projectsService;
        private RolesService _rolesService;

        public ExcelService(EmployeesService employeesService, ProjectsService projectsService, RolesService rolesService)
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
                    using ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Projects");

                    WriteExportHeader(worksheet);

                    int curRowIndexProjects = 3;
                    foreach (ProjectData projectData in _projectsService._allProjects)
                    {
                        curRowIndexProjects = WriteProjectData(worksheet, projectData, curRowIndexProjects) + 1;
                    }

                    for (int i = 1; i < 7; i++)
                    {
                        worksheet.Column(i).Width = CellWidth;
                    }

                    #endregion

                    #region Employees

                    using ExcelWorksheet worksheetEmployees = package.Workbook.Worksheets.Add("Employees");

                    WriteExportHeader(worksheetEmployees);

                    int curRowIndexEmployees = 3;
                    //Header
                    worksheetEmployees.Cells[curRowIndexEmployees, 1, curRowIndexEmployees, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetEmployees.Cells[curRowIndexEmployees, 1, curRowIndexEmployees, 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(10197915));

                    worksheetEmployees.Cells[curRowIndexEmployees, 1, curRowIndexEmployees, 2].Value = "Employee Name";
                    worksheetEmployees.Cells[curRowIndexEmployees, 1, curRowIndexEmployees, 2].Merge = true;

                    worksheetEmployees.Cells[curRowIndexEmployees, 3, curRowIndexEmployees, 4].Value = "Project Name";
                    worksheetEmployees.Cells[curRowIndexEmployees, 3, curRowIndexEmployees, 4].Merge = true;

                    worksheetEmployees.Cells[curRowIndexEmployees, 5, curRowIndexEmployees, 6].Value = "Role";
                    worksheetEmployees.Cells[curRowIndexEmployees, 5, curRowIndexEmployees, 6].Merge = true;

                    worksheetEmployees.Cells[curRowIndexEmployees, 7].Value = "Assignment";

                    curRowIndexEmployees++;
                    foreach (Employee employee in _employeesService._allEmployees)
                    {
                        curRowIndexEmployees = WriteEmployesData(worksheetEmployees, employee, curRowIndexEmployees) + 1;
                    }

                    for (int i = 1; i < 8; i++)
                    {
                        worksheetEmployees.Column(i).Width = CellWidth;
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

            worksheet.Cells[currentRowIndex, 7].Value = employee.TotalAssignmentUsage + "%";
            worksheet.Cells[currentRowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            System.Drawing.Color textColor;
            if (employee.TotalAssignmentUsage < 100) textColor = System.Drawing.Color.FromArgb(-569311);
            else if (employee.TotalAssignmentUsage > 100) textColor = System.Drawing.Color.FromArgb(-536287);
            else textColor = System.Drawing.Color.FromArgb(-12527265);

            worksheet.Cells[currentRowIndex, 7].Style.Font.Color.SetColor(textColor);
            worksheet.Cells[currentRowIndex, 7].Style.Font.Bold = true;
            worksheet.Cells[currentRowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[currentRowIndex, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[currentRowIndex, 7].Merge = true;

            currentRowIndex++;

            return currentRowIndex;
        }
    }
}