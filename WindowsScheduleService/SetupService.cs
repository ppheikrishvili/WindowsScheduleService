using System.Diagnostics;
using System.IO;

namespace WindowsScheduleService;

public class SetupService
{
    public virtual string InstalUtilName { get; set; }
    public virtual string InstalUtilArgument { get; set; }
    public virtual string UnInstalUtilArgument { get; set; }

    protected void ExecuteSetupServiceInternal(string fileName, string arguments)
    {
        ProcessStartInfo startInfo = new()
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = false,
            FileName = fileName,
            Arguments = arguments,
            Verb = "runas",
            UseShellExecute = true
        };
        Process.Start(startInfo)?.WaitForExit();
    }

    public virtual void ExecuteSetupService()
    {
        ExecuteSetupServiceInternal(InstalUtilName, UnInstalUtilArgument);
        ExecuteSetupServiceInternal(InstalUtilName, InstalUtilArgument);
    }
}

public class InstalUtilService : SetupService
{
    public override string InstalUtilArgument { get; set; } = $"\"{Process.GetCurrentProcess().MainModule?.FileName}\"";

    public override string InstalUtilName { get; set; } =
        Path.Combine($"{System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()}", @"InstallUtil.exe");

    public override string UnInstalUtilArgument { get; set; } =
        "/u " + $"\"{Process.GetCurrentProcess().MainModule?.FileName}\"";
}