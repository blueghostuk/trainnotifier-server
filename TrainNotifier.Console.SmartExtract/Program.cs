using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using TrainNotifier.Common.Model.SmartExtract;

namespace TrainNotifier.Console.SmartExtract
{
    class Program
    {
        static void Main(string[] args)
        {
            // this data is fetchable via http://nrodwiki.rockshore.net/index.php/ReferenceData
            string jsonData = File.ReadAllText("SMARTExtract.json");

            TDContainer container = JsonConvert.DeserializeObject<TDContainer>(jsonData);

            if (args.Length > 0 && args[0].Equals("extract", System.StringComparison.InvariantCultureIgnoreCase))
            {
                System.Console.WriteLine("Writing to SMARTExtract.txt");
                File.WriteAllText("SMARTExtract.txt",
                    string.Join(Environment.NewLine, container.BERTHDATA.Select(b => b.ToString())));
                return;
            }

            System.Console.WriteLine("Loaded {0} TD Elements", container.BERTHDATA.Count());

            System.Console.WriteLine("Enter Query");
            string data;
            while (!string.IsNullOrEmpty((data = System.Console.ReadLine())))
            {
                var clauses = data.Split(',');
                if (!clauses.Any())
                    break;

                var results = container.BERTHDATA
                    .Where(t => t.TD.Equals(clauses[0], System.StringComparison.InvariantCultureIgnoreCase));

                if (clauses.Length > 1)
                {
                    results = results.Where(t => t.FROMBERTH.Equals(clauses[1], System.StringComparison.InvariantCultureIgnoreCase));
                }

                results = results
                    .OrderBy(t => t.STANOX)
                    .ThenBy(t => string.IsNullOrEmpty(t.PLATFORM) ? -1 : int.Parse(t.PLATFORM))
                    .ThenBy(t => t.EventType);

                System.Console.WriteLine(string.Join("", Enumerable.Repeat("=", 72).ToArray()));

                System.Console.WriteLine("TD".PadRight(4) +
                    "F-B".PadRight(6) +
                    "T-B".PadRight(6) +
                    "F-L".PadRight(4) +
                    "T-L".PadRight(4) +
                    "STANME".PadRight(10) +
                    "P".PadRight(4) +
                    "EVENT-TYPE".PadRight(12) +
                    "STEP-TYPE".ToString().PadRight(10) + 
                    "OFFSET".ToString().PadRight(6));
                System.Console.WriteLine(string.Join("", Enumerable.Repeat("=", 72).ToArray()));

                foreach (var result in results)
                {
                    System.Console.WriteLine(result);
                }
                System.Console.WriteLine(string.Join("", Enumerable.Repeat("=", 72).ToArray()));

            }
        }
    }
}
