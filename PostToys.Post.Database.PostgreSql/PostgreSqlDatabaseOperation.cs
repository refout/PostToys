using System.Data.Common;
using Npgsql;
using PostToys.Post.Database.Abstract;

namespace PostToys.Post.Database.PostgreSql;
public class PostgreSqlDatabaseOperation : AbstractDatabaseOperation
{
    protected override DbCommand CreateCommand(string query, DbConnection connection)
    {
        return new NpgsqlCommand(query, (NpgsqlConnection)connection);
    }

    protected override DbConnection CreateConnection(ConnectionInfo connectionInfo)
    {
        // Host：数据库服务器的主机名或 IP 地址。
        // Port：数据库服务器的端口号，默认是 5432。
        // Username：连接数据库的用户名。
        // Password：连接数据库的密码。
        // Database：要连接的数据库名称。
        var connectionString = $"Host={connectionInfo.Host};Port={connectionInfo.Port ?? "5432"};Username={connectionInfo.Username};Password={connectionInfo.Password};Database={connectionInfo.Database};";
        return new NpgsqlConnection(connectionString);
    }

    protected override DbParameter CreateParameter(string parameterName, object value)
    {
        return new NpgsqlParameter(parameterName, value);
    }
}