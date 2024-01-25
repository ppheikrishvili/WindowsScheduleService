using System;
using System.Collections.Concurrent;
using System.Security.Principal;

namespace WindowsScheduleService.Model;

public static class ExecutedStore
{
    static readonly ConcurrentDictionary<string, Scheduler> Repositories = new();

    public static bool IsStored(Scheduler scheduler)
        => Repositories.TryGetValue(
            $"{scheduler.ExecutablePath}{scheduler.CommandLineParameter}", out Scheduler _);

    public static bool Store(Scheduler scheduler)
        => Repositories.GetOrAdd($"{scheduler.ExecutablePath}{scheduler.CommandLineParameter}", _ => scheduler) !=
            null;
}