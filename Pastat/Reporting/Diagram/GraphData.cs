using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pastat.Reporting.Diagram
{
    public class GraphData: IEnumerable<Series>
    {
        public List<XValue> XValues { get; set; }

        public Dictionary<string, List<long>> Series { get; set; }

        public long MinY => Series.Values.SelectMany(x => x).Min();
        public long MaxY => Series.Values.SelectMany(x => x).Max();

        public GraphData()
        {
            XValues = new List<XValue>();
            Series = new Dictionary<string, List<long>>();
        }

        public void Append(Dictionary<string, long> values, XValue x)
        {
            XValues.Add(x);

            foreach (var kvp in values)
            {
                if (Series.TryGetValue(kvp.Key, out List<long> series))
                    series.Add(kvp.Value);
                else
                    Series[kvp.Key] = new List<long> { kvp.Value };
            }
        }

        public IEnumerator<Series> GetEnumerator()
        {
            return Series.Select(kvp => new Series { Name = kvp.Key, Values = kvp.Value.ToArray() })
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
