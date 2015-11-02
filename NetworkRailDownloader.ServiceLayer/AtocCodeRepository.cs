using System.Collections.Generic;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Service
{
    public class AtocCodeRepository : DbRepository
    {
        public IEnumerable<AtocCode> GetAtocCodes()
        {
            const string sql = @"
                SELECT [AtocCode]
                      ,[Name]
                      ,[NumericCode]
                  FROM [dbo].[AtocCode]";

            return Query<AtocCode>(sql);
        }
        public AtocCode GetByNumericCode(byte numericCode)
        {
            const string sql = @"
                SELECT TOP 1
                      [AtocCode]
                      ,[Name]
                      ,[NumericCode]
                  FROM [dbo].[AtocCode]
                  WHERE [NumericCode] = @numericCode";

            return ExecuteScalar<AtocCode>(sql, new { numericCode });
        }

        public void InsertAtocCode(AtocCode code)
        {
            const string sql = @"
                INSERT INTO [dbo].[AtocCode]
                           ([AtocCode]
                           ,[Name]
                           ,[NumericCode])
                     VALUES
                           (@code
                           ,@name
                           ,@numericCode)";

            ExecuteNonQuery(sql, new { 
                code = code.Code,
                name = code.Name,
                numericCode = code.NumericCode
            });
        }
    }
}
