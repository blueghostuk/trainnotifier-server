using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.CorpusExtract;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.Common.Model.SmartExtract;
using TrainNotifier.Service;

namespace TrainNotifier.Console.SmartExtract
{
    class Program
    {
        static void Main(string[] args)
        {
            // this data is fetchable via http://nrodwiki.rockshore.net/index.php/ReferenceData
            List<TDElement> allSmartData = new List<TDElement>();

            foreach (string smartExtract in Directory.GetFiles(".", "SMARTExtract*.json"))
            {
                Trace.TraceInformation("Loading SMART data from {0}", smartExtract);
                string smartData = File.ReadAllText(smartExtract);
                allSmartData.AddRange(JsonConvert.DeserializeObject<TDContainer>(smartData).BERTHDATA);
            }

            TDContainer container = new TDContainer
            {
                BERTHDATA = allSmartData
            };
            ILookup<string, TDElement> tdElementsByArea = allSmartData
                .Where(td => !string.IsNullOrEmpty(td.STANOX))
                .ToLookup(td => td.TD);

            if (args.Length > 0 && args[0].Equals("extract", System.StringComparison.InvariantCultureIgnoreCase))
            {
                System.Console.WriteLine("Writing to SMARTExtract.txt");
                File.WriteAllText("SMARTExtract.txt",
                    string.Join(Environment.NewLine, container.BERTHDATA.Select(b => b.ToString())));
                return;
            }

            TiplocRepository _tiplocRepo = new TiplocRepository();

            var dbTiplocs = _tiplocRepo.Get();

            string tiplocData = File.ReadAllText("CORPUSExtract.json");
            ILookup<string, TiplocCode> tiplocByStanox = JsonConvert.DeserializeObject<TiplocContainer>(tiplocData).TIPLOCDATA
                .Select(t => t.ToTiplocCode())
                .ToLookup(t => t.Stanox);

            System.Console.WriteLine("Loaded {0} TD Elements", container.BERTHDATA.Count());

            System.Console.WriteLine("Enter Query");
            string data;
            while (!string.IsNullOrEmpty((data = System.Console.ReadLine())))
            {
                if (data.StartsWith("query=", StringComparison.InvariantCultureIgnoreCase))
                {
                    GenerateQuery(data, tdElementsByArea, tiplocByStanox);
                    continue;
                }
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
                    .OrderBy(t => t.STANME)
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

        private static void GenerateQuery(string data, ILookup<string, TDElement> tdElementsByArea, ILookup<string, TiplocCode> tiplocByStanox)
        {
            data = data.Replace("query=", string.Empty);
            // TODO: parse query

            UpdateTrainMovements(new TrainDescriber[] 
            {
                new CcTD
                {
                    AreaId = "WY",
                    Description = "1H14",
                    Time = DateTime.UtcNow,
                    To = "B014",
                    Type = "From"
                }
            }, tdElementsByArea, tiplocByStanox);
        }

        private static void UpdateTrainMovements(IEnumerable<TrainDescriber> trainData, ILookup<string, TDElement> tdElementsByArea, ILookup<string, TiplocCode> tiplocByStanox)
        {
            var data = trainData
                .Where(tdesc => tdElementsByArea.Contains(tdesc.AreaId))
                .Select(tdesc => Tuple.Create(tdesc, tdElementsByArea[tdesc.AreaId].FirstOrDefault(td => td.Equals(tdesc))))
                .Where(t => t.Item2 != null)
                .Select(t => Tuple.Create(t.Item1, t.Item2, tiplocByStanox[t.Item2.STANOX].FirstOrDefault()))
                .Where(t => t.Item3 != null)
                .ToList();

            foreach (var td in data)
            {
                ProcessTdResult(td);
            }
        }

        private static void ProcessTdResult(Tuple<TrainDescriber, TDElement, TiplocCode> td)
        {
            //throw new NotImplementedException();
        }
    }
}
