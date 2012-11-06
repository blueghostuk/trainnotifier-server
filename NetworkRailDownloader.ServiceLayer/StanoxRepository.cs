namespace TrainNotifier.ServiceLayer
{
    public class StanoxRepository : DbRepository
    {
        public string GetStanoxByCrs(string crs)
        {
            const string sql = "SELECT Stanox FROM Tiploc WHERE CRS = @crs";

            return ExecuteScalar<string>(sql, new { crs });
        }
    }
}
