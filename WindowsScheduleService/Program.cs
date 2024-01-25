using System.Linq;
using System.ServiceProcess;

namespace WindowsScheduleService;

static class Program
{
    static void Main(string[] args)
    {
        const string startInstal = "Instal";
        if (!args.Contains(startInstal))
        {
            ServiceBase[] servicesToRun =
            {
                new WindowsScheduleMainService()
            };
            ServiceBase.Run(servicesToRun);
        }
        else
        {
            new InstalUtilService().ExecuteSetupService();
        }
    }
}