using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQSimulator
{
    public static class NumericHelper
    {
        public static double Sum(IEnumerable<object> items)
        {
            return items.Where(TryConvertToDouble).Select(Convert.ToDouble).Sum();
        }

        public static double Average(IEnumerable<object> items)
        {
            var numbers = items.Where(TryConvertToDouble).Select(Convert.ToDouble).ToList();
            return numbers.Count > 0 ? numbers.Average() : 0;
        }

        public static object Min(IEnumerable<object> items)
        {
            var numbers = items.Where(TryConvertToDouble).ToList();
            if (numbers.Count > 0)
                return numbers.Select(Convert.ToDouble).Min();

            var nonNumbers = items.Where(x => !TryConvertToDouble(x)).ToList();
            return nonNumbers.Count > 0 ? nonNumbers.OrderBy(x => x?.ToString()).First() : null;
        }

        public static object Max(IEnumerable<object> items)
        {
            var numbers = items.Where(TryConvertToDouble).ToList();
            if (numbers.Count > 0)
                return numbers.Select(Convert.ToDouble).Max();

            var nonNumbers = items.Where(x => !TryConvertToDouble(x)).ToList();
            return nonNumbers.Count > 0 ? nonNumbers.OrderByDescending(x => x?.ToString()).First() : null;
        }

        private static bool TryConvertToDouble(object obj)
        {
            return obj switch
            {
                null => false,
                int or long or short or byte => true,
                float or double or decimal => true,
                _ => double.TryParse(obj.ToString(), out _)
            };
        }
    }
}
