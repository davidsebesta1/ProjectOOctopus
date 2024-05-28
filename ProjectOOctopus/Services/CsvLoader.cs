using ProjectOOctopus.Data;

namespace ProjectOOctopus.Services
{
    public class CsvLoader
    {
        public async IAsyncEnumerable<EmployeeRole> LoadBaseRoles()
        {
            using (Stream stream = await FileSystem.OpenAppPackageFileAsync("BaseRoles.csv"))
            {
                using (TextReader reader = new StreamReader(stream))
                {
                    string? line;
                    await reader.ReadLineAsync();
                    while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync()))
                    {
                        string[] split = line.Split(',');
                        yield return new EmployeeRole(split[0], Color.FromHex(split[1]));
                    }
                }
            }
        }
    }
}