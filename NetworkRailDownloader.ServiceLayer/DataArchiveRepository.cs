using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using TrainNotifier.Common.Model.Archive;

namespace TrainNotifier.Service
{
    public class ArchiveTrain
    {
        public Guid Id { get; set; }
        public Guid? ScheduleTrain { get; set; }
        public DateTime OriginDepartTimestamp { get; set; }
    }

    public class DataArchiveRepository : DbRepository
    {
        public DataArchiveRepository()
            : base(defaultCommandTimeout: 0)
        { }

        public IEnumerable<ArchiveTrain> GetTrainsToArchive(DateTime olderThan, uint limit = 10)
        {
            const string sql = @"
                SELECT TOP {0} [Id], [OriginDepartTimestamp], [ScheduleTrain]
                FROM [dbo].[LiveTrain]
                WHERE [OriginDepartTimestamp] <= @olderThan";

            return Query<ArchiveTrain>(string.Format(sql, limit), new { olderThan });
        }

        public IEnumerable<TrainStopArchive> GetTrainMovements(Guid trainId)
        {
            const string sql = @"
                SELECT [EventTypeId]
                      ,[PlannedTimestamp]
                      ,[ActualTimestamp]
                      ,[ReportingTiplocId]
                      ,[Platform]
                      ,[Line]
                      ,[ScheduleStopNumber]
                FROM [dbo].[LiveTrainStop]
                WHERE [TrainId] = @trainId";

            return Query<TrainStopArchive>(sql, new { trainId });
        }

        public void ArchiveTrainMovement(ArchiveTrain train, IEnumerable<TrainStopArchive> trainMovements, string directoryPath)
        {
            using (var ts = GetTransactionScope())
            {
                const string deleteCancellationsSql = "DELETE FROM [dbo].[LiveTrainCancellation] WHERE [TrainId] = @trainId";
                int cancel = ExecuteNonQuery(deleteCancellationsSql, new { trainId = train.Id });
                const string deleteCoOSql = "DELETE FROM [dbo].[LiveTrainChangeOfOrigin] WHERE [TrainId] = @trainId";
                int coo = ExecuteNonQuery(deleteCoOSql, new { trainId = train.Id });
                const string deleteReinstateSql = "DELETE FROM [dbo].[LiveTrainReinstatement] WHERE [TrainId] = @trainId";
                int reinstate = ExecuteNonQuery(deleteReinstateSql, new { trainId = train.Id });

                if (trainMovements.Any())
                {
                    string stops = JsonConvert.SerializeObject(trainMovements, new JsonSerializerSettings
                    {
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        TypeNameHandling = TypeNameHandling.None,
                        NullValueHandling = NullValueHandling.Ignore,
                        TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                        Formatting = Formatting.None
                    });

                    string path = Path.Combine(directoryPath, train.OriginDepartTimestamp.ToString("yyyy-MM-dd"), string.Concat(
                        cancel > 0 ? "CX_" : string.Empty,
                        coo > 0 ? "CO_" : string.Empty,
                        reinstate > 0 ? "RN_" : string.Empty,
                        "SD_",
                        train.ScheduleTrain.HasValue && train.ScheduleTrain != Guid.Empty ? train.ScheduleTrain.ToString() : "NS",  "_ID_" + train.Id.ToString(), ".json"));

                    string dir = Path.GetDirectoryName(path);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    File.WriteAllText(path, stops);

                    const string deleteSql = @"DELETE FROM [dbo].[LiveTrainStop] WHERE [TrainId] = @trainId";
                    ExecuteNonQuery(deleteSql, new { trainId = train.Id });
                }

                const string deleteTrainSql = "DELETE FROM [dbo].[LiveTrain] WHERE [Id] = @trainId";
                ExecuteNonQuery(deleteTrainSql, new { trainId = train.Id });

                ts.Complete();
            }
        }

        private static readonly int DefaultLongQueryTimeout = (int)TimeSpan.FromMinutes(30).TotalSeconds;

        private static readonly string[] _indexTables = new[] 
            {
                "ScheduleTrainStop",
                "LiveTrainStop",
                "LiveTrain",
                "ScheduleTrain",
                "PPMRecord"
            };
        public void UpdateIndexes()
        {
            const string updateIndexSqlFormat = "ALTER INDEX ALL ON [{0}] REBUILD";

            foreach (string table in _indexTables)
            {
                try
                {
                    ExecuteNonQuery(string.Format(updateIndexSqlFormat, table), commandTimeout: DefaultLongQueryTimeout);
                }
                catch (Exception e)
                {
                    Trace.TraceError("Error updating index for table {0}", table);
                }
            }
        }

        public void CleanSchedules(DateTime olderThan)
        {
            using (var ts = GetTransactionScope())
            {
                const string deleteStopsSql = @"
                    DELETE FROM [ScheduleTrainStop] 
                    WHERE [ScheduleId] IN (
                        SELECT DISTINCT [ScheduleId]
                        FROM [ScheduleTrain]
                        LEFT JOIN [LiveTrain] on [ScheduleTrain].[ScheduleId] = [LiveTrain].[ScheduleTrain]
                        WHERE [LiveTrain].[Id] IS NULL AND [ScheduleTrain].[EndDate] <= @olderThan)";

                ExecuteNonQuery(deleteStopsSql, new { olderThan }, commandTimeout: DefaultLongQueryTimeout);

                const string deleteSchedulesSql = @"
                    DELETE [ScheduleTrain] FROM [ScheduleTrain]
                    LEFT JOIN [LiveTrain] on [ScheduleTrain].[ScheduleId] = [LiveTrain].[ScheduleTrain]
                    WHERE [LiveTrain].[Id] IS NULL AND [ScheduleTrain].[EndDate] <= @olderThan";

                ExecuteNonQuery(deleteSchedulesSql, new { olderThan }, commandTimeout: DefaultLongQueryTimeout);

                ts.Complete();
            }
        }

        public void CleanAssociations(DateTime olderThan)
        {
            using (var ts = GetTransactionScope())
            {
                const string deleteAssocSql = @"
                    DELETE FROM [TrainAssociation] 
                    WHERE [EndDate] <= @olderThan OR [Deleted] = 1";

                ExecuteNonQuery(deleteAssocSql, new { olderThan }, commandTimeout: DefaultLongQueryTimeout);

                ts.Complete();
            }
        }

        public void CleanPPMRecords(DateTime olderThan)
        {
            using (var ts = GetTransactionScope())
            {
                const string deletePPMSql = @"
                    DELETE FROM [PPMRecord] 
                    WHERE [Timestamp] <= @olderThan";

                ExecuteNonQuery(deletePPMSql, new { olderThan }, commandTimeout: DefaultLongQueryTimeout);

                ts.Complete();
            }
        }
    }
}
