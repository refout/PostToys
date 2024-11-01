using System.Collections.Immutable;
using System.Data;
using System.Data.Common;

namespace PostToys.Post.Database.Abstract;

public abstract class AbstractDatabaseOperation : IDatabaseOperation
{
    private DbConnection? _connection;
    protected abstract DbConnection CreateConnection(ConnectionInfo connectionInfo);
    protected abstract DbCommand CreateCommand(string query, DbConnection connection);
    protected abstract DbParameter CreateParameter(string parameterName, object value);

    public void OpenConnection(ConnectionInfo connectionInfo)
    {
        _connection ??= CreateConnection(connectionInfo);
        _connection.Open();
        Console.WriteLine("Connection to opened successfully.");
    }

    public void CloseConnection()
    {
        if (_connection is not { State: ConnectionState.Open })
        {
            return;
        }

        _connection.Close();
        Console.WriteLine("Connection to closed.");
    }

    public List<ImmutableSortedDictionary<string, object>> ExecuteQuery(string query)
    {
        List<ImmutableSortedDictionary<string, object>> result = [];

        using var command = CreateCommand(query, Connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var row = Enumerable.Range(0, reader.FieldCount)
                .ToImmutableSortedDictionary(reader.GetName, reader.GetValue);
            result.Add(row);
        }

        return result;
    }

    public int ExecuteNonQuery(string query, Dictionary<string, object> parameters)
    {
        using var command = CreateCommand(query, Connection);
        foreach (var parameter in parameters)
        {
            command.Parameters.Add(CreateParameter(parameter.Key, parameter.Value));
        }

        return command.ExecuteNonQuery();
    }

    private DbConnection Connection
    {
        get
        {
            if (_connection is not null && _connection.State == ConnectionState.Open)
            {
                return _connection;
            }

            throw new InvalidOperationException("");
        }
    }
}