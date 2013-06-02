using System;
using System.Collections.Generic;
using TrainNotifier.Common.Model;

namespace TrainNotifier.Service
{
    public class CancellationRepository : DbRepository
    {
        public IEnumerable<ExtendedCancellation> GetCancelledSchedules(DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.AddDays(-1);
            endDate = endDate ?? DateTime.UtcNow.AddDays(1);

            const string sql = @"
                SELECT [CancelledTimestamp]
                      ,[Stanox]
                      ,[ReasonCode]
                      ,[Type]
                FROM [LiveTrainCancellation]
                INNER JOIN [LiveTrain] ON [LiveTrainCancellation].[TrainId] = [LiveTrain].[Id]
                INNER JOIN [ScheduleTrain] ON [LiveTrain].[ScheduleTrain] = [ScheduleTrain].[ScheduleId]";

            throw new NotImplementedException();


        }
    }
}
