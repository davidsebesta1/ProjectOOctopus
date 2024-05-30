using ProjectOOctopus.Data;
using System.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Globalization;

namespace ProjectOOctopus.Services
{
    public class ExcelService
    {
        private EmployeesService _employeesService;
        private ProjectsService _projectsService;
        private RolesService _rolesService;

        public ExcelService(EmployeesService employeesService, ProjectsService projectsService, RolesService rolesService)
        {
            _employeesService = employeesService;
            _projectsService = projectsService;
            _rolesService = rolesService;
        }

        public async Task Test()
        {
            try
            {
                var path = Path.Combine("C:\\Users\\David\\Desktop", $"test.xlsx");

                using (ExcelPackage package = new ExcelPackage())
                {
                    using (ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Roles"))
                    {
                        worksheet.Cells[1, 1].Value = $"Project Organization Octopus - Export {DateTime.Now.ToString(new CultureInfo("cz-CZ"))}";
                        worksheet.Cells[1, 1, 1, 6].Merge = true;

                        int curRowIndex = 3;
                        foreach (ProjectData projectData in _projectsService._allProjects)
                        {
                            curRowIndex = WriteProjectData(worksheet, projectData, curRowIndex) + 1;
                        }

                        /*
                        for (int i = 0; i < _rolesService.Roles.Count; i++)
                        {
                            EmployeeRole role = _rolesService.Roles[i];
                            worksheet.Cells[i + 2, 1].Value = role.Name;
                            //worksheet.Cells[i + 2, 1].Style.
                            worksheet.Cells[i + 2, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[i + 2, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(role.Color.ToInt()));
                            worksheet.Cells[i + 2, 1, i + 2, 2].Merge = true;
                            worksheet.Cells[i + 2, 1, i + 2, 2].Style.WrapText = true;
                        }
                        */

                        for (int i = 1; i < 7; i++)
                        {
                            worksheet.Column(i).Width = 10d;
                        }

                        FileInfo file = new FileInfo(path);
                        package.SaveAs(file);
                    }

                }

                await Shell.Current.DisplayAlert("Export", "Export successfull!", "Return");

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Export", ex.Message, "Okay");
            }
        }

        public int WriteProjectData(ExcelWorksheet worksheet, ProjectData projectData, int startingRowIndex)
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
                if (col.Employees.Count == 0)
                {
                    int endRowIndex = currentRowIndex + col.TargetCount - 1;

                    //No employees found
                    worksheet.Cells[startRowIndex, 3, endRowIndex, 6].Value = $"No employee{(col.TargetCount > 1 ? "s" : string.Empty)} assigned";
                    worksheet.Cells[startRowIndex, 3, endRowIndex, 6].Style.WrapText = true;
                    worksheet.Cells[startRowIndex, 3, endRowIndex, 6].Merge = true;
                    worksheet.Cells[startRowIndex, 3, endRowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[startRowIndex, 3, endRowIndex, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    currentRowIndex += col.TargetCount;
                }
            }

            /*
            //Employees
            foreach (AssignedRoleCollection col in projectData.EmployeesByRoles)
            {
                int start = currentRowIndex;
                if (col.Employees.Count > 0)
                {
                    foreach (AssignedEmployeeData employee in col.Employees)
                    {
                        worksheet.Cells[currentRowIndex, 3, currentRowIndex, 4].Value = employee.Employee.FullName;
                        worksheet.Cells[currentRowIndex, 3, currentRowIndex, 4].Style.WrapText = true;
                        worksheet.Cells[currentRowIndex, 3, currentRowIndex, 4].Merge = true;

                        worksheet.Cells[currentRowIndex, 5, currentRowIndex, 6].Value = employee.Employee.GetAssignentUsage(projectData, col) + "%";
                        worksheet.Cells[currentRowIndex, 5, currentRowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[currentRowIndex, 5, currentRowIndex, 6].Merge = true;

                        //Assignment warning over 100%
                        if (employee.Employee.TotalAssignmentUsage > 100)
                        {
                            worksheet.Cells[currentRowIndex, 5, currentRowIndex, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[currentRowIndex, 5, currentRowIndex, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(16249906));
                        }

                        currentRowIndex++;
                    }
                }
                else
                {
                    //No employees found
                    worksheet.Cells[start, 3, start + col.TargetCount, 6].Value = $"No employee{(col.TargetCount > 1 ? "s" : string.Empty)} assigned";
                    worksheet.Cells[start, 3, start + col.TargetCount, 6].Style.WrapText = true;
                    worksheet.Cells[start, 3, start + col.TargetCount, 6].Merge = true;
                    worksheet.Cells[start, 3, start + col.TargetCount, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[start, 3, start + col.TargetCount, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    currentRowIndex += col.TargetCount;
                }

                //Add role and merge rows if needed
                worksheet.Cells[start, 1, start + col.TargetCount, 2].Value = $"{col.Role.Name} ({$"{col.Employees.Count}/{col.TargetCount}"})";
                worksheet.Cells[start, 1, start + col.TargetCount, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[start, 1, start + col.TargetCount, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(col.Role.Color.ToInt()));
                worksheet.Cells[start, 1, start + col.TargetCount, 2].Style.WrapText = true;
                worksheet.Cells[start, 1, start + col.TargetCount, 2].Merge = true;

                worksheet.Cells[start, 1, start + col.TargetCount, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[start, 1, start + col.TargetCount, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            }
            */

            return currentRowIndex;
        }
    }
}
