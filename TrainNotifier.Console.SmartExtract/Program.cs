using Newtonsoft.Json;
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

            System.Console.WriteLine("Loaded {0} TD Elements", container.BERTHDATA.Count());

            System.Console.WriteLine("Enter Query");
            string data;
            while (!string.IsNullOrEmpty((data = System.Console.ReadLine())))
            {
                //var clauses = data.Split(',');
                //if (!clauses.Any())
                //    break;

                var results = container.BERTHDATA
                    .Where(t => t.TD == data)
                    .OrderBy(t => t.STANOX)
                    .ThenBy(t => string.IsNullOrEmpty(t.PLATFORM) ? -1 : int.Parse(t.PLATFORM))
                    .ThenBy(t => t.EventType);
                System.Console.WriteLine(string.Join("", Enumerable.Repeat("=", 68).ToArray()));

                System.Console.WriteLine("TD".PadRight(4) +
                    "F-B".PadRight(6) +
                    "T-B".PadRight(6) +
                    "F-L".PadRight(4) +
                    "T-L".PadRight(4) +
                    "STANOX".PadRight(8) +
                    "P".PadRight(4) +
                    "EVENT-TYPE".PadRight(12) +
                    "STEP-TYPE".ToString().PadRight(18));
                System.Console.WriteLine(string.Join("", Enumerable.Repeat("=", 68).ToArray()));

                foreach (var result in results)
                {
                    System.Console.WriteLine(result);
                }
                System.Console.WriteLine(string.Join("", Enumerable.Repeat("=", 68).ToArray()));

            }
        }
    }
}
