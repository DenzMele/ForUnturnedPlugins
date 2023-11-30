using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Logger = Rocket.Core.Logging.Logger;
public class MySQL
{
    private static readonly string connStr = "server=127.0.0.1;user=root;database=UnturnedServer;password=;Pooling=true"; // Ваши данные от MySQL

    public static void Test() // Проверка подключения к БД, вставлять в Load()
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                conn.Open();
                Logger.Log("База Данных MySQL успешно подключена!");
                conn.Close();
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
            }
        }
    }

    public static void Query(MySqlCommand command) // Изменения/добавление чего то
    {
        if (command == null || command.CommandText.Length < 1) { Logger.Log("Wrong command argument: null or empty."); return; }
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                conn.Open();
                command.Connection = conn;
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
            }
        }
    }

    public static DataTable QueryRead(MySqlCommand command) // Чтение столбца
    {
        if (command == null || command.CommandText.Length < 1) { Logger.Log("Wrong command argument: null or empty."); return null; }
        using (MySqlConnection connection = new MySqlConnection(connStr))
        {
            try
            {
                connection.Open();
                command.Connection = connection;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    using (DataTable dt = new DataTable())
                    {
                        dt.Load(reader);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                return null;
            }
        }

    }

    public static async Task<DataTable> QueryReadAsync(MySqlCommand command) // тоже самое только асинхронное
    {
        if (command == null || command.CommandText.Length < 1) { Logger.Log("Wrong command argument: null or empty."); return null; }
        using (MySqlConnection connection = new MySqlConnection(connStr))
        {
            try
            {
                await connection.OpenAsync();
                command.Connection = connection;

                using DbDataReader reader = await command.ExecuteReaderAsync();
                using DataTable dt = new DataTable();
                dt.Load(reader);
                return dt;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                return null;
            }
        }

    }
    public static DataTable GetPlayerFromMySQL(UnturnedPlayer player) // Получить игрока
        {
            string selectQuery = "SELECT * FROM Players WHERE CSteamID = @id";
            MySqlCommand selectCommand = new MySqlCommand(selectQuery);
            selectCommand.Parameters.AddWithValue("@id", player.CSteamID.ToString());
            DataTable tb = MySQL.QueryRead(selectCommand);
            return tb;
        }

     public static void SetPlayerData(UnturnedPlayer up, string Set, int New) // Обновить данные у игрока
        {
            string insertQueryt = $"UPDATE Players SET {Set} = @{Set} WHERE CSteamID = @CSteamID";
            MySqlCommand insertCommandt = new MySqlCommand(insertQueryt);
            insertCommandt.Parameters.AddWithValue($"@{Set}", New);
            insertCommandt.Parameters.AddWithValue("@CSteamID", up.CSteamID.ToString());
            MySQL.Query(insertCommandt);
        }
}
