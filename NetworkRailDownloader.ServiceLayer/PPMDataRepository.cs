using System;
using System.Collections.Generic;
using TrainNotifier.Common.Model.PPM;

namespace TrainNotifier.Service
{
    public class PPMDataRepository : DbRepository
    {
        public IEnumerable<PPMSector> GetSectors(byte? operatorCode = null)
        {
            const string sql = @"
                SELECT [PPMSectorId]
                    ,[OperatorCode]
                    ,[SectorCode]
                    ,[Description]
                FROM [PPMSectors]";

            return Query<PPMSector>(string.Concat(sql, operatorCode.HasValue ? " WHERE [OperatorCode] = @operatorCode AND [SectorCode] IS NOT NULL" : null), new { operatorCode });
        }

        public IEnumerable<PPMRecord> GetLatestRecords(byte? operatorCode, string name, DateTime startDate, DateTime endDate)
        {
            const string sql = @"
                SELECT
                   [PPMRecord].[Timestamp]
                  ,[PPMRecord].[Total]
                  ,[PPMRecord].[OnTime]
                  ,[PPMRecord].[Late]
                  ,[PPMRecord].[CancelVeryLate]
                  ,[PPMRecord].[Trend]
                  ,[PPMSectors].[OperatorCode] AS [Code]
                  ,[PPMSectors].[Description] AS [Name]
                FROM [PPMRecord]
                INNER JOIN [PPMSectors] ON [PPMRecord].[PPMSectorId] = [PPMSectors].[PPMSectorId]
                WHERE {0}
                ORDER BY [PPMRecord].[Timestamp] DESC";

            ICollection<string> whereClauses = new List<string>(2);
            if (operatorCode.HasValue)
                whereClauses.Add("[PPMSectors].[OperatorCode] = @operatorCode");
            else
                whereClauses.Add("[PPMSectors].[OperatorCode] IS NULL");
            if (name != null)
                whereClauses.Add("[PPMSectors].[Description] = @name");
            else
                whereClauses.Add("[PPMSectors].[SectorCode] IS NULL");

            whereClauses.Add("[PPMRecord].[Timestamp] >= @startDate");
            whereClauses.Add("[PPMRecord].[Timestamp] < @endDate");

            return Query<PPMRecord>(string.Format(sql, string.Join(" AND ", whereClauses)), new
            {
                operatorCode,
                name,
                startDate,
                endDate
            });
        }

        public PPMRecord GetLatestRecord(byte? operatorCode, string name)
        {
            const string sql = @"
                SELECT TOP 1
                   [PPMRecord].[Timestamp]
                  ,[PPMRecord].[Total]
                  ,[PPMRecord].[OnTime]
                  ,[PPMRecord].[Late]
                  ,[PPMRecord].[CancelVeryLate]
                  ,[PPMRecord].[Trend]
                  ,[PPMSectors].[OperatorCode] AS [Code]
                  ,[PPMSectors].[Description] AS [Name]
                FROM [PPMRecord]
                INNER JOIN [PPMSectors] ON [PPMRecord].[PPMSectorId] = [PPMSectors].[PPMSectorId]
                WHERE {0}
                ORDER BY [PPMRecord].[Timestamp] DESC";

            ICollection<string> whereClauses = new List<string>(2);
            if (operatorCode.HasValue)
                whereClauses.Add("[PPMSectors].[OperatorCode] = @operatorCode");
            else
                whereClauses.Add("[PPMSectors].[OperatorCode] IS NULL");
            if (name != null)
                whereClauses.Add("[PPMSectors].[Description] = @name");
            else
                whereClauses.Add("[PPMSectors].[SectorCode] IS NULL");

            return ExecuteScalar<PPMRecord>(string.Format(sql, string.Join(" AND ", whereClauses)), new
            {
                operatorCode,
                name
            });
        }

        public void AddPPMData(PPMRecord record)
        {
            const string sql = @"
                INSERT INTO [PPMRecord]
                    ([PPMSectorId]
                    ,[Timestamp]
                    ,[Total]
                    ,[OnTime]
                    ,[Late]
                    ,[CancelVeryLate]
                    ,[Trend])
                VALUES
                    (@PPMSectorId
                    ,@Timestamp
                    ,@Total
                    ,@OnTime
                    ,@Late
                    ,@CancelVeryLate
                    ,@Trend)";

            ExecuteNonQuery(sql, new
            {
                record.PPMSectorId,
                record.Timestamp,
                record.Total,
                record.OnTime,
                record.Late,
                record.CancelVeryLate,
                record.Trend
            });
        }
    }
}
