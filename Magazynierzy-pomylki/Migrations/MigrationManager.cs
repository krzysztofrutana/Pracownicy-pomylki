using System.Data.SQLite;
using Magazynierzy_pomylki.Database;

namespace Magazynierzy_pomylki.Migrations
{
    public static class MigrationManager
    {
        public static void Migrate()
        {
            CreateTableIfNotExist();
            AddDriverDescriptionAndDriverAction();
            AddCreatedBy();
        }

        private static void CreateTableIfNotExist()
        {
            using var connection = SqLiteContext.CreateConnection();
            if (connection.State == System.Data.ConnectionState.Open)
            {
                string query = @"CREATE TABLE if not exists Issues(
                    Id INTEGER NOT NULL PRIMARY KEY,
                    UserName VARCHAR(255) NOT NULL,
                    Change INTEGER NOT NULL,
                    Date TEXT NOT NULL,
                    Description TEXT NOT NULL
                );";
                var command = new SQLiteCommand(query, connection);
                command.ExecuteNonQuery();
            }
        }

        private static void AddDriverDescriptionAndDriverAction()
        {
            using var connection = SqLiteContext.CreateConnection();
            if (connection.State == System.Data.ConnectionState.Open)
            {
                if (!connection.IsColumnExists("Issues", "IsDriverExplainNeed")
                    && !connection.IsColumnExists("Issues", "ExplainedAtDriver")
                    && !connection.IsColumnExists("Issues", "DriverExplain"))
                {
                    string query = @"
                    BEGIN TRANSACTION;
                    ALTER TABLE Issues ADD COLUMN IsDriverExplainNeed INTEGER NOT NULL DEFAULT 0;
                    ALTER TABLE Issues ADD COLUMN ExplainedAtDriver INTEGER NOT NULL DEFAULT 0;
                    ALTER TABLE Issues ADD COLUMN DriverExplain TEXT NULL;
                    COMMIT";

                    var command = new SQLiteCommand(query, connection);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void AddCreatedBy()
        {
            using var connection = SqLiteContext.CreateConnection();
            if (connection.State == System.Data.ConnectionState.Open)
            {
                if (!connection.IsColumnExists("Issues", "CreatedBy"))
                {
                    string query = @"
                    BEGIN TRANSACTION;
                    ALTER TABLE Issues ADD COLUMN CreatedBy VARCHAR(255) NOT NULL DEFAULT 'Nie wybrano';
                    COMMIT";

                    var command = new SQLiteCommand(query, connection);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool IsColumnExists(this SQLiteConnection connection, string table, string column)
        {
            bool isExists = false;
            string query = $@"PRAGMA table_info({table});";
            var command = new SQLiteCommand(query, connection);
            using SQLiteDataReader reader = command.ExecuteReader();

            if (reader != null)
            {
                while (reader.Read())
                {
                    string name = reader.GetString(1);
                    if (column.Equals(name))
                    {
                        isExists = true;
                        break;
                    }
                }
            }

            return isExists;
        }
    }
}
