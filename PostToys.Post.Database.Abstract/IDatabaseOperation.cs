using System.Collections.Immutable;

namespace PostToys.Post.Database.Abstract;

public interface IDatabaseOperation
{
    void OpenConnection(ConnectionInfo connectionInfo);
    void CloseConnection();
    List<ImmutableSortedDictionary<string, object>> ExecuteQuery(string query);
    int ExecuteNonQuery(string query, Dictionary<string, object> parameters);
}