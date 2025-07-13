using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace bakinplayer
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            if (Environment.GetEnvironmentVariable("CONTINUE") == "true")
            {
                AppDomain.CurrentDomain.ExecuteAssembly("data\\bakinplayer.exe.bak");
                return;
            }
            const string dicName = "dic.txt";
            const string errorLogName = "error.log";
            if (File.Exists(errorLogName))
                File.Delete(errorLogName);
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                var args = Environment.GetCommandLineArgs().ToList();
                args.AddRange(new[] { "/Dic", "/L=EN" });
                var tempDir = args.FirstOrDefault(it => it.StartsWith("/TMP="))?.Split('|', '=')[3];
                File.Copy(Path.Combine("data", dicName), Path.Combine(tempDir, dicName));
                var processStartInfo = new ProcessStartInfo(args[0], string.Join(" ", args.Skip(1)));
                processStartInfo.EnvironmentVariables.Add("CONTINUE", "true");
                processStartInfo.UseShellExecute = false;
                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                File.WriteAllText("error.log", ex.ToString());
            }

        }
    }
}
