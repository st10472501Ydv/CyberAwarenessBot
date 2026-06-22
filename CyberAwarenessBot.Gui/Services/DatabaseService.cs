using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using CyberAwarenessBot.Gui.Models;

namespace CyberAwarenessBot.Gui.Services
{
    public class DatabaseService
    {
        private const string ConnectionString = "Server=localhost;Database=cyberawareness_bot;Uid=root;Pwd=@Labs2026!;AllowUserVariables=True;";

        public void Initialize()
        {
            using var rootConn = new MySqlConnection("Server=localhost;Uid=root;Pwd=@Labs2026!;");
            rootConn.Open();
            using var createDb = new MySqlCommand("CREATE DATABASE IF NOT EXISTS cyberawareness_bot", rootConn);
            createDb.ExecuteNonQuery();
            rootConn.Close();

            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var createTable = new MySqlCommand(@"
                CREATE TABLE IF NOT EXISTS tasks (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    Title VARCHAR(255) NOT NULL,
                    Description TEXT,
                    ReminderDate DATETIME NULL,
                    IsCompleted TINYINT(1) DEFAULT 0,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                )", conn);
            createTable.ExecuteNonQuery();
        }

        private MySqlConnection GetConnection()
        {
            var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        public int AddTask(CyberTask task)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(
                "INSERT INTO tasks (Title, Description, ReminderDate, IsCompleted) VALUES (@Title, @Description, @ReminderDate, @IsCompleted); SELECT LAST_INSERT_ID();", conn);
            cmd.Parameters.AddWithValue("@Title", task.Title);
            cmd.Parameters.AddWithValue("@Description", task.Description);
            cmd.Parameters.AddWithValue("@ReminderDate", (object?)task.ReminderDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsCompleted", task.IsCompleted);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<CyberTask> GetAllTasks()
        {
            var tasks = new List<CyberTask>();
            using var conn = GetConnection();
            using var cmd = new MySqlCommand("SELECT * FROM tasks ORDER BY CreatedAt DESC", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tasks.Add(new CyberTask
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? "" : reader.GetString(reader.GetOrdinal("Title")),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString(reader.GetOrdinal("Description")),
                    ReminderDate = reader.IsDBNull(reader.GetOrdinal("ReminderDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ReminderDate")),
                    IsCompleted = reader.GetBoolean(reader.GetOrdinal("IsCompleted")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                });
            }
            return tasks;
        }

        public void UpdateTask(CyberTask task)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(
                "UPDATE tasks SET Title=@Title, Description=@Description, ReminderDate=@ReminderDate, IsCompleted=@IsCompleted WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", task.Id);
            cmd.Parameters.AddWithValue("@Title", task.Title);
            cmd.Parameters.AddWithValue("@Description", task.Description);
            cmd.Parameters.AddWithValue("@ReminderDate", (object?)task.ReminderDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsCompleted", task.IsCompleted);
            cmd.ExecuteNonQuery();
        }

        public void DeleteTask(int id)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand("DELETE FROM tasks WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
