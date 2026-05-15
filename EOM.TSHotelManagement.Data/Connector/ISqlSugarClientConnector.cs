using SqlSugar;

namespace EOM.TSHotelManagement.Data
{
    public interface ISqlSugarClientConnector
    {
        ISqlSugarClient CreateClient(string dbName = null);
    }
}
