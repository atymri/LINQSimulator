using LinqSimulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQSimulator
{
    public static class GroupByAggregator
    {
        public static IEnumerable<object> Aggregate(IEnumerable<IGrouping<object, object>> groups, string aggregateFunction)
        {
            return aggregateFunction.ToLower() switch
            {
                "count" => groups.Select(g => new { Key = g.Key, Count = g.Count() }),
                "sum" => groups.Select(g => new { Key = g.Key, Sum = NumericHelper.Sum(g) }),
                "avg" or "average" => groups.Select(g => new { Key = g.Key, Average = NumericHelper.Average(g) }),
                "min" => groups.Select(g => new { Key = g.Key, Min = NumericHelper.Min(g) }),
                "max" => groups.Select(g => new { Key = g.Key, Max = NumericHelper.Max(g) }),
                "items" or _ => groups.Select(g => new { Key = g.Key, Items = g.ToList() })
            };
        }
    }
}
