using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using WindowsScheduleService.Extension;
using WindowsScheduleService.Model;

namespace WindowsScheduleService;

partial class WindowsScheduleMainService : ServiceBase
{
    public Timer RunWindowsScheduleTimer;
    readonly AutoResetEvent _mainWaiter = new(true);

    const string CronFileName = @"C:\Test\Test.txt";

    public WindowsScheduleMainService()
    {
        InitializeComponent();
        AutoLog = true;
    }


    protected override void OnStart(string[] args)
    {
        EventLog.WriteEntry("The WindowsScheduleMainService was started successfully.", EventLogEntryType.Information);
        RunWindowsScheduleTimer = new Timer(RunSpecified, _mainWaiter, 59000, 59000);

        void RunSpecified(object stateInfo)
        {
            if (stateInfo is not AutoResetEvent waiter) return;
            try
            {
                waiter.WaitOne();
                List<Scheduler> cronTabs = new CronTabGuru().ReadCronTab(CronFileName);
                Process[] processes = Process.GetProcesses();
                foreach (Scheduler cronTabGuru in cronTabs.Where(w => w.IsReady()))
                {
                    if (processes.FirstOrDefault(p => p.ProcessName.Equals(
                            Path.GetFileNameWithoutExtension(cronTabGuru.ExecutablePath),
                            StringComparison.InvariantCultureIgnoreCase)) != default(Process) ||
                        ExecutedStore.IsStored(cronTabGuru))
                    {
                        EventLog.WriteEntry(
                            $"Begin to execute as scheduled: {cronTabGuru.ExecutablePath} at {DateTime.Now} with parameters: " +
                            $"{cronTabGuru.CommandLineParameter}, but it has already run",
                            EventLogEntryType.Information);
                        continue;
                    }

                    RunProcess(cronTabGuru.ExecutablePath, cronTabGuru.CommandLineParameter);
                    EventLog.WriteEntry(
                        $"Executed as scheduled: {cronTabGuru.ExecutablePath} at {DateTime.Now} with parameters: " +
                        $"{cronTabGuru.CommandLineParameter}, solution running", EventLogEntryType.Information);
                    ExecutedStore.Store(cronTabGuru);
                }

                waiter.Set();

                void RunProcess(string path, string argument)
                {
                    if (!File.Exists($"{path}")) return;
                    using Process process = new();
                    process.StartInfo = new ProcessStartInfo(path)
                    {
                        UseShellExecute = true,
                        Arguments = argument
                    };
                    process.Start();
                }
            }
            catch (Exception e)
            {
                EventLog.WriteEntry($"Service internal error: {e.ToErrorStr()}", EventLogEntryType.Error);
                waiter.Set();
                throw;
            }
        }
    }

    protected override void OnStop() =>
        EventLog.WriteEntry("WindowsScheduleMainService service ended", EventLogEntryType.Information);

    protected override void OnShutdown()
    {
        EventLog.WriteEntry("Windows Server is SHUTDOWN, Message sender service is stop - " + DateTime.Now,
            EventLogEntryType.Information);
        base.OnShutdown();
    }
}