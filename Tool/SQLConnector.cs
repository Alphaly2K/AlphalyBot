using MySqlConnector;
using Serilog;

namespace AlphalyBot.Tool;

internal class SqlConnector
{
    private static string _connectString;
    private static string _collation;

    private readonly MySqlConnection _msc;

    public SqlConnector(string connectString, string collation)
    {
        _connectString = connectString;
        _collation = collation;
        _msc = new MySqlConnection(_connectString);
    }

    public SqlConnector()
    {
        _msc = new MySqlConnection(_connectString);
    }

    public async Task<bool> IsGroupIdExist(long groupId)
    {
        try
        {
            await _msc.OpenAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal("DbConnector: Error while opening SQL connection");
#if DEBUG
            Log.Fatal(ex.ToString());
#endif
            Environment.Exit(-1);
        }

        var sql =
            $"SELECT CASE WHEN EXISTS (SELECT 1 FROM groupservices WHERE Id = {groupId}) THEN 1 ELSE 0 END AS exists_flag;";
        MySqlCommand cmd = new(sql, _msc);
        var reader = await cmd.ExecuteReaderAsync();
        _ = reader.Read();
        if ((int)reader[0] == 1)
        {
            await _msc.CloseAsync();
            return true;
        }

        await _msc.CloseAsync();
        return false;
    }

    public async Task<string> QueryByGroupId(long groupId)
    {
        try
        {
            await _msc.OpenAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal("DbConnector: Error while opening SQL connection");
#if DEBUG
            Log.Fatal(ex.ToString());
#endif
            Environment.Exit(-1);
        }

        var sql = $"select Service from groupservices where Id = {groupId}";
        MySqlCommand cmd = new(sql, _msc);
        var reader = await cmd.ExecuteReaderAsync();
        _ = reader.Read();
        var tmp = (string)reader[0];
        await _msc.CloseAsync();
        return tmp;
    }

    public async Task InsertByGroupId(long groupId, string services)
    {
        try
        {
            await _msc.OpenAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal("DbConnector: Error while opening SQL connection");
#if DEBUG
            Log.Fatal(ex.ToString());
#endif
            Environment.Exit(-1);
        }

        var sql = $"insert into groupservices values ({groupId},{services})";
        MySqlCommand cmd = new(sql, _msc);
        _ = await cmd.ExecuteNonQueryAsync();
        await _msc.CloseAsync();
    }

    public async Task ChangeByGroupId(long groupId, string services)
    {
        try
        {
            await _msc.OpenAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal("DbConnector: Error while opening SQL connection");
#if DEBUG
            Log.Fatal(ex.ToString());
#endif
            Environment.Exit(-1);
        }

        var sql = @$"update groupservices set Service='{services}' where Id = '{groupId}'";
        MySqlCommand cmd = new(sql, _msc);
        _ = await cmd.ExecuteNonQueryAsync();
        await _msc.CloseAsync();
    }

    public async Task Init()
    {
        try
        {
            await _msc.OpenAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal("DbConnector: Error while opening SQL connection");
#if DEBUG
            Log.Fatal(ex.ToString());
#endif
            Environment.Exit(-1);
        }

        Log.Information("DbConnector: Initializing SQL connection");
        var sql = @"SELECT COUNT(*) FROM information_schema.tables WHERE table_name = 'groupservices';";
        MySqlCommand cmd = new(sql, _msc);
        var reader = await cmd.ExecuteReaderAsync();
        _ = reader.Read();
        if (reader[0].ToString() == "0")
        {
            Log.Information("DbConnector: Table 'groupservices' does not exist");
            var createSql =
                $@"CREATE TABLE `groupservices` ( `Id` BIGINT(20) UNSIGNED ZEROFILL NOT NULL, `Service` TINYTEXT NULL DEFAULT NULL COLLATE '{_collation}', PRIMARY KEY (`Id`) USING BTREE)";
            MySqlCommand createCmd = new(createSql, _msc);
            _ = await createCmd.ExecuteNonQueryAsync();
            Log.Information("DbConnector: Created table `groupservices`");
        }

        await _msc.CloseAsync();
    }
}