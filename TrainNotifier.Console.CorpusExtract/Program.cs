using Newtonsoft.Json;
using System.IO;
using System.Linq;
using TrainNotifier.Common.Model.CorpusExtract;
using TrainNotifier.Service;

namespace TrainNotifier.Console.CorpusExtract
{
    class Program
    {
        static void Main(string[] args)
        {
            // this data is fetchable via http://nrodwiki.rockshore.net/index.php/ReferenceData
            string jsonData = File.ReadAllText("CORPUSExtract.json");

            TiplocContainer container = JsonConvert.DeserializeObject<TiplocContainer>(jsonData);

            System.Console.WriteLine("Loaded {0} Tiplocs", container.TIPLOCDATA.Count());

            TiplocRepository tiplocRepo = new TiplocRepository();

            ulong counter = 1;
            foreach (var tiploc in container.TIPLOCDATA)
            {
                System.Console.WriteLine("Processing number {0}", counter);
                if (!string.IsNullOrWhiteSpace(tiploc.STANOX) && 
                    !string.IsNullOrWhiteSpace(tiploc.TIPLOC) && 
                    tiplocRepo.GetTiplocByStanox(tiploc.STANOX) == null)
                {
                    tiplocRepo.InsertTiploc(tiploc.ToTiplocCode());
                }
                counter++;
            }
        }
    }
}
