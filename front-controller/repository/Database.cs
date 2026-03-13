using Microsoft.Data.Sqlite;

public class MessageRepository
{
    private readonly string _connectionString = "Data Source=messages.db";

    public MessageRepository() 
    {
        EnsureDatabase();
    }

    private void EnsureDatabase() 
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Messages (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Content TEXT NOT NULL
                );
            ";
            cmd.ExecuteNonQuery();
    }

    public void SaveMessage(string message) 
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "INSERT INTO Messages (Content) VALUES ($content)";
        cmd.Parameters.AddWithValue("$content", message);

        cmd.ExecuteNonQuery();
    }

    public List<string> GetAllMessages()
    {   
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Content FROM Messages";

        var messages = new List<string>();

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            messages.Add(reader.GetString(0));
        }

        return messages;
    }
}