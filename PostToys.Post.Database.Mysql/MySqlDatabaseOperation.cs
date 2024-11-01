using System.Data.Common;
using MySql.Data.MySqlClient;
using PostToys.Post.Database.Abstract;

namespace PostToys.Post.Database.Mysql;
public class MySqlDatabaseOperation : AbstractDatabaseOperation
{
    protected override DbCommand CreateCommand(string query, DbConnection connection)
    {
        return new MySqlCommand(query, (MySqlConnection)connection);
    }

    protected override DbConnection CreateConnection(ConnectionInfo connectionInfo)
    {
        // Server：数据库服务器的主机名或 IP 地址。
        // Port：数据库服务器的端口号，默认是 3306。
        // User ID：连接数据库的用户名。
        // Password：连接数据库的密码。
        // Database：要连接的数据库名称。
        string connectionString = $"Server={connectionInfo.Host};Port={connectionInfo.Port ?? "3306"};User ID={connectionInfo.Username};Password={connectionInfo.Password};Database={connectionInfo.Database};";
        return new MySqlConnection(connectionString);
    }

    protected override DbParameter CreateParameter(string parameterName, object value)
    {
        return new MySqlParameter(parameterName, value);
    }
}