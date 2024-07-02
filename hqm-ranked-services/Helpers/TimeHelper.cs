using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_services.Helpers
{
    public static class TimeHelper
    {
        public static string GetRemainingTime(DateTime date)
        {
            var timeDifference = date -DateTime.UtcNow;

            if (timeDifference.TotalSeconds < 60)
            {
                return $"in {(int)timeDifference.TotalSeconds} seconds";
            }
            else if (timeDifference.TotalMinutes < 60)
            {
                return $"in {(int)timeDifference.TotalMinutes} minutes";
            }
            else if (timeDifference.TotalHours < 24)
            {
                return $"in {(int)timeDifference.TotalHours} hours";
            }
            else if (timeDifference.TotalDays < 30)
            {
                return $"in {(int)timeDifference.TotalDays} days";
            }
            else
            {
                int months = (int)(timeDifference.TotalDays / 30);
                return $"in {months} months";
            }
        }
    }
}
