using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Service
{
    public class AssociationRepository : DbRepository
    {
        public Guid AddAssociation(Association a)
        {
            const string sql = @"
                INSERT INTO [dbo].[TrainAssociation]
                           ([MainTrainUid]
                           ,[AssocTrainUid]
                           ,[StartDate]
                           ,[EndDate]
                           ,[AppliesMonday]
                           ,[AppliesTuesday]
                           ,[AppliesWednesday]
                           ,[AppliesThursday]
                           ,[AppliesFriday]
                           ,[AppliesSaturday]
                           ,[AppliesSunday]
                           ,[LocationTiplocId]
                           ,[AssociationDate]
                           ,[AssociationType]
                           ,[STPIndicatorId]
                           ,[Deleted])
                    OUTPUT [inserted].[AssociationId]
                     VALUES
                           (@mainTrainUid
                           ,@assocTrainUid
                           ,@startDate
                           ,@endDate
                           ,@appliesMonday
                           ,@appliesTuesday
                           ,@appliesWednesday
                           ,@appliesThursday
                           ,@appliesFriday
                           ,@appliesSaturday
                           ,@appliesSunday
                           ,@locationTiplocId
                           ,@associationDate
                           ,@associationType
                           ,@stpIndicatorId
                           ,@deleted)";

            Guid id = ExecuteInsert(sql, new
            {
                mainTrainUid = a.MainTrainUid,
                assocTrainUid = a.AssocTrainUid,
                startDate = a.StartDate,
                endDate = a.EndDate,
                appliesMonday = GetBoolean(a.Schedule, (s) => s.Monday),
                appliesTuesday = GetBoolean(a.Schedule, (s) => s.Tuesday),
                appliesWednesday = GetBoolean(a.Schedule, (s) => s.Wednesday),
                appliesThursday = GetBoolean(a.Schedule, (s) => s.Thursday),
                appliesFriday = GetBoolean(a.Schedule, (s) => s.Friday),
                appliesSaturday = GetBoolean(a.Schedule, (s) => s.Saturday),
                appliesSunday = GetBoolean(a.Schedule, (s) => s.Sunday),
                locationTiplocId = a.Location.TiplocId,
                associationDate = a.DateType,
                associationType = a.AssociationType,
                stpIndicatorId = a.STPIndicator,
                deleted = a.TransactionType == TransactionType.Delete
            });
            a.AssociationId = id;
            return id;
        }

        private bool? GetBoolean(Schedule s, Func<Schedule, bool> selector)
        {
            return s == null ? default(bool?) : selector(s);
        }

        public IEnumerable<Association> GetForTrain(string trainUid, DateTime date)
        {
            const string sql = @"
                SELECT [TrainAssociation].[AssociationId]
                      ,[TrainAssociation].[MainTrainUid]
                      ,[TrainAssociation].[AssocTrainUid]
                      ,[TrainAssociation].[StartDate]
                      ,[TrainAssociation].[EndDate]
                      ,[TrainAssociation].[AppliesMonday]
                      ,[TrainAssociation].[AppliesTuesday]
                      ,[TrainAssociation].[AppliesWednesday]
                      ,[TrainAssociation].[AppliesThursday]
                      ,[TrainAssociation].[AppliesFriday]
                      ,[TrainAssociation].[AppliesSaturday]
                      ,[TrainAssociation].[AppliesSunday]
                      ,[TrainAssociation].[LocationTiplocId]
                      ,[TrainAssociation].[AssociationDate]
                      ,[TrainAssociation].[AssociationType]
                      ,[TrainAssociation].[Deleted]
                      ,[TrainAssociation].[STPIndicatorId] AS [STPIndicator]
                      ,[Tiploc].[TiplocId]
                      ,[Tiploc].[Tiploc]
                      ,[Tiploc].[Nalco]
                      ,[Tiploc].[Description]
                      ,[Tiploc].[Stanox]
                      ,[Tiploc].[CRS]
                  FROM [TrainAssociation]
                  INNER JOIN [Tiploc] ON [TrainAssociation].[LocationTiplocId] = [Tiploc].[TiplocId]
                  WHERE ([TrainAssociation].[MainTrainUid] = @trainUid OR [TrainAssociation].[AssocTrainUid] = @trainUid)
                  AND @date >= [TrainAssociation].[StartDate]
                  AND @date <= [TrainAssociation].[EndDate]
                  AND [TrainAssociation].[Applies{0}] = 1";

            IEnumerable<Association> assocs = Enumerable.Empty<Association>();
            using (DbConnection dbConnection = CreateAndOpenConnection())
            {
                assocs = dbConnection.Query<Association, TiplocCode, Association>(
                    string.Format(sql, date.DayOfWeek),
                    (a, l) =>
                    {
                        a.Location = l;
                        return a;
                    },
                    new
                    {
                        trainUid,
                        date = date.Date
                    },
                    splitOn: "TiplocId").ToList();
            }

            foreach (var grouping in assocs.GroupBy(a => new { a.MainTrainUid, a.AssocTrainUid }))
            {
                if (!grouping.Any(a => a.Deleted && a.StartDate.Date == date.Date))
                {
                    if (grouping.Count() == 1)
                        yield return grouping.ElementAt(0);
                    else
                        yield return grouping.OrderBy(a => a.STPIndicator).First();
                }
            }
        }
    }
}
