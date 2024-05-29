using ProjectOOctopus.Data;
using System.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;

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
            var path = Path.Combine("C:\\Users\\David\\Desktop", $"test.xlsx");

            // Create a new Excel package
            using (ExcelPackage package = new ExcelPackage())
            {
                // Add a worksheet to the workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Roles");

                // Add some data to the worksheet
                worksheet.Cells[1, 1].Value = "Name";

                for (int i = 0; i < _rolesService.Roles.Count; i++)
                {
                    EmployeeRole role = _rolesService.Roles[i];
                    worksheet.Cells[i + 2, 1].Value = role.Name;
                    //worksheet.Cells[i + 2, 1].Style.
                    worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                }

                // Save the package to the file
                FileInfo file = new FileInfo(path);
                package.SaveAs(file);
            }

            Console.WriteLine("Excel file created successfully!");
        }
    }
}
