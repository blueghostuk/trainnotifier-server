using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using TrainNotifier.Common.Archive;

namespace TrainNotifier.Service
{
    public class DataArchiveRepository : DbRepository
    {
        public DataArchiveRepository()
            : base(defaultCommandTimeout: 0)
        { }

        public IEnumerable<Guid> GetTrainsToArchive(DateTime olderThan, uint limit = 10)
        {
            const string sql = @"
                SELECT TOP {0} [Id]
                FROM [dbo].[LiveTrain]
                WHERE [OriginDepartTimestamp] <= @olderThan
                    AND [Archived] = 0";

            return Query<Guid>(string.Format(sql, limit), new { olderThan });
        }

        public IEnumerable<TrainStopArchive> GetTrainMovements(Guid trainId)
        {
            const string sql = @"
                SELECT [EventType]
                      ,[PlannedTimestamp]
                      ,[ActualTimestamp]
                      ,[ReportingStanox]
                      ,[Platform]
                      ,[Line]
                      ,[TrainTerminated]
                FROM [dbo].[LiveTrainStop]
                WHERE [TrainId] = @trainId";

            return Query<TrainStopArchive>(sql, new { trainId });
        }

        public void ArchiveTrainMovement(Guid trainId, IEnumerable<TrainStopArchive> trainMovements)
        {
            using (var ts = GetTransactionScope())
            {
                if (trainMovements.Any())
                {
                    const string insertSql = @"
                        INSERT INTO [natrail].[dbo].[LiveTrainStopArchive]
                           ([LiveTrainId]
                           ,[Stops])
                        VALUES
                           (@trainId
                           ,@stops)";

                    string stops = JsonConvert.SerializeObject(trainMovements, new JsonSerializerSettings
                    {
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        TypeNameHandling = TypeNameHandling.None,
                        NullValueHandling = NullValueHandling.Ignore,
                        TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                        Formatting = Formatting.None
                    });

                    ExecuteNonQuery(insertSql, new { trainId, stops});

                    const string deleteSql = @"DELETE FROM [dbo].[LiveTrainStop] WHERE [TrainId] = @trainId";
                    ExecuteNonQuery(deleteSql, new { trainId });
                }

                const string updateSql = "UPDATE [dbo].[LiveTrain] SET [Archived] = 1 WHERE [Id] = @trainId";
                ExecuteNonQuery(updateSql, new { trainId });

                ts.Complete();
            }
        }
    }
}
