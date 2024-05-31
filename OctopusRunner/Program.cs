using System.Diagnostics;

namespace OctopusRunner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                using (Process myProcess = new Process())
                {
                    myProcess.StartInfo.UseShellExecute = false;
                    myProcess.StartInfo.FileName = Path.Combine(Environment.CurrentDirectory, "lib", "ProjectOOctopus.exe");
                    myProcess.StartInfo.CreateNoWindow = true;
                    myProcess.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
