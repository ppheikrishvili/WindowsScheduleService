using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace WindowsScheduleService.Model;

class CronTabGuru
{
    public List<Scheduler> ReadCronTab(string filename)
    {
        List<Scheduler> retCronTabs = new();
        StreamReader sr = new(filename);
        while (sr.ReadLine() is { } line)
        {
            ArrayList mDays, wDays;
            line = line.Trim();
            //line = Regex.Replace(line.Trim(), @"\s+", " ");
            if (line.Length == 0 || line.StartsWith("#")) continue;
            string[] cols = line.Replace("\\\\", "<BACKSLASH>").Replace("\\ ", "<SPACE>").Split(' ', '\t');
            for (int i = 0; i < cols.Length; i++)
            {
                cols[i] = cols[i].Replace("<BACKSLASH>", "\\").Replace("<SPACE>", " ");
            }

            ArrayList minutes = ParseTimes(cols[0], 0, 59);
            ArrayList hours = ParseTimes(cols[1], 0, 23);
            ArrayList months = ParseTimes(cols[3], 1, 12);
            if (!cols[2].Equals("*") && cols[3].Equals("*"))
            {
                mDays = ParseTimes(cols[2], 1, 31);
                wDays = new ArrayList {-1};
            }
            else if (cols[2].Equals("*") && !cols[3].Equals("*"))
            {
                mDays = new ArrayList {-1};
                wDays = ParseTimes(cols[4], 1, 7); // 60 * 24 * 7
            }
            else
            {
                mDays = ParseTimes(cols[2], 1, 31);
                wDays = ParseTimes(cols[4], 1, 7); // 60 * 24 * 7
            }

            string args = string.Join(" ", cols.Length > 6 ? cols[6] : "");
            retCronTabs.Add(new Scheduler(months, mDays, wDays, hours, minutes,
                cols[5], args));
        }

        sr.Close();
        return retCronTabs;


        ArrayList ParseTimes(string line, int startNr, int maxNr)
        {
            ArrayList ovals = new();
            string[] list = line.Split(',');
            foreach (string entry in list)
            {
                int start = -1, end = -1, interval = 1;
                string[] parts = entry.Split('-', '/');
                if (parts[0].Equals("*"))
                {
                    if (parts.Length > 1) (start, end, interval) = (startNr, maxNr, int.Parse(parts[1]));
                }
                else
                {
                    (start, end, interval) = (int.Parse(parts[0]),
                        parts.Length > 1 ? int.Parse(parts[1]) : int.Parse(parts[0]),
                        parts.Length > 2 ? int.Parse(parts[2]) : 1);
                }

                for (int i = start; i <= end; i += interval) ovals.Add(i);
            }

            return ovals;
        }
    }
}