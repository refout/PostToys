using System.Diagnostics;
using PostToys.Common;
using PostToys.Parse.Model;
using PostToys.Post.Database.Abstract;
using PostToys.Post.Database.Mysql;
using PostToys.Post.Database.PostgreSql;
using PostToys.Post.Model;

namespace PostToys.Post.Database;

public class DatabasePost : IPost
{
    /// <summary>
    /// 服务管理器
    /// </summary>
    private readonly ServiceManager _serviceManager = new();

    public DatabasePost()
    {
        _serviceManager.AddKeyedSingleton<IDatabaseOperation, MySqlDatabaseOperation>("mysql");
        _serviceManager.AddKeyedSingleton<IDatabaseOperation, PostgreSqlDatabaseOperation>("postgresql");
    }

    private static readonly ParseUrl ParseUrl = new()
    {
        LastParams = ["mysql", "postgresql"]
    };

    public IBoy Post(Toy toy)
    {
        if (!ParseUrl.TryParseUrlExp(toy.Url, out var method, out var connectionStr, out var databaseType))
        {
            return new DatabaseBoy();
        }

        var operation = _serviceManager.GetKeyedService<IDatabaseOperation>(databaseType.ToLower());
        var boy = new DatabaseBoy()
        {
            Toy = toy,
            TakeTime = 0
        };
        if (operation == null)
        {
            return boy;
        }

        try
        {
            if (string.IsNullOrEmpty(connectionStr))
            {
                return boy;
            }

            var map = ParseConnectionString(connectionStr);
            var info = new ConnectionInfo
            {
                Username = map["username"],
                Password = map["password"],
                Host = map["host"],
                Port = map["port"],
                Database = map["database"]
            };
            operation.OpenConnection(info);


            var stopwatch = Stopwatch.StartNew();

            var result = method switch
            {
                "DQL" => JsonUtil.ToJson(operation.ExecuteQuery(toy.Body)),
                "DDL" or "DML" or "DCL" => operation.ExecuteNonQuery(toy.Body, new Dictionary<string, object>())
                    .ToString(),
                _ => throw new ArgumentOutOfRangeException(nameof(toy), "未知的执行类型：" + method)
            };

            var takeTime = stopwatch.ElapsedMilliseconds;

            return boy with
            {
                Body = result,
                Method = method,
                IsSuccessStatusCode = true,
                DatabaseType = databaseType,
                Url = connectionStr,
                TakeTime = takeTime
            };
        }
        finally
        {
            operation.CloseConnection();
        }
    }

    private static Dictionary<string, string> ParseConnectionString(string connectionString)
    {
        // 创建字典
        Dictionary<string, string> dict = [];

        // 分隔连接字符串
        var pairs = connectionString.Split(';');

        foreach (var pair in pairs)
        {
            if (string.IsNullOrWhiteSpace(pair))
            {
                continue;
            }

            var keyValue = pair.Split('=');
            if (keyValue.Length == 2)
            {
                dict[keyValue[0].Trim()] = keyValue[1].Trim().ToLower();
            }
        }

        return dict;
    }
}