using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using Todo.Infra.EventStore;

namespace Todo.Infra.SqliteEventStore
{
    public class SqliteEventStore : IAppendOnlyStore
    {
        private readonly string _connectionString;

        public SqliteEventStore(string connectionString)
        {
            _connectionString = connectionString;
            Initialize();
        }

        private void Initialize()
        {
            const string txt = @"
                CREATE TABLE IF NOT EXISTS ES_Events (
                    Name TEXT NOT NULL,
                    Version INTEGER NOT NULL,
                    EventType TEXT NOT NULL,
                    Data BLOB NOT NULL
                )";

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = new SqliteCommand(txt, conn);
            cmd.ExecuteNonQuery();
        }

        public void Append(string eventType, string streamName, byte[] data, long expectedStreamVersion)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var tx = conn.BeginTransaction();
            const string sql =
                @"SELECT COALESCE(MAX(Version), 0)
                    FROM ES_Events
                    WHERE Name = $name";

            long version;
            using var cmdVersion = new SqliteCommand(sql, conn, tx);
            cmdVersion.Parameters.AddWithValue("$name", streamName);

            version = (long)cmdVersion.ExecuteScalar();

            if (version != expectedStreamVersion)
                throw new AppendOnlyStoreConcurrencyException(version, expectedStreamVersion, streamName);

            const string txt =
                @"INSERT INTO ES_Events (Name, Version, EventType, Data) 
                    VALUES($name, $version, $event, $data)";

            using var cmdAppend = new SqliteCommand(txt, conn, tx);
            cmdAppend.Parameters.AddWithValue("$name", streamName);
            cmdAppend.Parameters.AddWithValue("$version", version + 1);
            cmdAppend.Parameters.AddWithValue("$event", eventType);
            cmdAppend.Parameters.AddWithValue("$data", data);
            cmdAppend.ExecuteNonQuery();

            tx.Commit();
        }

        public IEnumerable<DataWithVersion> ReadRecords(string streamName, long afterVersion, int maxCount)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            const string sql =
                @"SELECT Version, EventType, Data FROM ES_Events
                    WHERE Name = $name AND Version > $version
                    ORDER BY Version
                    LIMIT $take";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("$name", streamName);
            cmd.Parameters.AddWithValue("$version", afterVersion);
            cmd.Parameters.AddWithValue("$take", maxCount);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var data = (byte[])reader["Data"];
                var version = (long)reader["Version"];
                var eventType = (string)reader["EventType"];
                yield return new DataWithVersion(version, data, eventType);
            }
        }

        public IEnumerable<DataWithName> ReadRecords(long afterVersion, int maxCount)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            const string sql =
                @"SELECT Name, EventType, Data FROM ES_Events
                    WHERE Version > $version
                    ORDER BY Version
                    LIMIT $take";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("$version", afterVersion);
            cmd.Parameters.AddWithValue("$take", maxCount);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var data = (byte[])reader["Data"];
                var name = (string)reader["Name"];
                var eventType = (string)reader["EventType"];
                yield return new DataWithName(name, data, eventType);
            }
        }

        public void Close()
        {
        }

        public void Dispose()
        {
        }
    }
}
