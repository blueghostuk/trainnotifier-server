using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

        private DateTime GetTrainDate(Guid trainId)
        {
            const string sql = @"
                SELECT
                    [OriginDepartTimestamp]
                FROM [LiveTrain]
                WHERE [Id] = @trainId";

            return ExecuteScalar<DateTime>(sql, new { trainId });
        }

        public void ArchiveTrainMovement(Guid trainId, IEnumerable<TrainStopArchive> trainMovements, string directoryPath)
        {
            using (var ts = GetTransactionScope())
            {
                if (trainMovements.Any())
                {
                    DateTime trainDate = GetTrainDate(trainId);
                    string stops = JsonConvert.SerializeObject(trainMovements, new JsonSerializerSettings
                    {
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        TypeNameHandling = TypeNameHandling.None,
                        NullValueHandling = NullValueHandling.Ignore,
                        TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                        Formatting = Formatting.None
                    });

                    string path = Path.Combine(directoryPath, trainDate.ToString("yyyy-MM-dd"), string.Concat(trainId.ToString(), ".json"));

                    string dir = Path.GetDirectoryName(path);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    File.WriteAllText(path, stops);

                    const string deleteSql = @"DELETE FROM [dbo].[LiveTrainStop] WHERE [TrainId] = @trainId";
                    ExecuteNonQuery(deleteSql, new { trainId });
                }

                const string updateSql = "UPDATE [dbo].[LiveTrain] SET [Archived] = 1 WHERE [Id] = @trainId";
                ExecuteNonQuery(updateSql, new { trainId });

                ts.Complete();
            }
        }

        public void UpdateIndexes()
        {
            const string updateIndexSql = @"DECLARE @TableName varchar(255)
 
                DECLARE TableCursor CURSOR FOR
                (
                      SELECT '[' + IST.TABLE_SCHEMA + '].[' + IST.TABLE_NAME + ']' AS [TableName]
                      FROM INFORMATION_SCHEMA.TABLES IST
                      WHERE IST.TABLE_TYPE = 'BASE TABLE'
                )
 
                OPEN TableCursor
                FETCH NEXT FROM TableCursor INTO @TableName
                WHILE @@FETCH_STATUS = 0
 
                BEGIN
                      PRINT('Rebuilding Indexes on ' + @TableName)
                      EXEC('ALTER INDEX ALL ON ' + @TableName + ' REBUILD')
                      FETCH NEXT FROM TableCursor INTO @TableName
                END
 
                CLOSE TableCursor
                DEALLOCATE TableCursor";

            ExecuteNonQuery(updateIndexSql);
        }
    }
}
