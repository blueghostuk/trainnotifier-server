using System.Collections.Generic;
using TrainNotifier.Common.Model.PPM;

namespace TrainNotifier.Service
{
    public class PPMDataRepository : DbRepository
    {
        public IEnumerable<PPMSector> GetSectors()
        {
            const string sql = @"
                SELECT [PPMSectorId]
                    ,[OperatorCode]
                    ,[SectorCode]
                    ,[Description]
                FROM [PPMSectors]";

            return Query<PPMSector>(sql);
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
