using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                deleted = a.TransactionType == TransactionType.Delete
            });
            a.AssociationId = id;
            return id;
        }

        private bool? GetBoolean(Schedule s, Func<Schedule, bool> selector)
        {
            return s == null ? default(bool?) : selector(s);
        }
    }
}
