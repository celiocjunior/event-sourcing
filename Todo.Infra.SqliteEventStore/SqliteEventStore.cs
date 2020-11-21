using Microsoft.Data.Sqlite;
using System;
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
                    Id TEXT NOT NULL,
                    Version INTEGER NOT NULL,
                    EventType TEXT NOT NULL,
                    Data BLOB NOT NULL
                )";

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = new SqliteCommand(txt, conn);
            cmd.ExecuteNonQuery();
        }

        public void Append(string eventType, string streamId, byte[] data, long expectedStreamVersion)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            using var cmdVersion = new SqliteCommand(
                 @"SELECT COALESCE(MAX(Version), 0)
                    FROM ES_Events
                    WHERE Id = $id", connection, transaction);
            cmdVersion.Parameters.AddWithValue("$id", streamId);

            var version = (long)cmdVersion.ExecuteScalar();
            if (version != expectedStreamVersion)
                throw new AppendOnlyStoreConcurrencyException(version, expectedStreamVersion, streamId);

            using var cmdAppend = new SqliteCommand(
                @"INSERT INTO ES_Events (Id, Version, EventType, Data) 
                    VALUES($id, $version, $event, $data)", connection, transaction);
            cmdAppend.Parameters.AddWithValue("$id", streamId);
            cmdAppend.Parameters.AddWithValue("$version", version + 1);
            cmdAppend.Parameters.AddWithValue("$event", eventType);
            cmdAppend.Parameters.AddWithValue("$data", data);
            cmdAppend.ExecuteNonQuery();

            transaction.Commit();
        }

        public IEnumerable<DataWithVersion> ReadRecords(string streamId, long afterVersion, int maxCount)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var cmd = new SqliteCommand(
                @"SELECT Version, EventType, Data FROM ES_Events
                    WHERE Id = $id AND Version > $version
                    ORDER BY Version
                    LIMIT $take", connection);
            cmd.Parameters.AddWithValue("$id", streamId);
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

        public IEnumerable<DataWithId> ReadRecords(long afterVersion, int maxCount)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            const string sql =
                @"SELECT Id, EventType, Data FROM ES_Events
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
                var id = (string)reader["Id"];
                var eventType = (string)reader["EventType"];
                yield return new DataWithId(id, data, eventType);
            }
        }

        public void Close() { }

        public void Dispose() => GC.SuppressFinalize(this);
    }
}
