using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;

namespace WindowsScheduleService;

[RunInstaller(true)]
public partial class ProjectInstaller : Installer
{
    public ProjectInstaller()
    {
        InitializeComponent();
        EventLogInstaller installer = FindInstaller(Installers);
        if (installer == null) return;
        installer.Log = "Application";
        installer.Source = "WindowsScheduleMainService";
    }

    static EventLogInstaller FindInstaller(InstallerCollection installers)
    {
        foreach (Installer installer in installers)
        {
            if (installer is EventLogInstaller logInstaller) return logInstaller;
            EventLogInstaller eventLogInstaller = FindInstaller(installer.Installers);
            if (eventLogInstaller != null) return eventLogInstaller;
        }

        return null;
    }

    public void ServiceInstaller1_AfterInstall(object sender, InstallEventArgs e)
    {
        ServiceInstaller serviceInstaller = (ServiceInstaller) sender;
        using ServiceController sc = new(serviceInstaller.ServiceName);
        sc.Start();
    }
}