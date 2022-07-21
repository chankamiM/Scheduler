using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler
{
    internal static class InternalHelper
    {
        public static void GetTime(string strtime, Action<DateTime> passaction, Action Failaction)
        {
            if(strtime == null)
            {
                Failaction();
                return;
            }
            DateTime dt;
            try
            {
                dt = DateTime.Parse(strtime);
                passaction(dt);
            }
            catch(Exception ex)
            {
                Failaction();
            }

        }

    }
}
