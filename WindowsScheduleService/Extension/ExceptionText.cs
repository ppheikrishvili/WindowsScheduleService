using System;

namespace WindowsScheduleService.Extension;

public static class ExceptionText
{
    public static string ToErrorStr(this Exception ex)
    {
        string errorStr = "";
        Exception tmpEx = ex;
        while (tmpEx != null)
        {
            errorStr += $"{tmpEx.Message}{Environment.NewLine}";
            tmpEx = tmpEx.InnerException;
        }

        return errorStr;
    }
}