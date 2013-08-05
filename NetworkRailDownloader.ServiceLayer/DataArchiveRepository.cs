﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using TrainNotifier.Common.Archive;

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
            ExecuteNonQuery(updateIndexSql, commandTimeout: (int)TimeSpan.FromMinutes(10).TotalSeconds);
        }
    }
}
