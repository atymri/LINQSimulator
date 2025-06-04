using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQSimulator
{
    public static class ValidationHelper
    {
        public static bool TryParsePositiveInt(string input, out int result)
        {
            result = 0;
            return int.TryParse(input?.Trim(), out result) && result > 0;
        }

        public static bool TryParseNonNegativeInt(string input, out int result)
        {
            result = 0;
            return int.TryParse(input?.Trim(), out result) && result >= 0;
        }
    }
}
