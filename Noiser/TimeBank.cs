using System;
using System.Collections.Generic;
using System.Linq;

namespace Noiser
{
    public class TimeBank
    {
        private static readonly int DefaultHourBank = 10 * 60;
        private static readonly int DefaultCoefficientHelp = 5 * 60;
        private List<(DateTime Timestamp, int Length)> _timeBank;

        public TimeBank()
        {
            _timeBank = new List<(DateTime, int)>();
        }

        public bool IsWithinOkHours()
        {
            var currTime = DateTime.Now;
            return currTime.Hour >= 8 && currTime.Hour < 22;
        }

        public bool IsTimeAvailable()
        {
            if (!IsWithinOkHours()) return false;
            CleanUp();
            var totalTime = _timeBank.Sum(x => x.Length);
            return totalTime < DefaultHourBank;
        }

        public bool AddTime(DateTime timestamp, int length)
        {
            CleanUp();
            if (IsTimeAvailable())
            {
                _timeBank.Add((timestamp, length));
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CleanUp() => _timeBank = _timeBank.Where(x => x.Timestamp > DateTime.Now.AddHours(-1)).ToList();

        public int GetTimeAvailable()
        {
            if (!IsWithinOkHours()) return 0;
            var result = DefaultHourBank - _timeBank.Sum(x => x.Length);
            return result >= 0 ? result : 0;
        }

        public int GetSpentTime() => DefaultHourBank - GetTimeAvailable();

        public int GetTimeAvailablePercentage() => GetTimeAvailable() * 100 / DefaultHourBank;

        public int GenerateWaitTime(Random rnd)
        {
            var coefficient = DefaultCoefficientHelp / (double) DefaultHourBank;
            var spendTimeAdj = GetSpentTime() * coefficient;
            var waitTime = rnd.Next((int)(250 * spendTimeAdj), (int)(1000 * 120 + 1000 * spendTimeAdj));
            return waitTime;
        }
    }
}
